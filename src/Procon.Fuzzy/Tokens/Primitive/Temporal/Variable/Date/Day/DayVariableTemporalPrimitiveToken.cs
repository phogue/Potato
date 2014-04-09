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
using Procon.Fuzzy.Tokens.Primitive.Numeric;
using Procon.Fuzzy.Tokens.Primitive.Temporal.Units;
using Procon.Fuzzy.Tokens.Syntax.Articles;
using Procon.Fuzzy.Utils;

namespace Procon.Fuzzy.Tokens.Primitive.Temporal.Variable.Date.Day {

    public class DayVariableTemporalPrimitiveToken : DateVariableTemporalPrimitiveToken {
        public static Phrase ReduceNumberDays(IFuzzyState state, Dictionary<String, Token> parameters) {
            FloatNumericPrimitiveToken number = (FloatNumericPrimitiveToken) parameters["number"];
            DaysUnitTemporalPrimitiveToken days = (DaysUnitTemporalPrimitiveToken) parameters["days"];

            return new Phrase() {
                new DayVariableTemporalPrimitiveToken() {
                    Pattern = new FuzzyDateTimePattern() {
                        Rule = TimeType.Relative,
                        Day = (int) number.ToFloat().ConvertTo(typeof (int))
                    },
                    Text = String.Format("{0} {1}", number.Text, days.Text),
                    Similarity = (days.Similarity + number.Similarity) / 2.0F
                }
            };
        }

        public static Phrase ReduceNumberWeeks(IFuzzyState state, Dictionary<String, Token> parameters) {
            FloatNumericPrimitiveToken number = (FloatNumericPrimitiveToken) parameters["number"];
            WeeksUnitTemporalPrimitiveToken weeks = (WeeksUnitTemporalPrimitiveToken) parameters["weeks"];

            return new Phrase() {
                new DayVariableTemporalPrimitiveToken() {
                    Pattern = new FuzzyDateTimePattern() {
                        Rule = TimeType.Relative,
                        Day = (int) number.ToFloat().ConvertTo(typeof (int)) * 7
                    },
                    Text = String.Format("{0} {1}", number.Text, weeks.Text),
                    Similarity = (weeks.Similarity + number.Similarity) / 2.0F
                }
            };
        }

        public static Phrase ReduceArticleDays(IFuzzyState state, Dictionary<String, Token> parameters) {
            IndefiniteArticlesSyntaxToken article = (IndefiniteArticlesSyntaxToken) parameters["article"];
            DaysUnitTemporalPrimitiveToken days = (DaysUnitTemporalPrimitiveToken) parameters["days"];

            return new Phrase() {
                new DayVariableTemporalPrimitiveToken() {
                    Pattern = new FuzzyDateTimePattern() {
                        Rule = TimeType.Relative,
                        Day = 1
                    },
                    Text = String.Format("{0} {1}", article.Text, days.Text),
                    Similarity = (days.Similarity + article.Similarity) / 2.0F
                }
            };
        }

        public static Phrase ReduceArticleWeeks(IFuzzyState state, Dictionary<String, Token> parameters) {
            IndefiniteArticlesSyntaxToken article = (IndefiniteArticlesSyntaxToken) parameters["article"];
            WeeksUnitTemporalPrimitiveToken weeks = (WeeksUnitTemporalPrimitiveToken) parameters["weeks"];

            return new Phrase() {
                new DayVariableTemporalPrimitiveToken() {
                    Pattern = new FuzzyDateTimePattern() {
                        Rule = TimeType.Relative,
                        Day = 7
                    },
                    Text = String.Format("{0} {1}", article.Text, weeks.Text),
                    Similarity = (weeks.Similarity + article.Similarity) / 2.0F
                }
            };
        }
    }
}