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
using System.Text.RegularExpressions;

namespace Potato.Fuzzy.Tokens.Primitive.Temporal.Variable.Time {
    using Operator.Logical;

    public class TimeVariableTemporalPrimitiveToken : DateTimeTemporalPrimitiveToken {
        // @todo should this be moved to the localization file, even if it is the same code here but referenced?
        protected static readonly Regex RegexMatch = new Regex(@"^(((?<hours>[0]?[1-9]|1[0-2])[ ]*(:|\.)[ ]*(?<minutes>[0-5][0-9])([ ]*(:|\.)[ ]*(?<seconds>[0-5][0-9]))?[ ]*(?<meridiem>AM|am|aM|Am|PM|pm|pM|Pm))|((?<hours>[0]?[0-9]|1[0-9]|2[0-3])[ ]*(:|\.)[ ]*(?<minutes>[0-5][0-9])([ ]*(:|\.)[ ]*(?<seconds>[0-5][0-9]))?))$", RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);

        public static Phrase Parse(IFuzzyState state, Phrase phrase) {
            var regexMatch = RegexMatch.Match(phrase.Text);

            if (regexMatch.Success == true) {
                int? hours = regexMatch.Groups["hours"].Value.Length > 0 ? int.Parse(regexMatch.Groups["hours"].Value) : 0;
                int? minutes = regexMatch.Groups["minutes"].Value.Length > 0 ? int.Parse(regexMatch.Groups["minutes"].Value) : 0;
                int? seconds = regexMatch.Groups["seconds"].Value.Length > 0 ? int.Parse(regexMatch.Groups["seconds"].Value) : 0;

                if (string.Compare(regexMatch.Groups["meridiem"].Value, "pm", StringComparison.OrdinalIgnoreCase) == 0 && hours < 12) {
                    hours += 12;
                }
                else if (string.Compare(regexMatch.Groups["meridiem"].Value, "am", StringComparison.OrdinalIgnoreCase) == 0) {
                    if (hours == 12) {
                        hours = 0;
                    }
                }

                hours %= 24;

                phrase.Add(new TimeVariableTemporalPrimitiveToken() {
                    Pattern = new FuzzyDateTimePattern() {
                        Rule = TimeType.Definitive,
                        Hour = (regexMatch.Groups["hours"].Value.Length > 0 ? hours : null),
                        Minute = (regexMatch.Groups["minutes"].Value.Length > 0 ? minutes : null),
                        Second = (regexMatch.Groups["seconds"].Value.Length > 0 ? seconds : null)
                    },
                    Text = phrase.Text,
                    Similarity = 100.0F
                });
            }

            return phrase;
        }

        public static Phrase ReduceTimeTime(IFuzzyState state, Dictionary<string, Token> parameters) {
            var timeA = (TimeVariableTemporalPrimitiveToken) parameters["timeA"];
            var timeB = (TimeVariableTemporalPrimitiveToken) parameters["timeB"];

            return new Phrase() {
                new TimeVariableTemporalPrimitiveToken() {
                    Pattern = timeA.Pattern + timeB.Pattern,
                    Text = string.Format("{0} {1}", timeA.Text, timeB.Text),
                    Similarity = (timeA.Similarity + timeB.Similarity) / 2.0F
                }
            };
        }

        public static Phrase ReduceTimeAndTime(IFuzzyState state, Dictionary<string, Token> parameters) {
            var timeA = (TimeVariableTemporalPrimitiveToken) parameters["timeA"];
            var and = (AndLogicalOperatorToken) parameters["and"];
            var timeB = (TimeVariableTemporalPrimitiveToken) parameters["timeB"];

            return new Phrase() {
                new TimeVariableTemporalPrimitiveToken() {
                    Pattern = timeA.Pattern + timeB.Pattern,
                    Text = string.Format("{0} {1} {2}", timeA.Text, and.Text, timeB.Text),
                    Similarity = (timeA.Similarity + and.Similarity + timeB.Similarity) / 3.0F
                }
            };
        }
    }
}