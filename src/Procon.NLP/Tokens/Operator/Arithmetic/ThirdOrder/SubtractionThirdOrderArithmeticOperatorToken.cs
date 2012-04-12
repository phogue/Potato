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
    using Procon.NLP.Tokens.Syntax.Punctuation;
    public class SubtractionThirdOrderArithmeticOperatorToken : ThirdOrderArithmeticOperatorToken {
        public static Phrase Parse(IStateNLP state, Phrase phrase) {
            return TokenReflection.CreateDescendants<SubtractionThirdOrderArithmeticOperatorToken>(state, phrase);
        }

        public static Phrase Reduce(IStateNLP state, FloatNumericPrimitiveToken dividend, SubtractionThirdOrderArithmeticOperatorToken subtraction, FloatNumericPrimitiveToken divisor) {
            HyphenPunctuationSyntaxToken subtract = new HyphenPunctuationSyntaxToken() {
                Text = subtraction.Text,
                Similarity = subtraction.Similarity
            };

            return SubtractionThirdOrderArithmeticOperatorToken.Reduce(state, dividend, subtract, divisor);
        }

        public static Phrase Reduce(IStateNLP state, FloatNumericPrimitiveToken minuend, HyphenPunctuationSyntaxToken subtract, FloatNumericPrimitiveToken subtrahend) {
            return new Phrase() {
                new FloatNumericPrimitiveToken() {
                    Value = minuend.ToFloat() - subtrahend.ToFloat(),
                    Text = String.Format("{0} {1} {2}", minuend.Text, subtract.Text, subtrahend.Text),
                    Similarity = (minuend.Similarity + subtract.Similarity + subtrahend.Similarity) / 3.0F
                }
            };
        }
    }
}
