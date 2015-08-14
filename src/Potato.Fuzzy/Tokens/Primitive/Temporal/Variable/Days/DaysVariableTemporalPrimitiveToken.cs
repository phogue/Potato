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

namespace Potato.Fuzzy.Tokens.Primitive.Temporal.Variable.Days {
    using Syntax.Adjectives;

    public class DaysVariableTemporalPrimitiveToken : DateTimeTemporalPrimitiveToken {
        public static Phrase ReduceAdjectiveDays(IFuzzyState state, Dictionary<string, Token> parameters) {
            var adjective = (AdjectiveSyntaxToken) parameters["adjective"];
            var days = (DaysVariableTemporalPrimitiveToken) parameters["days"];


            var newDateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

            if (adjective is LastAdjectiveSyntaxToken) {
                // Today - Maximum of 7
                do {
                    newDateTime = newDateTime.AddDays(-1);
                } while (newDateTime.DayOfWeek != days.Pattern.DayOfWeek);
            }
            else if (adjective is NextAdjectiveSyntaxToken) {
                // Today + Maximum of 7
                do {
                    newDateTime = newDateTime.AddDays(1);
                } while (newDateTime.DayOfWeek != days.Pattern.DayOfWeek);
            }
            else if (adjective is ThisAdjectiveSyntaxToken) {
                // Today or +7
                while (newDateTime.DayOfWeek != days.Pattern.DayOfWeek) {
                    newDateTime = newDateTime.AddDays(1);
                }
            }

            return new Phrase() {
                new DaysVariableTemporalPrimitiveToken() {
                    Pattern = new FuzzyDateTimePattern() {
                        Rule = TimeType.Definitive,
                        Year = newDateTime.Year,
                        Month = newDateTime.Month,
                        Day = newDateTime.Day
                    },
                    Text = string.Format("{0} {1}", adjective.Text, days.Text),
                    Similarity = (days.Similarity + adjective.Similarity) / 2.0F
                }
            };
        }
    }
}