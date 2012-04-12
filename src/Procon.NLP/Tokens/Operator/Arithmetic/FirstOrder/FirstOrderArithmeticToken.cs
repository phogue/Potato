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

namespace Procon.NLP.Tokens.Operator.Arithmetic.FirstOrder {
    using Procon.NLP.Tokens.Primitive.Numeric;
    using Procon.NLP.Tokens.Operator.Logical;
    using Procon.NLP.Tokens.Primitive.Numeric.Cardinal;

    public class FirstOrderArithmeticToken : ArithmeticOperatorToken {

        public static Phrase Reduce(IStateNLP state, FloatNumericPrimitiveToken multiplier, MultiplicandCardinalNumericPrimitiveToken multiplicand) {
            return new Phrase() {
                new FloatNumericPrimitiveToken() {
                    Value = multiplier.ToFloat() * multiplicand.ToFloat(),
                    Text = String.Format("{0} {1}", multiplier.Text, multiplicand.Text),
                    Similarity = (multiplier.Similarity + multiplicand.Similarity) / 2.0F
                }
            };
        }

        public static Phrase Reduce(IStateNLP state, FloatNumericPrimitiveToken addend1, AndLogicalOperatorToken and, FloatNumericPrimitiveToken addend2) {
            return new Phrase() {
                new FloatNumericPrimitiveToken() {
                    Value = addend1.ToFloat() + addend2.ToFloat(),
                    Text = String.Format("{0} {1} {2}", addend1.Text, and.Text, addend2.Text),
                    Similarity = (addend1.Similarity + and.Similarity + addend2.Similarity) / 2.0F
                }
            };
        }

        public static Phrase Reduce(IStateNLP state, FloatNumericPrimitiveToken addend1, FloatNumericPrimitiveToken addend2) {
            return new Phrase() {
                new FloatNumericPrimitiveToken() {
                    Value = addend1.ToFloat() + addend2.ToFloat(),
                    Text = String.Format("{0} {1}", addend1.Text, addend2.Text),
                    Similarity = (addend1.Similarity + addend2.Similarity) / 2.0F
                }
            };
        }

    }
}
