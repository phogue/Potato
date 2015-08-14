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

namespace Potato.Fuzzy.Tokens.Primitive.Temporal.Variable.Time.Minute {
    using Potato.Fuzzy.Tokens.Primitive.Temporal.Units;
    using Potato.Fuzzy.Tokens.Primitive.Numeric;
    using Potato.Fuzzy.Tokens.Syntax.Adjectives;
    using Potato.Fuzzy.Tokens.Syntax.Articles;
    using Potato.Fuzzy.Utils;

    public class MinuteVariableTemporalPrimitiveToken : TimeVariableTemporalPrimitiveToken {
        public static Phrase ReduceNumberMinutes(IFuzzyState state, Dictionary<string, Token> parameters) {
            var number = (FloatNumericPrimitiveToken) parameters["number"];
            var minutes = (MinutesUnitTemporalPrimitiveToken) parameters["minutes"];

            return new Phrase() {
                new MinuteVariableTemporalPrimitiveToken() {
                    Pattern = new FuzzyDateTimePattern() {
                        Rule = TimeType.Relative,
                        Minute = (int) number.ToFloat().ConvertTo(typeof (int))
                    },
                    Text = string.Format("{0} {1}", number.Text, minutes.Text),
                    Similarity = (minutes.Similarity + number.Similarity) / 2.0F
                }
            };
        }

        public static Phrase ReduceArticleMinutes(IFuzzyState state, Dictionary<string, Token> parameters) {
            var article = (IndefiniteArticlesSyntaxToken) parameters["article"];
            var minutes = (MinutesUnitTemporalPrimitiveToken) parameters["minutes"];

            return new Phrase() {
                new MinuteVariableTemporalPrimitiveToken() {
                    Pattern = new FuzzyDateTimePattern() {
                        Rule = TimeType.Relative,
                        Minute = 1
                    },
                    Text = string.Format("{0} {1}", article.Text, minutes.Text),
                    Similarity = (minutes.Similarity + article.Similarity) / 2.0F
                }
            };
        }

        public static Phrase ReduceEveryMinutes(IFuzzyState state, Dictionary<string, Token> parameters) {
            var every = (EveryAdjectiveSyntaxToken) parameters["every"];
            var minutes = (MinutesUnitTemporalPrimitiveToken) parameters["minutes"];

            return new Phrase() {
                new MinutesUnitTemporalPrimitiveToken() {
                    Pattern = new FuzzyDateTimePattern() {
                        Rule = TimeType.Relative,
                        Modifier = TimeModifier.Interval,
                        Minute = 1
                    },
                    Text = string.Format("{0} {1}", every.Text, minutes.Text),
                    Similarity = (minutes.Similarity + every.Similarity) / 2.0F
                }
            };
        }
    }
}