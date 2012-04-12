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

namespace Procon.NLP.Tokens.Primitive.Temporal.Variable.Date.Month {
    using Procon.NLP.Utils;
    using Procon.NLP.Tokens.Primitive.Temporal.Units;
    using Procon.NLP.Tokens.Primitive.Temporal.Variable.Months;
    using Procon.NLP.Tokens.Primitive.Numeric;
    using Procon.NLP.Tokens.Syntax.Articles;

    public class MonthVariableTemporalPrimitiveToken : DateVariableTemporalPrimitiveToken {

        public static Phrase Reduce(IStateNLP state, FloatNumericPrimitiveToken number, MonthsUnitTemporalPrimitiveToken month) {
            return new Phrase() {
                new MonthVariableTemporalPrimitiveToken() {
                    Pattern = new DateTimePatternNLP() {
                        Rule = TimeType.Relative,// Rule = TimeType.Definitive,
                        Month = (int)number.ToFloat().ConvertTo(typeof(int))
                    },
                    Text = String.Format("{0} {1}", number.Text, month.Text),
                    Similarity = (month.Similarity + number.Similarity) / 2.0F
                }
            };
        }

        public static Phrase Reduce(IStateNLP state, OrdinalNumericPrimitiveToken ordinal, MonthMonthsVariableTemporalPrimitiveToken month) {

            DateTimePatternNLP pattern = month.Pattern;
            pattern.Day = (int)ordinal.ToFloat().ConvertTo(typeof(int));

            return new Phrase() {
                new MonthVariableTemporalPrimitiveToken() {
                    Pattern = pattern,
                    Text = String.Format("{0} {1}", ordinal.Text, month.Text),
                    Similarity = (month.Similarity + ordinal.Similarity) / 2.0F
                }
            };
        }

        public static Phrase Reduce(IStateNLP state, IndefiniteArticlesSyntaxToken article, MonthsUnitTemporalPrimitiveToken month) {
            return new Phrase() {
                new MonthVariableTemporalPrimitiveToken() {
                    Pattern = new DateTimePatternNLP() {
                        Rule = TimeType.Relative,
                        Month = 1
                    },
                    Text = String.Format("{0} {1}", article.Text, month.Text),
                    Similarity = (month.Similarity + article.Similarity) / 2.0F
                }
            };
        }
    }
}
