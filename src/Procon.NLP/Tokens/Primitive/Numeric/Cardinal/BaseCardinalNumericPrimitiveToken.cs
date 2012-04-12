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

/*
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Procon.NLP.Tokens.Primitive.Numeric.Cardinal {
    public class BaseCardinalNumericPrimitiveToken : CardinalNumericPrimitiveToken {
        public static Phrase Parse(IStateNLP state, Phrase phrase) {
            return TokenReflection.CreateDescendants<BaseCardinalNumericPrimitiveToken>(state, phrase);
        }


        public static Dictionary<string, int> m_aliasesDictionary = new Dictionary<string, int>() {
            { "zero", 0 },
            { "one", 1 },
            { "two", 2 },
            { "three", 3 },
            { "four", 4 },
            { "five", 5 },
            { "six", 6 },
            { "seven", 7 },
            { "eight", 8 },
            { "nine", 9 },
            { "eleven", 11 },
            { "twelve", 12 },
            { "thirteen", 13 },
            { "fourteen", 14 },
            { "fifteen", 15 },
            { "sixteen", 16 },
            { "seventeen", 17 },
            { "eighteen", 18 },
            { "nineteen", 19 },
            { "ten", 10 },
            { "twenty", 20 },
            { "thirty", 30 },
            { "forty", 40 },
            { "fifty", 50 },
            { "sixty", 60 },
            { "seventy", 70 },
            { "eighty", 80 },
            { "ninety", 90 },
        };

        public static List<string> m_aliasesDictionaryKeys = new List<string>(BaseCardinalNumericPrimitiveToken.m_aliasesDictionary.Keys);

        [BaseTokenMethod]
        public static Token ToBaseToken(ProconState state, Token token) {

            Token returnToken = token;
            float similarity = 0;
            string matchedKey = String.Empty;

            if ((similarity = Token.isMatch(BaseCardinalNumericPrimitiveToken.m_aliasesDictionaryKeys, token, out matchedKey)) >= 0.8F) {

                returnToken = new BaseCardinalNumericPrimitiveToken() { Value = BaseCardinalNumericPrimitiveToken.m_aliasesDictionary[matchedKey], MatchedText = token.MatchedText, Similarity = similarity };
            }

            return returnToken;
        }
    }
}
*/