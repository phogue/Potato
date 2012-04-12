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

namespace Procon.NLP.Tokens.Primitive.Temporal.Variable.Time.Hour {
    using Procon.NLP.Tokens.Primitive.Temporal.Units;
    using Procon.NLP.Tokens.Primitive.Temporal.Units.Meridiem;
    using Procon.NLP.Tokens.Primitive.Numeric;
    using Procon.NLP.Tokens.Syntax.Adjectives;
    using Procon.NLP.Tokens.Syntax.Articles;
    using Procon.NLP.Utils;

    public class HourVariableTemporalPrimitiveToken : TimeVariableTemporalPrimitiveToken {

        public static Phrase Reduce(IStateNLP state, FloatNumericPrimitiveToken number, HoursUnitTemporalPrimitiveToken hours) {
            return new Phrase() {
                new HourVariableTemporalPrimitiveToken() {
                    Pattern = new DateTimePatternNLP() {
                        Rule = TimeType.Relative,
                        Hour = (int)number.ToFloat().ConvertTo(typeof(int))
                    },
                    Text = String.Format("{0} {1}", number.Text, hours.Text),
                    Similarity = (hours.Similarity + number.Similarity) / 2.0F
                }
            };
        }

        public static Phrase Reduce(IStateNLP state, FloatNumericPrimitiveToken number, MeridiemUnitsTemporalPrimitiveToken meridiem) {

            int hours = (int)number.ToFloat().ConvertTo(typeof(int)) % 24;
            if (meridiem is PostMeridiemUnitsTemporalPrimitiveToken) {
                hours += 12;
            }
            
            return new Phrase() {
                new HourVariableTemporalPrimitiveToken() {
                    Pattern = new DateTimePatternNLP() {
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

        public static Phrase Reduce(IStateNLP state, IndefiniteArticlesSyntaxToken article, HoursUnitTemporalPrimitiveToken hours) {
            return new Phrase() {
                new HourVariableTemporalPrimitiveToken() {
                    Pattern = new DateTimePatternNLP() {
                        Rule = TimeType.Relative,
                        Hour = 1
                    },
                    Text = String.Format("{0} {1}", article.Text, hours.Text),
                    Similarity = (hours.Similarity + article.Similarity) / 2.0F
                }
            };
        }

        public static Phrase Reduce(IStateNLP state, EveryAdjectiveSyntaxToken every, HoursUnitTemporalPrimitiveToken hours) {
            return new Phrase() {
                new HourVariableTemporalPrimitiveToken() {
                    Pattern = new DateTimePatternNLP() {
                        Rule = TimeType.Relative,
                        Modifier = TimeModifier.Interval,
                        Hour = 1
                    },
                    Text = String.Format("{0} {1}", every.Text, hours.Text),
                    Similarity = (hours.Similarity + every.Similarity) / 2.0F
                }
            };
        }

        /*
        [RefactoringTokenMethod]
        public static SentenceNLP NumberHours(ProconState state, SentenceNLP sentence, FloatNumberToken number, HoursUnitTemporalPrimitiveToken hours) {

            sentence.ReplaceRange(0, sentence.Count, );

            return sentence;
        }
        */
    }
}
