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

namespace Procon.NLP.Tokens.Primitive.Temporal.Variable.Date.Day {
    using Procon.NLP.Utils;
    using Procon.NLP.Tokens.Primitive.Temporal.Units;
    using Procon.NLP.Tokens.Primitive.Temporal.Variable.Days;
    using Procon.NLP.Tokens.Primitive.Numeric;
    using Procon.NLP.Tokens.Operator.Logical;
    using Procon.NLP.Tokens.Syntax.Adjectives;
    using Procon.NLP.Tokens.Syntax.Articles;
    public class DayVariableTemporalPrimitiveToken : DateVariableTemporalPrimitiveToken {

        public static Phrase Reduce(IStateNLP state, FloatNumericPrimitiveToken number, DaysUnitTemporalPrimitiveToken days) {
            return new Phrase() {
                new DayVariableTemporalPrimitiveToken() {
                    Pattern = new DateTimePatternNLP() {
                        Rule = TimeType.Relative,
                        Day = (int)number.ToFloat().ConvertTo(typeof(int))
                    },
                    Text = String.Format("{0} {1}", number.Text, days.Text),
                    Similarity = (days.Similarity + number.Similarity) / 2.0F
                }
            };
        }

        public static Phrase Reduce(IStateNLP state, FloatNumericPrimitiveToken number, WeeksUnitTemporalPrimitiveToken weeks) {
            return new Phrase() {
                new DayVariableTemporalPrimitiveToken() {
                    Pattern = new DateTimePatternNLP() {
                        Rule = TimeType.Relative,
                        Day = (int)number.ToFloat().ConvertTo(typeof(int)) * 7
                    },
                    Text = String.Format("{0} {1}", number.Text, weeks.Text),
                    Similarity = (weeks.Similarity + number.Similarity) / 2.0F
                }
            };
        }

        public static Phrase Reduce(IStateNLP state, IndefiniteArticlesSyntaxToken article, DaysUnitTemporalPrimitiveToken days) {
            return new Phrase() {
                new DayVariableTemporalPrimitiveToken() {
                    Pattern = new DateTimePatternNLP() {
                        Rule = TimeType.Relative,
                        Day = 1
                    },
                    Text = String.Format("{0} {1}", article.Text, days.Text),
                    Similarity = (days.Similarity + article.Similarity) / 2.0F
                }
            };
        }

        public static Phrase Reduce(IStateNLP state, IndefiniteArticlesSyntaxToken article, WeeksUnitTemporalPrimitiveToken weeks) {
            return new Phrase() {
                new DayVariableTemporalPrimitiveToken() {
                    Pattern = new DateTimePatternNLP() {
                        Rule = TimeType.Relative,
                        Day = 7
                    },
                    Text = String.Format("{0} {1}", article.Text, weeks.Text),
                    Similarity = (weeks.Similarity + article.Similarity) / 2.0F
                }
            };
        }

        /* TO DO: What is this?
        [RefactoringTokenMethod]
        public static SentenceNLP NumberDays(ProconState state, SentenceNLP sentence, AdjectiveSyntaxToken adjective, DayTemporalPrimitiveToken day) {

            // sentence.ReplaceRange(0, sentence.Count, new DayVariableTemporalToken() { Value = new DateTime(1, 1, (int)number.ToFloat()), MatchedText = sentence.ToString(), Similarity = (days.Similarity + number.Similarity) / 2.0F });

            return sentence;
        }
        */
    }
}
