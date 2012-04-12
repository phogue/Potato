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

namespace Procon.NLP.Tokens.Operator.Arithmetic.ThirdOrder {
    using Procon.NLP.Tokens.Primitive.Numeric;
    using Procon.NLP.Tokens.Syntax.Typography;
    using Procon.NLP.Tokens.Operator.Logical;

    public class AdditionThirdOrderArithmeticOperatorToken : ThirdOrderArithmeticOperatorToken {
        public static Phrase Parse(IStateNLP state, Phrase phrase) {
            return TokenReflection.CreateDescendants<AdditionThirdOrderArithmeticOperatorToken>(state, phrase);
        }

        public static Phrase Reduce(IStateNLP state, FloatNumericPrimitiveToken addend1, AdditionThirdOrderArithmeticOperatorToken addition, FloatNumericPrimitiveToken addend2) {
            PlusTypographySyntaxToken plus = new PlusTypographySyntaxToken() {
                Text = addition.Text,
                Similarity = addition.Similarity
            };

            return AdditionThirdOrderArithmeticOperatorToken.Reduce(state, addend1, plus, addend2);
        }

        public static Phrase Reduce(IStateNLP state, FloatNumericPrimitiveToken addend1, PlusTypographySyntaxToken plus, FloatNumericPrimitiveToken addend2) {
            return new Phrase() {
                new FloatNumericPrimitiveToken() {
                    Value = addend1.ToFloat() + addend2.ToFloat(),
                    Text = String.Format("{0} {1} {2}", addend1.Text, plus.Text, addend2.Text),
                    Similarity = (addend1.Similarity + plus.Similarity + addend2.Similarity) / 3.0F
                }
            };
        }
    }
}
