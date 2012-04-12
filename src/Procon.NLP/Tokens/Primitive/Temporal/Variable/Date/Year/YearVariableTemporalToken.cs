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

namespace Procon.NLP.Tokens.Primitive.Temporal.Variable.Date.Year {
    using Procon.NLP.Utils;
    using Procon.NLP.Tokens.Primitive.Temporal.Units;
    using Procon.NLP.Tokens.Primitive.Numeric;
    using Procon.NLP.Tokens.Syntax.Articles;

    public class YearVariableTemporalToken : DateVariableTemporalPrimitiveToken {

        //protected static Regex m_regexMatch = new System.Text.RegularExpressions.Regex(@"^\'(?<number>[0-9][0-9])$", System.Text.RegularExpressions.RegexOptions.Compiled);

        //[BaseTokenMethod]
        //public static Token ToBaseToken(ProconState state, Token token) {

        //    Token returnToken = token;

        //    if (Token.isMatch(null, YearVariableTemporalToken.m_regexMatch, token) == 1.0F) {

        //        // Optimize so its not running this twice.
        //        Match regexMatch = YearVariableTemporalToken.m_regexMatch.Match(token.MatchedText);
                
        //        // TO DO: If needed "84" needs to be "1984"
        //        returnToken = new YearVariableTemporalToken() { Value = new DateTime(int.Parse(regexMatch.Groups["number"].Value), 0, 0), MatchedText = token.MatchedText, Similarity = 1.0F };
        //    }

        //    return returnToken;
        //}

        public static Phrase Reduce(IStateNLP state, FloatNumericPrimitiveToken number, YearsUnitTemporalPrimitiveToken year) {
            return new Phrase() {
                new YearVariableTemporalToken() {
                    Pattern = new DateTimePatternNLP() {
                        Rule = TimeType.Relative,//Rule = TimeType.Definitive,
                        Year = (int)number.ToFloat().ConvertTo(typeof(int))
                    },
                    Text = String.Format("{0} {1}", number.Text, year.Text),
                    Similarity = (year.Similarity + number.Similarity) / 2.0F
                }
            };
        }

        public static Phrase Reduce(IStateNLP state, IndefiniteArticlesSyntaxToken article, YearsUnitTemporalPrimitiveToken year) {
            return new Phrase() {
                new YearVariableTemporalToken() {
                    Pattern = new DateTimePatternNLP() {
                        Rule = TimeType.Relative,
                        Year = 1
                    },
                    Text = String.Format("{0} {1}", article.Text, year.Text),
                    Similarity = (year.Similarity + article.Similarity) / 2.0F
                }
            };
        }
    }
}
