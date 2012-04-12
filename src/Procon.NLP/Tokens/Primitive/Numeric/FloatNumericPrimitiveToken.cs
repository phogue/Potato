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

namespace Procon.NLP.Tokens.Primitive.Numeric {
    public class FloatNumericPrimitiveToken : NumericPrimitiveToken {
        public static Phrase Parse(IStateNLP state, Phrase phrase) {
            return TokenReflection.CreateDescendants<FloatNumericPrimitiveToken>(state, phrase);
        }

        //protected static Regex RegexMatch = new System.Text.RegularExpressions.Regex(@"^(?<value>[-+]?[0-9]+[ ]?[\.,]?[ ]?[0-9]*)$", System.Text.RegularExpressions.RegexOptions.Compiled);

        public float ToFloat() {
            float returnValue = 0.0F;

            if (this.Value is float) {
                returnValue = (float)this.Value;
            }
            else if (this.Value is string) {
                if (float.TryParse(((string)this.Value).Replace(" ", ""), out returnValue) == true) {
                    // So future conversions don't need to be converted.
                    this.Value = returnValue;
                    this.Text = this.Text.Replace(" ", "");
                }
            }

            return returnValue;
        }

        /*
        public static Phrase Parse(IStateNLP state, Phrase phrase) {

            if (FloatNumericPrimitiveToken.RegexMatch.Match(phrase.Text).Success == true) {
                phrase.Add(new FloatNumericPrimitiveToken() {
                    Text = phrase.Text.Replace(" ", ""),
                    Similarity = 100,
                    Value = float.Parse(phrase.Text.Replace(" ", ""))
                });
            }

            return phrase;
            //return TokenReflection.CreateDescendants<ExcludingLogicalOperatorToken>(state, phrase);
        }
        */
    }
}
