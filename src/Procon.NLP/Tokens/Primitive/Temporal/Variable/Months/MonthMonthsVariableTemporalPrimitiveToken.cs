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

namespace Procon.NLP.Tokens.Primitive.Temporal.Variable.Months {
    using Syntax.Adjectives;

    public class MonthMonthsVariableTemporalPrimitiveToken : DateTimeTemporalPrimitiveToken {

        public static Phrase Reduce(IStateNLP state, AdjectiveSyntaxToken adjective, MonthMonthsVariableTemporalPrimitiveToken month) {

            DateTime newDateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

            if (adjective is LastAdjectiveSyntaxToken) {
                // Today - Maximum of 7
                do {
                    newDateTime = newDateTime.AddMonths(-1);
                } while (newDateTime.Month != month.Pattern.Month);
            }
            else if (adjective is NextAdjectiveSyntaxToken) {
                // Today + Maximum of 7
                do {
                    newDateTime = newDateTime.AddMonths(1);
                } while (newDateTime.Month != month.Pattern.Month);
            }
            else if (adjective is ThisAdjectiveSyntaxToken) {
                // Today or +7
                while (newDateTime.Month != month.Pattern.Month) {
                    newDateTime = newDateTime.AddMonths(1);
                }
            }

            return new Phrase() {
                new MonthMonthsVariableTemporalPrimitiveToken() {
                    Pattern = new DateTimePatternNLP() {
                        Year = newDateTime.Year,
                        Month = newDateTime.Month
                    },
                    Text = String.Format("{0} {1}", adjective.Text, month.Text),
                    Similarity = (month.Similarity + adjective.Similarity) / 2.0F
                }
            };
        }

    }
}
