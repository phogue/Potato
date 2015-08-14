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

namespace Potato.Fuzzy.Tokens.Primitive.Temporal.Variable.Months {
    using Syntax.Adjectives;

    public class MonthMonthsVariableTemporalPrimitiveToken : DateTimeTemporalPrimitiveToken {
        public static Phrase ReduceAdjectiveMonths(IFuzzyState state, Dictionary<string, Token> parameters) {
            var adjective = (AdjectiveSyntaxToken) parameters["adjective"];
            var months = (MonthMonthsVariableTemporalPrimitiveToken) parameters["months"];

            var newDateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

            if (adjective is LastAdjectiveSyntaxToken) {
                // Today - Maximum of 7
                do {
                    newDateTime = newDateTime.AddMonths(-1);
                } while (newDateTime.Month != months.Pattern.Month);
            }
            else if (adjective is NextAdjectiveSyntaxToken) {
                // Today + Maximum of 7
                do {
                    newDateTime = newDateTime.AddMonths(1);
                } while (newDateTime.Month != months.Pattern.Month);
            }
            else if (adjective is ThisAdjectiveSyntaxToken) {
                // Today or +7
                while (newDateTime.Month != months.Pattern.Month) {
                    newDateTime = newDateTime.AddMonths(1);
                }
            }

            return new Phrase() {
                new MonthMonthsVariableTemporalPrimitiveToken() {
                    Pattern = new FuzzyDateTimePattern() {
                        Year = newDateTime.Year,
                        Month = newDateTime.Month
                    },
                    Text = string.Format("{0} {1}", adjective.Text, months.Text),
                    Similarity = (months.Similarity + adjective.Similarity) / 2.0F
                }
            };
        }
    }
}