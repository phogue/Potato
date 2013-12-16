using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Procon.Core.Connections.TextCommands.Parsers.Fuzzy;
using Procon.Fuzzy;
using Procon.Fuzzy.Tokens.Object;
using Procon.Fuzzy.Tokens.Primitive;
using Procon.Fuzzy.Tokens.Primitive.Numeric;
using Procon.Fuzzy.Tokens.Primitive.Temporal;
using Procon.Fuzzy.Utils;
using Procon.Net.Actions;
using Procon.Net.Geolocation;
using Procon.Net.Models;

namespace Procon.Core.Connections.TextCommands.Parsers {

    /// <summary>
    /// Finds matches agaisnt text with no structure. Extracts various information from the text
    /// </summary>
    public class FuzzyParser : Parser, IFuzzyState {

        /// <summary>
        /// The document to use for localization purposes. This is a raw format to be used
        /// by Procon.Fuzzy
        /// </summary>
        public XElement Document { get; set; }

        /// <summary>
        /// Dictionary of cached property info fetches. Minor optimization.
        /// </summary>
        protected Dictionary<String, PropertyInfo> PropertyInfoCache = new Dictionary<string, PropertyInfo>();

        protected int MinimumSimilarity(int lower, int upper, int maximumLength, int itemLength) {
            return lower + (upper - lower) * (itemLength / maximumLength);
        }

        protected void ParseMapNames(Phrase phrase) {
            var mapNames = this.Connection.GameState.MapPool.Select(map => new {
                map,
                similarity = Math.Max(map.FriendlyName.DePluralStringSimularity(phrase.Text), map.Name.DePluralStringSimularity(phrase.Text))
            })
            .Where(@t => @t.similarity >= 60)
            .Select(@t => new ThingObjectToken() {
                Reference = new MapThingReference() {
                    Maps = new List<Map>() {
                        @t.map
                    }
                },
                Text = phrase.Text,
                Similarity = @t.similarity,
                MinimumWeightedSimilarity = 60
            });

            List<Token> names = new List<Token>();
            mapNames.ToList().ForEach(names.Add);

            phrase.AppendDistinctRange(names);
        }

        protected void ParsePlayerNames(Phrase phrase) {

            // We should cache this some where.
            int maximumNameLength = this.Connection.GameState.Players.Count > 0 ? this.Connection.GameState.Players.Max(player => player.Name.Length) : 0;

            var playerNames = this.Connection.GameState.Players.Select(player => new {
                player,
                similarity = Math.Max(player.NameStripped.DePluralStringSimularity(phrase.Text), player.Name.DePluralStringSimularity(phrase.Text))
            })
            .Where(@t => @t.similarity >= this.MinimumSimilarity(55, 70, maximumNameLength, @t.player.Name.Length))
            .Select(@t => new ThingObjectToken() {
                Reference = new PlayerThingReference() {
                    Players = new List<Player>() {
                        @t.player
                    }
                },
                Text = phrase.Text,
                Similarity = @t.similarity,
                MinimumWeightedSimilarity = 55
            });

            List<Token> names = new List<Token>();
            playerNames.ToList().ForEach(names.Add);

            phrase.AppendDistinctRange(names);
        }

        protected void ParseCountryNames(Phrase phrase) {
            var playerCountries = this.Connection.GameState.Players.Select(player => new {
                player,
                similarity = player.Location.CountryName.StringSimularitySubsetBonusRatio(phrase.Text)
            })
            .Where(@t => @t.similarity >= 60)
            .Select(@t => new ThingObjectToken() {
                Reference = new LocationThingReference() {
                    Locations = new List<Location>() {
                        @t.player.Location
                    }
                },
                Text = phrase.Text,
                Similarity = @t.similarity,
                MinimumWeightedSimilarity = 60
            });

            List<Token> names = new List<Token>();
            playerCountries.ToList().ForEach(names.Add);

            phrase.AppendDistinctRange(names);
        }

        protected void ParseItemNames(Phrase phrase) {
            var playerItems = this.Connection.GameState.Players.SelectMany(player => player.Inventory.Items).Select(item => new {
                item,
                similarity = Math.Max(item.FriendlyName.StringSimularitySubsetBonusRatio(phrase.Text), item.Tags.Select(tag => tag.StringSimularitySubsetBonusRatio(phrase.Text)).Max())
            })
            .Where(@t => @t.similarity >= 60)
            .Select(@t => new ThingObjectToken() {
                Reference = new ItemThingReference() {
                    Items = new List<Item>() {
                        @t.item
                    }
                },
                Text = phrase.Text,
                Similarity = @t.similarity,
                MinimumWeightedSimilarity = 60
            });

            List<Token> names = new List<Token>();
            playerItems.ToList().ForEach(names.Add);

            phrase.AppendDistinctRange(names);
        }

        public Phrase ParseThing(IFuzzyState state, Phrase phrase) {

            if (phrase.Any()) {
                ThingObjectToken thing = phrase.First() as ThingObjectToken;

                if (thing != null) {
                    if (thing.Name == "Players") {
                        thing.Reference = new PlayerThingReference() {
                            Players = new List<Player>(this.Connection.GameState.Players)
                        };
                    }
                    else if (thing.Name == "Maps") {
                        thing.Reference = new MapThingReference() {
                            Maps = new List<Map>(this.Connection.GameState.MapPool)
                        };
                    }
                }
            }

            this.ParsePlayerNames(phrase);
            this.ParseMapNames(phrase);
            this.ParseCountryNames(phrase);
            this.ParseItemNames(phrase);

            return phrase;
        }

        private float MaximumLevenshtein(string argument, List<string> commands) {
            float max = 0.0F;

            commands.ForEach(x => max = Math.Max(max, x.StringSimularitySubsetBonusRatio(argument)));

            return max;
        }

        public Phrase ParseMethod(IFuzzyState state, Phrase phrase) {
            var methods = this.TextCommands.Select(textCommand => new {
                textCommand,
                similarity = this.MaximumLevenshtein(phrase.Text, textCommand.Commands)
            })
            .Where(@t => @t.similarity >= 60)
            .Select(@t => new MethodObjectToken() {
                MethodName = @t.textCommand.PluginCommand,
                Text = phrase.Text,
                Similarity = @t.similarity,
                MinimumWeightedSimilarity = 60
            });

            List<Token> names = new List<Token>();
            methods.ToList().ForEach(names.Add);
            phrase.AppendDistinctRange(names);

            return phrase;
        }

        public Phrase ParseProperty(IFuzzyState state, Phrase phrase) {
            // Edit each NumericPropertyObjectToken
            foreach (NumericPropertyObjectToken token in phrase.Where(token => token is NumericPropertyObjectToken)) {
                if (token.Name == "Ping") {
                    token.Reference = new PingPropertyReference();
                }
                else if (token.Name == "Score") {
                    token.Reference = new ScorePropertyReference();
                }
                else if (token.Name == "Kills") {
                    token.Reference = new KillsPropertyReference();
                }
                else if (token.Name == "Deaths") {
                    token.Reference = new DeathsPropertyReference();
                }
                else if (token.Name == "Kdr") {
                    token.Reference = new KdrPropertyReference();
                }
            }

            return phrase;
        }

        /// <summary>
        /// Finds the player object of the speaker to reference "me"
        /// </summary>
        /// <param name="state"></param>
        /// <param name="selfThing"></param>
        /// <returns></returns>
        public SelfReflectionThingObjectToken ParseSelfReflectionThing(IFuzzyState state, SelfReflectionThingObjectToken selfThing) {
            if (this.SpeakerPlayer != null) {
                selfThing.Reference = new PlayerThingReference() {
                    Players = new List<Player>() {
                        this.SpeakerPlayer
                    }
                };
            }

            return selfThing;
        }

        private List<TextCommand> ExtractCommandList(Sentence sentence) {

            // We need to know this method ahead of time so we can clear all other tokens in this phrase.
            MethodObjectToken mainMethod = sentence.ExtractFirstOrDefault<MethodObjectToken>();
            List<MethodObjectToken> resultMethodList = new List<MethodObjectToken>();

            foreach (Phrase phrase in sentence) {

                if (phrase.Count > 0 && phrase[0] == mainMethod) {
                    // Only bubble up very close matching arguments.
                    phrase.RemoveAll(token => token.Similarity < 80);
                }

                // Select them as good alternatives 
                resultMethodList.AddRange(phrase.OfType<MethodObjectToken>().ToList());

                // Then remove them for the remainder of the execution.
                phrase.RemoveAll(token => token is MethodObjectToken);
            }

            resultMethodList = resultMethodList.OrderByDescending(token => token.Similarity)
                                               .ThenByDescending(token => token.Text.Length)
                                               .ToList();

            List<TextCommand> results = resultMethodList.Select(method => this.TextCommands.Find(command => command.PluginCommand == method.MethodName)).Where(command => command != null).ToList();

            return results.OrderByDescending(token => token.Priority).ToList();
        }

        /// <summary>
        /// Extracts a list of things from the sentence, combining sets and loose things.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sentence"></param>
        /// <returns></returns>
        public List<T> ExtractThings<T>(Sentence sentence) where T : IThingReference {
            List<T> things = sentence.ScrapeStrictList<ThingObjectToken>().Where(token => token.Reference is T).Select(token => token.Reference).Cast<T>().ToList();

            things.AddRange(sentence.ScrapeStrictList<SelfReflectionThingObjectToken>().Where(token => token.Reference is T).Select(token => token.Reference).Cast<T>());
            
            return things;
        }

        protected TextCommandInterval ExtractTextCommandInterval(Sentence sentence) {
            TextCommandInterval interval = null;

            FuzzyDateTimePattern pattern = sentence.ExtractList<TemporalToken>().Where(token => token.Pattern != null && token.Pattern.Modifier == TimeModifier.Interval)
                                           .Select(token => token.Pattern)
                                           .FirstOrDefault();

            if (pattern != null) {
                interval = new TextCommandInterval() {
                    Day = pattern.Day,
                    DayOfWeek = pattern.DayOfWeek,
                    Hour = pattern.Hour,
                    IntervalType = (TextCommandIntervalType) Enum.Parse(typeof (TextCommandIntervalType), pattern.TemporalInterval.ToString()),
                    Minute = pattern.Minute,
                    Month = pattern.Month,
                    Second = pattern.Second,
                    Year = pattern.Year,
                };
            }

            return interval;
        }

        public override CommandResultArgs Parse(string prefix, string text) {
            Sentence sentence = new Sentence().Parse(this, text).Reduce(this);

            CommandResultArgs result = null;
            
            List<TextCommand> commands = this.ExtractCommandList(sentence);
            TextCommand priorityCommand = commands.FirstOrDefault();

            List<String> quotes = sentence.Where(token => token.Count > 0 && token[0] is StringPrimitiveToken).Select(token => token[0].Text).ToList();

            List<TemporalToken> timeTokens = sentence.ExtractList<TemporalToken>();
            DateTime? delay = timeTokens.Where(token => token.Pattern != null && token.Pattern.Modifier == TimeModifier.Delay)
                                        .Select(token => token.Pattern.ToDateTime())
                                        .FirstOrDefault();

            TimeSpan? period = timeTokens.Where(token => token.Pattern != null && (token.Pattern.Modifier == TimeModifier.Period || token.Pattern.Modifier == TimeModifier.None))
                                         .Select(token => token.Pattern.ToTimeSpan())
                                         .FirstOrDefault();
            
            // Must have a method to execute on, the rest is optional.
            if (priorityCommand != null) {
                commands.Remove(priorityCommand);

                result = new CommandResultArgs() {
                    Success = true,
                    Status = CommandResultType.Success,
                    Now = new CommandData() {
                        Players = new List<Player>() {
                            this.SpeakerPlayer
                        },
                        TextCommands = new List<TextCommand>() {
                            priorityCommand
                        }
                        .Concat(commands)
                        .ToList(),
                        TextCommandMatches = new List<TextCommandMatch>() {
                            new TextCommandMatch() {
                                Prefix = prefix,
                                Players = this.ExtractThings<PlayerThingReference>(sentence).SelectMany(thing => thing.Players).ToList(),
                                Maps = this.ExtractThings<MapThingReference>(sentence).SelectMany(thing => thing.Maps).ToList(),
                                Numeric = sentence.ExtractList<FloatNumericPrimitiveToken>().Select(token => token.ToFloat()).ToList(),
                                Delay = delay,
                                Period = period,
                                Interval = this.ExtractTextCommandInterval(sentence),
                                Text = text,
                                Quotes = quotes
                            }
                        }
                    }
                };
            }

            return result;
        }
    }
}
