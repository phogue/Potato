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

namespace Procon.NLP.Tokens.Primitive.Temporal.Variable.Time {
    using Operator.Logical;
    using Primitive.Numeric;
    using Temporal.Units;
    using Temporal.Units.Meridiem;
    using Temporal.Variable.Time.Hour;
    using Temporal.Variable.Time.Minute;
    using Temporal.Variable.Time.Second;
    public class TimeVariableTemporalPrimitiveToken : DateTimeTemporalPrimitiveToken {

        protected static readonly Regex m_regexMatch = new Regex(@"^(((?<hours>[0]?[1-9]|1[0-2])[ ]*(:|\.)[ ]*(?<minutes>[0-5][0-9])([ ]*(:|\.)[ ]*(?<seconds>[0-5][0-9]))?[ ]*(?<meridiem>AM|am|aM|Am|PM|pm|pM|Pm))|((?<hours>[0]?[0-9]|1[0-9]|2[0-3])[ ]*(:|\.)[ ]*(?<minutes>[0-5][0-9])([ ]*(:|\.)[ ]*(?<seconds>[0-5][0-9]))?))$", RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);

        public static Phrase Parse(IStateNLP state, Phrase phrase) {

            Match regexMatch = TimeVariableTemporalPrimitiveToken.m_regexMatch.Match(phrase.Text);

            if (regexMatch.Success == true) {

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

                phrase.Add(
                    new TimeVariableTemporalPrimitiveToken() {
                        Pattern = new DateTimePatternNLP() {
                            Rule = TimeType.Definitive,
                            Hour = (regexMatch.Groups["hours"].Value.Length > 0 ? hours : null),
                            Minute = (regexMatch.Groups["minutes"].Value.Length > 0 ? minutes : null),
                            Second = (regexMatch.Groups["seconds"].Value.Length > 0 ? seconds : null)
                        },
                        Text = phrase.Text,
                        Similarity = 100.0F
                    }
                );

            }

            return phrase;
        }

        public static Phrase Reduce(IStateNLP state, TimeVariableTemporalPrimitiveToken timeA, TimeVariableTemporalPrimitiveToken timeB) {
            return new Phrase() {
                new TimeVariableTemporalPrimitiveToken() {
                    Pattern = timeA.Pattern + timeB.Pattern,
                    Text = String.Format("{0} {1}", timeA.Text, timeB.Text),
                    Similarity = (timeA.Similarity + timeB.Similarity) / 2.0F }
            };
        }

        public static Phrase Reduce(IStateNLP state, TimeVariableTemporalPrimitiveToken timeA, AndLogicalOperatorToken and, TimeVariableTemporalPrimitiveToken timeB) {
            return new Phrase() {
                new TimeVariableTemporalPrimitiveToken() {
                    Pattern = timeA.Pattern + timeB.Pattern,
                    Text = String.Format("{0} {1} {2}", timeA.Text, and.Text, timeB.Text),
                    Similarity = (timeA.Similarity + and.Similarity + timeB.Similarity) / 3.0F }
            };
        }

        /*
        [RefactoringTokenMethod]
        public static SentenceNLP TimeTime(ProconState state, SentenceNLP sentence, TimeVariableTemporalToken timeA, TimeVariableTemporalToken timeB) {
            sentence.ReplaceRange(0, sentence.Count, );

            return sentence;
        }

        [RefactoringTokenMethod]
        public static SentenceNLP AndTimeTime(ProconState state, SentenceNLP sentence, AndLogicalOperatorToken and, TimeVariableTemporalToken timeA, TimeVariableTemporalToken timeB) {

            return TimeVariableTemporalToken.TimeTime(state, sentence, timeA, timeB);
        }

        
        [RefactoringTokenMethod]
        [Obsolete]
        public static SentenceNLP NumberColonNumberMeridiem(ProconState state, SentenceNLP sentence, FloatNumberToken hour, ColonPunctuationToken colon, FloatNumberToken minute, MeridiemTemporalToken meridiem) {
            // 5:15:26 PM -> [hour(5)] [minute(15)] [:] [26] [PM]
            // so it won't match again for the seconds.

            int calculatedHour = (int)(hour.ToFloat() + (minute.ToFloat() >= 60.0F ? (float)Math.Abs(minute.ToFloat() / 60.0F) : 0.0F));
            int calculatedMinute = (int)minute.ToFloat() % 60;

            if (meridiem is PostMeridiemTemporalToken && calculatedHour < 12) {
                calculatedHour += 12;
            }
            else if (meridiem is AnteMeridiemTemporalToken) {
                if (calculatedHour == 12) {
                    calculatedHour = 0;
                }
            }

            calculatedHour %= 24;

            sentence.ReplaceRange(0, sentence.Count, new SentenceNLP() { new HourVariableTemporalToken() { Value = new DateTime(1, 1, 1, calculatedHour, 0, 0), MatchedText = hour.ToString(), Similarity = 1.0F }, new MinuteVariableTemporalToken() { Value = new DateTime(1, 1, 1, 0, calculatedMinute, 0), MatchedText = minute.ToString(), Similarity = 1.0F } });

            return sentence;
        }
        */
    }
}
