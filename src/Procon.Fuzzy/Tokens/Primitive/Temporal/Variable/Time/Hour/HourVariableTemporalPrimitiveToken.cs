#region Copyright
// Copyright 2014 Myrcon Pty. Ltd.
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
using Procon.Fuzzy.Tokens.Primitive.Temporal.Units;
using Procon.Fuzzy.Tokens.Primitive.Temporal.Units.Meridiem;
using Procon.Fuzzy.Tokens.Primitive.Numeric;
using Procon.Fuzzy.Tokens.Syntax.Adjectives;
using Procon.Fuzzy.Tokens.Syntax.Articles;
using Procon.Fuzzy.Utils;

namespace Procon.Fuzzy.Tokens.Primitive.Temporal.Variable.Time.Hour {

    public class HourVariableTemporalPrimitiveToken : TimeVariableTemporalPrimitiveToken {
        public static Phrase ReduceNumberHours(IFuzzyState state, Dictionary<String, Token> parameters) {
            FloatNumericPrimitiveToken number = (FloatNumericPrimitiveToken) parameters["number"];
            HoursUnitTemporalPrimitiveToken hours = (HoursUnitTemporalPrimitiveToken) parameters["hours"];

            return new Phrase() {
                new HourVariableTemporalPrimitiveToken() {
                    Pattern = new FuzzyDateTimePattern() {
                        Rule = TimeType.Relative,
                        Hour = (int) number.ToFloat().ConvertTo(typeof (int))
                    },
                    Text = String.Format("{0} {1}", number.Text, hours.Text),
                    Similarity = (hours.Similarity + number.Similarity) / 2.0F
                }
            };
        }

        public static Phrase ReduceNumberMeridiem(IFuzzyState state, Dictionary<String, Token> parameters) {
            FloatNumericPrimitiveToken number = (FloatNumericPrimitiveToken) parameters["number"];
            MeridiemUnitsTemporalPrimitiveToken meridiem = (MeridiemUnitsTemporalPrimitiveToken) parameters["meridiem"];

            int hours = (int) number.ToFloat().ConvertTo(typeof (int)) % 24;
            if (meridiem is PostMeridiemUnitsTemporalPrimitiveToken) {
                hours += 12;
            }

            return new Phrase() {
                new HourVariableTemporalPrimitiveToken() {
                    Pattern = new FuzzyDateTimePattern() {
                        Rule = TimeType.Definitive,
                        Hour = hours,
                        Minute = 0,
                        Second = 0
                    },
                    Text = String.Format("{0} {1}", number.Text, meridiem.Text),
                    Similarity = (meridiem.Similarity + number.Similarity) / 2.0F
                }
            };
        }

        public static Phrase ReduceArticleHours(IFuzzyState state, Dictionary<String, Token> parameters) {
            IndefiniteArticlesSyntaxToken article = (IndefiniteArticlesSyntaxToken) parameters["article"];
            HoursUnitTemporalPrimitiveToken hours = (HoursUnitTemporalPrimitiveToken) parameters["hours"];

            return new Phrase() {
                new HourVariableTemporalPrimitiveToken() {
                    Pattern = new FuzzyDateTimePattern() {
                        Rule = TimeType.Relative,
                        Hour = 1
                    },
                    Text = String.Format("{0} {1}", article.Text, hours.Text),
                    Similarity = (hours.Similarity + article.Similarity) / 2.0F
                }
            };
        }

        public static Phrase ReduceEveryHours(IFuzzyState state, Dictionary<String, Token> parameters) {
            EveryAdjectiveSyntaxToken every = (EveryAdjectiveSyntaxToken) parameters["every"];
            HoursUnitTemporalPrimitiveToken hours = (HoursUnitTemporalPrimitiveToken) parameters["hours"];

            return new Phrase() {
                new HourVariableTemporalPrimitiveToken() {
                    Pattern = new FuzzyDateTimePattern() {
                        Rule = TimeType.Relative,
                        Modifier = TimeModifier.Interval,
                        Hour = 1
                    },
                    Text = String.Format("{0} {1}", every.Text, hours.Text),
                    Similarity = (hours.Similarity + every.Similarity) / 2.0F
                }
            };
        }
    }
}