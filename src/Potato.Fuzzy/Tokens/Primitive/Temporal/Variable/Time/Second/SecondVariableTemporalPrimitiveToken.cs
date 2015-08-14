#region Copyright
// Copyright 2015 Geoff Green.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
using System;
using System.Collections.Generic;
using Potato.Fuzzy.Tokens.Primitive.Temporal.Units;
using Potato.Fuzzy.Tokens.Primitive.Numeric;
using Potato.Fuzzy.Tokens.Syntax.Adjectives;
using Potato.Fuzzy.Tokens.Syntax.Articles;
using Potato.Fuzzy.Utils;

namespace Potato.Fuzzy.Tokens.Primitive.Temporal.Variable.Time.Second {

    public class SecondVariableTemporalPrimitiveToken : TimeVariableTemporalPrimitiveToken {
        public static Phrase ReduceNumberSeconds(IFuzzyState state, Dictionary<string, Token> parameters) {
            var number = (FloatNumericPrimitiveToken) parameters["number"];
            var seconds = (SecondsUnitTemporalPrimitiveToken) parameters["seconds"];

            return new Phrase() {
                new SecondVariableTemporalPrimitiveToken() {
                    Pattern = new FuzzyDateTimePattern() {
                        Rule = TimeType.Relative,
                        Second = (int) number.ToFloat().ConvertTo(typeof (int))
                    },
                    Text = string.Format("{0} {1}", number.Text, seconds.Text),
                    Similarity = (seconds.Similarity + number.Similarity) / 2.0F
                }
            };
        }

        public static Phrase ReduceArticleSeconds(IFuzzyState state, Dictionary<string, Token> parameters) {
            var article = (IndefiniteArticlesSyntaxToken) parameters["article"];
            var seconds = (SecondsUnitTemporalPrimitiveToken) parameters["seconds"];

            return new Phrase() {
                new SecondVariableTemporalPrimitiveToken() {
                    Pattern = new FuzzyDateTimePattern() {
                        Rule = TimeType.Relative,
                        Second = 1
                    },
                    Text = string.Format("{0} {1}", article.Text, seconds.Text),
                    Similarity = (seconds.Similarity + article.Similarity) / 2.0F
                }
            };
        }

        public static Phrase ReduceEverySeconds(IFuzzyState state, Dictionary<string, Token> parameters) {
            var every = (EveryAdjectiveSyntaxToken) parameters["every"];
            var seconds = (SecondsUnitTemporalPrimitiveToken) parameters["seconds"];

            return new Phrase() {
                new SecondsUnitTemporalPrimitiveToken() {
                    Pattern = new FuzzyDateTimePattern() {
                        Rule = TimeType.Relative,
                        Modifier = TimeModifier.Interval,
                        Second = 1
                    },
                    Text = string.Format("{0} {1}", every.Text, seconds.Text),
                    Similarity = (seconds.Similarity + every.Similarity) / 2.0F
                }
            };
        }
    }
}