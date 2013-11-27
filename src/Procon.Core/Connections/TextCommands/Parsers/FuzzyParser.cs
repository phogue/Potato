using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Procon.Net;
using Procon.Net.Actions;
using Procon.Net.Data;

namespace Procon.Core.Connections.TextCommands.Parsers {
    using Procon.Core.Events;
    using Procon.Fuzzy;
    using Procon.Fuzzy.Utils;
    using Procon.Fuzzy.Tokens.Object;
    using Procon.Fuzzy.Tokens.Object.Sets;
    using Procon.Fuzzy.Tokens.Primitive;
    using Procon.Fuzzy.Tokens.Primitive.Numeric;
    using Procon.Fuzzy.Tokens.Primitive.Temporal;
    using Procon.Fuzzy.Tokens.Reduction;

    public class FuzzyParser : Parser {

        /// <summary>
        /// Dictionary of cached property info fetches. Minor optimization.
        /// </summary>
        protected Dictionary<String, PropertyInfo> PropertyInfoCache { get; set; }

        public FuzzyParser() {
            this.PropertyInfoCache = new Dictionary<string, PropertyInfo>();
        }

        #region Parsing

        protected int MinimumSimilarity(int lower, int upper, int maximumLength, int itemLength) {
            return lower + (upper - lower) * (itemLength / maximumLength);
        }

        protected void ParseMapNames(Phrase phrase) {

            PropertyInfo aliasId = this.GetPropertyInfo<Map>("Name");

            var mapNames = this.Connection.GameState.MapPool.Select(map => new {
                map,
                similarity = Math.Max(map.FriendlyName.DePluralStringSimularity(phrase.Text), map.Name.DePluralStringSimularity(phrase.Text))
            }).Where(@t => @t.similarity >= 60).Select(@t => new ThingObjectToken() {
                Reference = @t.map.Name,
                ReferenceProperty = aliasId,
                Text = phrase.Text,
                Similarity = @t.similarity,
                MinimumWeightedSimilarity = 60
            });

            List<Token> names = new List<Token>();
            mapNames.ToList().ForEach(names.Add);

            phrase.AddDistinctRange(names);
        }

        protected void ParsePlayerNames(Phrase phrase) {

            PropertyInfo aliasId = this.GetPropertyInfo<Player>("Uid");

            // We should cache this some where.
            int maximumNameLength = this.Connection.GameState.Players.Count > 0 ? this.Connection.GameState.Players.Max(player => player.Name.Length) : 0;

            var playerNames = this.Connection.GameState.Players.Select(player => new {
                player,
                similarity = Math.Max(player.NameStripped.DePluralStringSimularity(phrase.Text), player.Name.DePluralStringSimularity(phrase.Text))
            }).Where(@t => @t.similarity >= this.MinimumSimilarity(55, 70, maximumNameLength, @t.player.Name.Length)).Select(@t => new ThingObjectToken() {
                Reference = @t.player.Uid,
                ReferenceProperty = aliasId,
                Text = phrase.Text,
                Similarity = @t.similarity,
                MinimumWeightedSimilarity = 55
            });

            List<Token> names = new List<Token>();
            playerNames.ToList().ForEach(names.Add);

            phrase.AddDistinctRange(names);
        }

        protected void ParseCountryNames(Phrase phrase) {
            PropertyInfo countryName = this.GetPropertyInfo<Player>("CountryName");

            var playerCountries = this.Connection.GameState.Players.Select(player => new {
                player,
                similarity = player.Location.CountryName.StringSimularitySubsetBonusRatio(phrase.Text)
            }).Where(@t => @t.similarity >= 60).Select(@t => new ThingObjectToken() {
                Reference = @t.player.Location.CountryName,
                ReferenceProperty = countryName,
                Text = phrase.Text,
                Similarity = @t.similarity,
                MinimumWeightedSimilarity = 60
            });

            List<Token> names = new List<Token>();
            playerCountries.ToList().ForEach(names.Add);

            phrase.AddDistinctRange(names);

        }

        public override Phrase ParseThing(IFuzzyState state, Phrase phrase) {

            this.ParsePlayerNames(phrase);
            this.ParseMapNames(phrase);
            this.ParseCountryNames(phrase);
            //this.ParseRegionNames(phrase);
            //this.ParseCityNames(phrase);
            //this.ParseItemNames(phrase);

            return phrase;
        }

        private float MaximumLevenshtein(string argument, List<string> commands) {

            float max = 0.0F;

            commands.ForEach(x => max = Math.Max(max, x.StringSimularitySubsetBonusRatio(argument)));

            return max;
        }

        /// <summary>
        /// Parses the text for any commands it can use.
        /// </summary>
        /// <param name="state"></param>
        /// <param name="phrase"></param>
        /// <returns></returns>
        public override Phrase ParseMethod(IFuzzyState state, Phrase phrase) {

            var methods = from textCommand in this.TextCommands
                          let similarity = this.MaximumLevenshtein(phrase.Text, textCommand.Commands)
                          where similarity >= 60
                          select new MethodObjectToken() {
                              MethodName = textCommand.PluginCommand,
                              Text = phrase.Text,
                              Similarity = similarity,
                              MinimumWeightedSimilarity = 60
                          };

            List<Token> names = new List<Token>();
            methods.ToList().ForEach(names.Add);
            phrase.AddDistinctRange(names);

            return phrase;
        }

        public override SelfReflectionThingObjectToken ParseSelfReflectionThing(IFuzzyState state, SelfReflectionThingObjectToken selfThing) {

            if (this.Speaker != null) {
                selfThing.Reference = this.Speaker.Uid;
                selfThing.ReferenceProperty = this.GetPropertyInfo<Player>("Uid");
            }

            return selfThing;
        }

        /// <summary>
        /// The NLP library does not know what types we have, so it just passes a generic property name.
        /// 
        /// This could be a problem if property names clash, but then players would need to say "player name" and "map name"
        /// or we would have no reference to the type they want the property of.
        /// </summary>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        public override PropertyInfo GetPropertyInfo(string propertyName) {
            return this.GetPropertyInfo<Player>(propertyName) ?? this.GetPropertyInfo<Map>(propertyName);
        }

        /// <summary>
        /// Fetches a property, caching it in an internal dictionary so it can be fetched 
        /// without using reflection next time.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        private PropertyInfo GetPropertyInfo<T>(String propertyName) {
            String key = typeof(T) + propertyName;

            if (this.PropertyInfoCache.ContainsKey(key) == false) {
                this.PropertyInfoCache[key] = typeof(T).GetProperty(propertyName);
            }

            return this.PropertyInfoCache[key];
        }

        #endregion

        #region NLP

        protected List<ThingObjectToken> GetThings(Sentence sentence, PropertyInfo referenceProperty) {
            return sentence.Where(phrase => phrase.Count > 0).Where(phrase => phrase[0] is ThingObjectToken).Where(phrase => ((ThingObjectToken) phrase[0]).ReferenceProperty == referenceProperty).Select(phrase => (ThingObjectToken) phrase[0]).ToList();
        }

        protected ExpressionBuilder<T> BuildThingExpression<T>(PropertyInfo referenceProperty, ThingObjectToken thing) {

            ExpressionBuilder<T> expressions = new ExpressionBuilder<T>() {
                Parameter = this.LinqParameterMappings[typeof(T)].Parameter
            };

            SetsThingObjectToken token = thing as SetsThingObjectToken;

            if (token != null) {

                Sentence newSentence = new Sentence();
                newSentence.AddRange(token.Things.Select(innerThing => new Phrase() {
                    innerThing
                }));

                ExpressionType joiner = ExpressionType.OrElse;

                if (token.ExpressionType == ExpressionType.NotEqual) {
                    joiner = ExpressionType.AndAlso;
                }

                expressions.AddRange(this.ExecuteThingObjects<T>(newSentence, referenceProperty, joiner));
            }
            else {
                if (thing.ReferenceProperty != null && thing.ReferenceProperty.ReflectedType == typeof(T)) {
                    if (thing.ReferenceProperty.PropertyType == typeof(string)) {
                        expressions.AddExpression(Expression.MakeBinary(
                            thing.ExpressionType,
                            Expression.MakeMemberAccess(expressions.Parameter, thing.ReferenceProperty),
                            Expression.Constant(thing.Reference)));
                    }
                    /*
                    // This code was used when we had unsigned integers in a database, but
                    // since everything is now strings it'll never be hit.
                    else if (thing.ReferenceProperty.PropertyType == typeof(uint)) {
                        expressions.AddExpression(Expression.MakeBinary(
                            thing.ExpressionType,
                            Expression.MakeMemberAccess(expressions.Parameter, thing.ReferenceProperty),
                            Expression.Constant((uint)thing.Reference)));
                    }
                    */
                }
            }

            return expressions;
        }

        protected ExpressionBuilder<T> ExecuteThingObjects<T>(Sentence sentence, PropertyInfo referenceProperty, ExpressionType joiner) {
            ExpressionBuilder<T> expressions = new ExpressionBuilder<T>() {
                Parameter = this.LinqParameterMappings[typeof(T)].Parameter
            };

            foreach (ThingObjectToken thing in this.GetThings(sentence, referenceProperty)) {
                expressions.AddRange(this.BuildThingExpression<T>(referenceProperty, thing));
            }

            expressions.Combine(joiner);

            return expressions;
        }

        protected Sentence ExecuteThingObjects<T>(Sentence sentence, ExpressionBuilder<T> expressions) {

            expressions.AddRange(this.ExecuteThingObjects<T>(sentence, this.GetPropertyInfo<T>("Uid"), ExpressionType.OrElse));
            expressions.AddRange(this.ExecuteThingObjects<T>(sentence, this.GetPropertyInfo<T>("CountryName"), ExpressionType.OrElse));
            expressions.AddRange(this.ExecuteThingObjects<T>(sentence, this.GetPropertyInfo<T>("Name"), ExpressionType.OrElse));
            expressions.AddRange(this.ExecuteThingObjects<T>(sentence, this.GetPropertyInfo<T>("FriendlyName"), ExpressionType.OrElse));
            // expressions.AddRange(this.ExecuteThingObjects<T>(sentence, this.GetPropertyInfo("CountryName"), ExpressionType.OrElse));

            expressions.Combine(ExpressionType.AndAlso);

            return sentence;
        }

        protected Sentence ExecuteReducedObjects<T>(Sentence sentence, ExpressionBuilder<T> expressions) {
            for (int offset = 0; offset < sentence.Count; offset++) {
                ReductionToken token = sentence[offset].FirstOrDefault() as ReductionToken;

                if (token != null) {
                    // If the parameter applies to everything..
                    if (token.Parameter == null) {
                        expressions.AddExpression(token.LinqExpression);
                    }
                    else if (token.Parameter == expressions.Parameter) {
                        expressions.AddExpression(token.LinqExpression);

                        sentence.RemoveAt(offset);
                        offset--;
                    }
                }
            }

            return sentence;
        }

        private List<T> Extract<T>(Func<T, bool> predicate) where T : NetworkObject, new() {
            return this.LinqParameterMappings[typeof (T)].FetchCollection<T>().Where(item => predicate(item) == true).ToList();
        }

        private List<T> ExecuteBinaryExpressions<T>(Sentence sentence) where T : NetworkObject, new() {

            List<T> result = null;
            ExpressionBuilder<T> expressions = new ExpressionBuilder<T>() {
                Parameter = this.LinqParameterMappings[typeof(T)].Parameter
            };

            sentence = this.ExecuteThingObjects(sentence, expressions);
            sentence = this.ExecuteReducedObjects(sentence, expressions);

            expressions.Combine(ExpressionType.AndAlso);

            if (expressions.Count > 0) {
                result = this.Extract(expressions[0].Compile());
            }

            return result;
        }

        private List<TextCommand> ExtractCommandList(Sentence sentence) {

            // We need to know this method ahead of time so we can clear all other tokens in this phrase.
            MethodObjectToken mainMethod = sentence.Extract<MethodObjectToken>();
            List<MethodObjectToken> resultMethodList = new List<MethodObjectToken>();

            foreach (Phrase phrase in sentence) {

                if (phrase.Count > 0 && phrase[0] == mainMethod) {
                    // Only bubble up very close matching arguments.
                    phrase.RemoveAll(x => x.Similarity < 80);
                }

                // Select them as good alternatives 
                resultMethodList.AddRange(phrase.OfType<MethodObjectToken>().ToList());

                // Then remove them for the remainder of the execution.
                phrase.RemoveAll(x => x is MethodObjectToken);
            }

            resultMethodList = resultMethodList.OrderByDescending(x => x.Similarity)
                                               .ThenByDescending(x => x.Text.Length)
                                               .ToList();

            List<TextCommand> results = resultMethodList.Select(method => this.TextCommands.Find(x => x.PluginCommand == method.MethodName)).Where(command => command != null).ToList();

            return results.OrderByDescending(x => x.Priority).ToList();
        }

        #endregion

        public override CommandResultArgs BuildEvent(string prefix, string text, GenericEventType eventType) {
            Sentence sentence = new Sentence().Parse(this, text).Reduce(this);

            CommandResultArgs commandResult = null;
            
            List<TextCommand> commands = this.ExtractCommandList(sentence);
            TextCommand priorityCommand = commands.FirstOrDefault();

            List<String> quotes = sentence.Where(x => x.Count > 0 && x[0] is StringPrimitiveToken).Select(x => x[0].Text).ToList();

            List<TemporalToken> timeTokens = sentence.ExtractList<TemporalToken>();
            DateTime? delay = timeTokens.Where(x => x.Pattern != null && x.Pattern.Modifier == TimeModifier.Delay)
                                        .Select(x => x.Pattern.ToDateTime())
                                        .FirstOrDefault();

            FuzzyDateTimePattern interval = timeTokens.Where(x => x.Pattern != null && x.Pattern.Modifier == TimeModifier.Interval)
                                           .Select(x => x.Pattern)
                                           .FirstOrDefault();

            TimeSpan? period = timeTokens.Where(x => x.Pattern != null && (x.Pattern.Modifier == TimeModifier.Period || x.Pattern.Modifier == TimeModifier.None))
                                         .Select(x => x.Pattern.ToTimeSpan())
                                         .FirstOrDefault();
            
            // Must have a method to execute on, the rest is optional.
            if (priorityCommand != null) {
                commands.Remove(priorityCommand);

                commandResult = new CommandResultArgs() {
                    Success = true,
                    Status = CommandResultType.Success,
                    Now = new CommandData() {
                        Players = new List<Player>() {
                            this.Speaker
                        },
                        TextCommands = new List<TextCommand>() {
                            priorityCommand
                        }.Concat(commands).ToList(),
                        TextCommandMatches = new List<TextCommandMatch>() {
                            new TextCommandMatch() {
                                Prefix = prefix,
                                Players = this.ExecuteBinaryExpressions<Player>(sentence),
                                Maps = this.ExecuteBinaryExpressions<Map>(sentence),
                                Numeric = sentence.ExtractList<FloatNumericPrimitiveToken>().Select(x => x.ToFloat()).ToList(),
                                Delay = delay,
                                Period = period,
                                Interval = interval,
                                Text = text,
                                Quotes = quotes
                            }
                        }
                    }
                };
            }

            return commandResult;
        }

    }
}
