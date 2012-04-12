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
using System.Text;
using System.Text.RegularExpressions;

namespace Procon.NLP.Tokens.Primitive.Temporal.Variable.Date {
    using Procon.NLP.Tokens.Operator.Logical;
    public class DateVariableTemporalPrimitiveToken : DateTimeTemporalPrimitiveToken {

        protected static readonly Regex m_regexMatch = new Regex(@"^([0-9]+)[ ]?[-/.][ ]?([0-9]+)[ ]?[- /.][ ]?([0-9]{2,4})$", RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);

        public static Phrase Parse(IStateNLP state, Phrase phrase) {

            Match regexMatch = DateVariableTemporalPrimitiveToken.m_regexMatch.Match(phrase.Text);

            if (regexMatch.Success == true) {
                DateTime dt;
                if (DateTime.TryParse(phrase.Text, out dt) == true) {

                    phrase.Add(
                        new DateVariableTemporalPrimitiveToken() {
                            Pattern = new DateTimePatternNLP() {
                                Rule = TimeType.Definitive,
                                Year = dt.Year,
                                Month = dt.Month,
                                Day = dt.Day
                            },
                            Text = phrase.Text,
                            Similarity = 100.0F
                        }
                    );
                }
            }

            return phrase;
/*
                }

                int? hours = regexMatch.Groups["hours"].Value.Length > 0 ? int.Parse(regexMatch.Groups["hours"].Value) : 0;
                int? minutes = regexMatch.Groups["minutes"].Value.Length > 0 ? int.Parse(regexMatch.Groups["minutes"].Value) : 0;
                int? seconds = regexMatch.Groups["seconds"].Value.Length > 0 ? int.Parse(regexMatch.Groups["seconds"].Value) : 0;

                if (String.Compare(regexMatch.Groups["meridiem"].Value, "pm", true) == 0 && hours < 12) {
                    hours += 12;
                }
                else if (String.Compare(regexMatch.Groups["meridiem"].Value, "am", true) == 0) {
                    if (hours == 12) {
                        hours = 0;
                    }
                }

                hours %= 24;

                DateTimePatternNLP newDateTime = new DateTimePatternNLP() {
                    Rule = TimeType.Definitive,
                    Hour = (regexMatch.Groups["hours"].Value.Length > 0 ? hours : null),
                    Minute = (regexMatch.Groups["minutes"].Value.Length > 0 ? minutes : null),
                    Second = (regexMatch.Groups["seconds"].Value.Length > 0 ? seconds : null)
                };

                phrase.Add(
                    new TimeVariableTemporalPrimitiveToken() {
                        Pattern = newDateTime,
                        Text = phrase.Text,
                        Similarity = 100.0F
                    }
                );
                    
            }
*/
        }

        // 

        public static Phrase Reduce(IStateNLP state, DateVariableTemporalPrimitiveToken dateA, DateVariableTemporalPrimitiveToken dateB) {
            return new Phrase() {
                new DateVariableTemporalPrimitiveToken() {
                    Pattern = dateA.Pattern + dateB.Pattern,
                    Text = String.Format("{0} {1}", dateA.Text, dateB.Text),
                    Similarity = (dateA.Similarity + dateB.Similarity) / 2.0F
                }
            };
        }

        public static Phrase Reduce(IStateNLP state, DateVariableTemporalPrimitiveToken dateA, AndLogicalOperatorToken and, DateVariableTemporalPrimitiveToken dateB) {
            return new Phrase() {
                new DateVariableTemporalPrimitiveToken() {
                    Pattern = dateA.Pattern + dateB.Pattern,
                    Text = String.Format("{0} {1} {2}", dateA.Text, and.Text, dateB.Text),
                    Similarity = (dateA.Similarity + and.Similarity + dateB.Similarity) / 2.0F
                }
            };
        }
        /*
        [RefactoringTokenMethod]
        public static SentenceNLP DateDate(ProconState state, SentenceNLP sentence, DateVariableTemporalToken dateA, DateVariableTemporalToken dateB) {
            sentence.ReplaceRange(0, sentence.Count, );

            return sentence;
        }

        [RefactoringTokenMethod]
        public static SentenceNLP AndDateDate(ProconState state, SentenceNLP sentence, AndLogicalOperatorToken and, DateVariableTemporalToken dateA, DateVariableTemporalToken dateB) {

            return DateVariableTemporalToken.DateDate(state, sentence, dateA, dateB);
        }
        */
    }
}
