// Copyright 2011 Geoffrey 'Phogue' Green
// 
// http://www.phogue.net
//  
// This file is part of Procon 2.
// 
// Procon 2 is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// Procon 2 is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Procon 2.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Xml.Linq;
using System.Text;
using System.Reflection;

namespace Procon.Core.Interfaces.Connections.Text {
    using Procon.NLP;
    using Procon.NLP.Utils;
    using Procon.NLP.Tokens.Object;
    using Procon.NLP.Tokens.Object.Sets;
    using Procon.NLP.Tokens.Primitive;
    using Procon.NLP.Tokens.Primitive.Numeric;
    using Procon.NLP.Tokens.Primitive.Temporal;
    using Procon.NLP.Tokens.Reduction;
    using Procon.Core.Localization;
    using Procon.Core.Interfaces.Variables;
    using Procon.Core.Interfaces.Connections;
    using Procon.Core.Interfaces.Security.Objects;
    using Procon.Net;
    using Procon.Net.Protocols;
    using Procon.Net.Protocols.Objects;

    public class LocalTextCommandController : TextCommandController, IStateNLP {

        // Elsewhere.
        public XElement Document { get; set; }

        /// <summary>
        /// All linq commands require the exact same object in order to compile
        /// </summary>
        //public ParameterExpression LinqParameterExpression { get; set; }

        public Dictionary<Type, ParameterExpression> LinqParameterExpressions { get; set; }

        //public GameState GameState { get; set; }

        public Player Speaker { get; protected set; }
        public Account SpeakerAccount { get; protected set; }

        public LanguageController Languages { get; set; }

        public LocalTextCommandController() {
            this.LinqParameterExpressions = new Dictionary<Type, ParameterExpression>() {
                { typeof(Player), Expression.Parameter(typeof(Player), "x") },
                { typeof(Map), Expression.Parameter(typeof(Map), "m") }
            };

            //this.LinqParameterExpression = Expression.Parameter(typeof(Player), "x");
        }

        #region Parsing

        protected void ParseMapNames(Phrase phrase) {

            PropertyInfo aliasId = this.GetPropertyInfo<Map>("Name");

            var mapNames = from map in this.Connection.GameState.MapPool
                              let similarity = Math.Max(map.FriendlyName.DePluralLevenshtein(phrase.Text), map.Name.DePluralLevenshtein(phrase.Text))
                              where similarity >= 60
                              select new ThingObjectToken() {
                                  Reference = map.Name,
                                  ReferenceProperty = aliasId,
                                  Text = phrase.Text,
                                  Similarity = similarity
                              };

            List<Token> names = new List<Token>();
            mapNames.ToList().ForEach(x => names.Add(x));

            phrase.AddDistinctRange(names);
        }

        protected void ParsePlayerNames(Phrase phrase) {
            
            PropertyInfo aliasId = this.GetPropertyInfo<Player>("UID");

            var playerNames = from player in this.Connection.GameState.PlayerList
                              let similarity = Math.Max(player.NameStripped.DePluralLevenshtein(phrase.Text), player.Name.DePluralLevenshtein(phrase.Text))
                              where similarity >= 60
                              select new ThingObjectToken() {
                                  Reference = player.UID,
                                  ReferenceProperty = aliasId,
                                  Text = phrase.Text,
                                  Similarity = similarity
                              };

            List<Token> names = new List<Token>();
            playerNames.ToList().ForEach(x => names.Add(x));

            phrase.AddDistinctRange(names);
        }

        protected void ParseCountryNames(Phrase phrase) {
            PropertyInfo countryName = this.GetPropertyInfo<Player>("CountryName");

            var playerCountries = from player in this.Connection.GameState.PlayerList
                                  let similarity = player.CountryName.LevenshteinSubsetBonusRatio(phrase.Text)
                                  where similarity >= 60
                                  select new ThingObjectToken() {
                                      Reference = player.CountryName,
                                      ReferenceProperty = countryName,
                                      Text = phrase.Text,
                                      Similarity = similarity
                                  };

            List<Token> names = new List<Token>();
            playerCountries.ToList().ForEach(x => names.Add(x));

            phrase.AddDistinctRange(names);

        }

        public Phrase ParseThing(IStateNLP state, Phrase phrase) {

            this.ParsePlayerNames(phrase);
            this.ParseMapNames(phrase);
            this.ParseCountryNames(phrase);
            //this.ParseCountryNames(phrase);
            //this.ParseRegionNames(phrase);
            //this.ParseCityNames(phrase);
            //this.ParseItemNames(phrase);

            return phrase;
        }

        private float MaximumLevenshtein(string argument, List<string> commands) {

            float max = 0.0F;

            commands.ForEach(x=> max = Math.Max(max, x.LevenshteinSubsetBonusRatio(argument)));

            return max;
        }

        protected void ParseMethods(Phrase phrase) {

            var methods = from textCommand in this.TextCommands
                          let similarity = this.MaximumLevenshtein(phrase.Text, textCommand.Commands)
                          where similarity >= 60
                          select new MethodObjectToken() {
                              MethodName = textCommand.MethodCallback,
                              Text = phrase.Text,
                              Similarity = similarity
                          };

            List<Token> names = new List<Token>();
            methods.ToList().ForEach(x => names.Add(x));
            phrase.AddDistinctRange(names);

            /*
            PropertyInfo aliasId = this.GetPropertyInfo("UID");

            var playerNames = from player in this.GameState.PlayerList
                              let similarity = Math.Max(player.NameStripped.DePluralLevenshtein(phrase.Text), player.Name.DePluralLevenshtein(phrase.Text))
                              where similarity >= 60
                              select new ThingObjectToken() {
                                  Reference = player.Uid,
                                  ReferenceProperty = aliasId,
                                  Text = phrase.Text,
                                  Similarity = similarity
                              };

            List<Token> names = new List<Token>();
            playerNames.ToList().ForEach(x => names.Add(x));
            */
            //phrase.AddDistinctRange(names);
        }


        public Phrase ParseMethod(IStateNLP state, Phrase phrase) {

            this.ParseMethods(phrase);

            return phrase;
        }

        public SelfReflectionThingObjectToken ParseSelfReflectionThing(IStateNLP state, SelfReflectionThingObjectToken selfThing) {
            
            selfThing.Reference = this.Speaker.UID;
            selfThing.ReferenceProperty = this.GetPropertyInfo<Player>("UID");

            return selfThing;
        }

        public PropertyInfo GetPropertyInfo(string propertyName) {
            return this.GetPropertyInfo<Player>(propertyName);
        }

        private PropertyInfo GetPropertyInfo<T>(string propertyName) {
            PropertyInfo returnInfo = null;

            returnInfo = (returnInfo == null ? typeof(T).GetProperty(propertyName) : returnInfo);

            return returnInfo;
        }

        #endregion

        #region NLP

        protected List<ThingObjectToken> GetThings(Sentence sentence, PropertyInfo referenceProperty) {

            List<ThingObjectToken> returnList = new List<ThingObjectToken>();

            foreach (Phrase phrase in sentence) {
                if (phrase.Count > 0 && phrase[0] is ThingObjectToken && ((ThingObjectToken)phrase[0]).ReferenceProperty == referenceProperty) {

                    returnList.Add((ThingObjectToken)phrase[0]);
                }
            }

            return returnList;
        }

        protected ExpressionBuilder<T> BuildThingExpression<T>(PropertyInfo referenceProperty, ThingObjectToken thing) {

            ExpressionBuilder<T> expressions = new ExpressionBuilder<T>() {
                Parameter = this.LinqParameterExpressions[typeof(T)]
            };

            if (thing is SetsThingObjectToken) {

                Sentence newSentence = new Sentence();

                foreach (ThingObjectToken innerThing in ((SetsThingObjectToken)thing).Things) {
                    newSentence.Add(new Phrase() { innerThing });
                }

                ExpressionType joiner = ExpressionType.OrElse;

                if (thing.ExpressionType == ExpressionType.NotEqual) {
                    joiner = ExpressionType.AndAlso;
                }

                expressions.AddRange(this.ExecuteThingObjects<T>(newSentence, referenceProperty, joiner));
            }
            else {
                if (thing.ReferenceProperty.ReflectedType == typeof(T)) {
                    if (thing.ReferenceProperty.PropertyType == typeof(string)) {
                        expressions.AddExpression(Expression.MakeBinary(
                            thing.ExpressionType,
                            Expression.MakeMemberAccess(expressions.Parameter, thing.ReferenceProperty),
                            Expression.Constant(thing.Reference)));
                    }
                    else if (thing.ReferenceProperty.PropertyType == typeof(uint)) {
                        expressions.AddExpression(Expression.MakeBinary(
                            thing.ExpressionType,
                            Expression.MakeMemberAccess(expressions.Parameter, thing.ReferenceProperty),
                            Expression.Constant((uint)thing.Reference)));
                    }
                }
            }

            return expressions;
        }

        protected ExpressionBuilder<T> ExecuteThingObjects<T>(Sentence sentence, PropertyInfo referenceProperty, ExpressionType joiner) {
            ExpressionBuilder<T> expressions = new ExpressionBuilder<T>() {
                Parameter = this.LinqParameterExpressions[typeof(T)]
            };

            foreach (ThingObjectToken thing in this.GetThings(sentence, referenceProperty)) {
                expressions.AddRange(this.BuildThingExpression<T>(referenceProperty, thing));
            }

            expressions.Combine(joiner);

            return expressions;
        }

        protected Sentence ExecuteThingObjects<T>(Sentence sentence, ExpressionBuilder<T> expressions) {

            expressions.AddRange(this.ExecuteThingObjects<T>(sentence, this.GetPropertyInfo<T>("UID"), ExpressionType.OrElse));
            expressions.AddRange(this.ExecuteThingObjects<T>(sentence, this.GetPropertyInfo<T>("CountryName"), ExpressionType.OrElse));
            expressions.AddRange(this.ExecuteThingObjects<T>(sentence, this.GetPropertyInfo<T>("Name"), ExpressionType.OrElse));
            expressions.AddRange(this.ExecuteThingObjects<T>(sentence, this.GetPropertyInfo<T>("FriendlyName"), ExpressionType.OrElse));
            // expressions.AddRange(this.ExecuteThingObjects<T>(sentence, this.GetPropertyInfo("CountryName"), ExpressionType.OrElse));

            expressions.Combine(ExpressionType.AndAlso);

            return sentence;
        }

        protected Sentence ExecuteReducedObjects<T>(Sentence sentence, ExpressionBuilder<T> expressions) {
            for (int offset = 0; offset < sentence.Count; offset++) {
                if (sentence[offset].FirstOrDefault() is ReductionToken) {
                    expressions.AddExpression(((ReductionToken)sentence[offset].FirstOrDefault()).LinqExpression);

                    sentence.RemoveAt(offset);
                    offset--;
                }
            }

            return sentence;
        }

        private List<T> Extract<T>(Func<T, bool> predicate) where T : ProtocolObject, new() {

            List<T> result = new List<T>();

            // I know this breaks the generic right here, but it's just a foot holder for now
            // as I convert the NLP over for use in procon.
            if (typeof(T) == typeof(Player)) {
                foreach (Player player in this.Connection.GameState.PlayerList) {
                    if (predicate.Invoke(player as T) == true) {
                        result.Add(player as T);
                    }
                }
            }
            else if (typeof(T) == typeof(Map)) {
                foreach (Map map in this.Connection.GameState.MapPool) {
                    if (predicate.Invoke(map as T) == true) {
                        result.Add(map as T);
                    }
                }
            }

            return result;
        }

        private List<T> ExecuteBinaryExpressions<T>(Sentence sentence) where T : ProtocolObject, new() {

            List<T> result = null;
            ExpressionBuilder<T> expressions = new ExpressionBuilder<T>() {
                Parameter = this.LinqParameterExpressions[typeof(T)]
            };

            sentence = this.ExecuteThingObjects(sentence, expressions);
            sentence = this.ExecuteReducedObjects(sentence, expressions);

            expressions.Combine(ExpressionType.AndAlso);

            if (expressions.Count > 0) {
                result = this.Extract<T>(expressions[0].Compile());
            }

            return result;
        }

        private List<TextCommand> ExtractCommandList(Sentence sentence) {

            /// We need to know this method ahead of time so we can clear all other tokens in this phrase.
            MethodObjectToken mainMethod = sentence.Extract<MethodObjectToken>();
            List<MethodObjectToken> resultMethodList = new List<MethodObjectToken>();

            foreach (Phrase phrase in sentence) {

                if (phrase.Count > 0 && phrase[0] == mainMethod) {
                    // Only bubble up very close matching arguments.
                    phrase.RemoveAll(x => x.Similarity < 80);
                }

                // Select them as good alternatives 
                resultMethodList.AddRange(phrase.Where(x => x is MethodObjectToken).Select(x => x as MethodObjectToken).ToList());

                // Then remove them for the remainder of the execution.
                phrase.RemoveAll(x => x is MethodObjectToken);
            }

            resultMethodList = resultMethodList.OrderByDescending(x => x.Similarity)
                                               .ThenByDescending(x => x.Text.Length)
                                               .ToList();

            List<TextCommand> results = new List<TextCommand>();
            foreach (MethodObjectToken method in resultMethodList) {
                TextCommand command = this.TextCommands.Find(x => x.MethodCallback == method.MethodName);

                if (command != null) {
                    results.Add(command);
                }
            }

            return results.OrderByDescending(x => x.Priority).ToList();
        }

        private Sentence Execute(Sentence sentence, string prefix, string originalSentence) {

            List<TextCommand> commands = this.ExtractCommandList(sentence);
            TextCommand priorityCommand = commands.FirstOrDefault();

            FloatNumericPrimitiveToken numericToken = sentence.Extract<FloatNumericPrimitiveToken>();
            // TemporalToken timeToken = sentence.Extract<TemporalToken>();
            List<string> quotes = sentence.Where(x => x.Count > 0 && x[0] is StringPrimitiveToken).Select(x => x[0].Text).ToList();

            List<TemporalToken> timeTokens = sentence.ExtractList<TemporalToken>();
            DateTime? delay = timeTokens.Where(x => x.Pattern != null && x.Pattern.Modifier == TimeModifier.Delay)
                                        .Select(x => x.Pattern.ToDateTime())
                                        .FirstOrDefault();

            DateTimePatternNLP interval = timeTokens.Where(x => x.Pattern != null && x.Pattern.Modifier == TimeModifier.Interval)
                                           .Select(x => x.Pattern)
                                           .FirstOrDefault();

            TimeSpan? period = timeTokens.Where(x => x.Pattern != null && (x.Pattern.Modifier == TimeModifier.Period || x.Pattern.Modifier == TimeModifier.None))
                                         .Select(x => x.Pattern.ToTimeSpan())
                                         .FirstOrDefault();

            // Must have a method to execute on, the rest is optional.
            if (priorityCommand != null) {
                commands.Remove(priorityCommand);

                this.ThrowTextCommandEvent(
                    new TextCommandEventArgs() {
                        EventType = TextCommandEventType.Matched,
                        Speaker = this.Speaker,
                        Command = priorityCommand,
                        AlternativeCommands = commands,
                        Match = new Match() {
                            Prefix = prefix,
                            Players = this.ExecuteBinaryExpressions<Player>(sentence),
                            Maps = this.ExecuteBinaryExpressions<Map>(sentence),
                            Numeric = sentence.ExtractList<FloatNumericPrimitiveToken>().Select(x => x.ToFloat()).ToList(),
                            Delay = delay,
                            Period = period,
                            Interval = interval,
                            Text = originalSentence,
                            Quotes = quotes
                        }
                    }
                );
            }

            return sentence;
        }

        #endregion

        #region Execution

        /// <summary>
        /// Checks if a prefix is an allowed prefix
        /// </summary>
        /// <param name="prefix">The prefix to check (e.g !, @ etc.)</param>
        /// <returns>The parameter prefix, or null if the prefix is invalid</returns>
        private string GetValidPrefix(string prefix) {

            string result = null;

            if (prefix == this.Connection.Variables.Get<String>(CommandInitiator.Local, CommonVariableNames.TextCommandPublicPrefix) ||
                prefix == this.Connection.Variables.Get<String>(CommandInitiator.Local, CommonVariableNames.TextCommandProtectedPrefix) ||
                prefix == this.Connection.Variables.Get<String>(CommandInitiator.Local, CommonVariableNames.TextCommandPrivatePrefix)) {
                result = prefix;
            }

            return result;
        }

        protected override void AssignEvents() {
            this.Connection.GameEvent += new Game.GameEventHandler(Connection_GameEvent);
        }

        private void Connection_GameEvent(Game sender, GameEventArgs e) {
            if (e.EventType == GameEventType.Chat) {
                if (e.Chat.Text.Length > 0) {

                    String prefix = e.Chat.Text.First().ToString();
                    String text = e.Chat.Text.Remove(0, 1);

                    if ((prefix = GetValidPrefix(prefix)) != null) {
                        this.Execute(
                            e.Chat.Author,
                            this.Connection.Security.Account(this.Connection.GameType, e.Chat.Author.UID),
                            prefix,
                            text
                        );
                    }
                }
            }
        }

        /// <summary>
        /// This method is only ever called internally as a result of a call from
        /// a game event firing, i.e chat within a game.
        /// </summary>
        /// <param name="speaker"></param>
        /// <param name="speakerAccount"></param>
        /// <param name="prefix"></param>
        /// <param name="?"></param>
        /// <returns></returns>
        protected Sentence Execute(Player speaker, Account speakerAccount, String prefix, String text) {
            Sentence result = null;
            
            this.Speaker = speaker;
            this.SpeakerAccount = speakerAccount;

            Language selectedLanguage = null;
            if (this.SpeakerAccount != null && this.SpeakerAccount.PreferredLanguageCode != String.Empty) {
                selectedLanguage = this.Languages.Languages.Find(x => x.LanguageCode == this.SpeakerAccount.PreferredLanguageCode);
            }
            else {
                selectedLanguage = this.Languages.Default;
            }

            if (selectedLanguage != null) {
                this.Document = selectedLanguage.Root;

                result = new Sentence().Parse(this, text).Reduce(this);
                this.Execute(result, prefix, text);
            }

            return result;
        }

        #endregion
    }
}
