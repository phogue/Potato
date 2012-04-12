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

namespace Procon.NLP.Tokens.Primitive.Temporal.Variable.Days {
    using Syntax.Adjectives;
    public class DaysVariableTemporalPrimitiveToken : DateTimeTemporalPrimitiveToken {
        public static Phrase Reduce(IStateNLP state, AdjectiveSyntaxToken adjective, DaysVariableTemporalPrimitiveToken day) {

            DateTime newDateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

            if (adjective is LastAdjectiveSyntaxToken) {
                // Today - Maximum of 7
                do {
                    newDateTime = newDateTime.AddDays(-1);
                } while (newDateTime.DayOfWeek != day.Pattern.DayOfWeek);
            }
            else if (adjective is NextAdjectiveSyntaxToken) {
                // Today + Maximum of 7
                do {
                    newDateTime = newDateTime.AddDays(1);
                } while (newDateTime.DayOfWeek != day.Pattern.DayOfWeek);
            }
            else if (adjective is ThisAdjectiveSyntaxToken) {
                // Today or +7
                while (newDateTime.DayOfWeek != day.Pattern.DayOfWeek) {
                    newDateTime = newDateTime.AddDays(1);
                }
            }

            return new Phrase() {
                new DaysVariableTemporalPrimitiveToken() {
                    Pattern = new DateTimePatternNLP() {
                        Rule = TimeType.Definitive,
                        Year = newDateTime.Year,
                        Month = newDateTime.Month,
                        Day = newDateTime.Day
                    },
                    Text = String.Format("{0} {1}", adjective.Text, day.Text),
                    Similarity = (day.Similarity + adjective.Similarity) / 2.0F
                }
            };
        }

    }
}
