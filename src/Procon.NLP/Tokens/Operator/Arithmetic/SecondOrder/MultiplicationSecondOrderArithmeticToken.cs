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

namespace Procon.NLP.Tokens.Operator.Arithmetic.SecondOrder {
    using Procon.NLP.Tokens.Primitive.Numeric;
    using Procon.NLP.Tokens.Syntax.Typography;
    using Procon.NLP.Tokens.Primitive.Numeric.Cardinal;

    public class MultiplicationSecondOrderArithmeticToken : SecondOrderArithmeticToken {
        public static Phrase Parse(IStateNLP state, Phrase phrase) {
            return TokenReflection.CreateDescendants<MultiplicationSecondOrderArithmeticToken>(state, phrase);
        }

        public static Phrase Reduce(IStateNLP state, FloatNumericPrimitiveToken dividend, MultiplicationSecondOrderArithmeticToken multiply, FloatNumericPrimitiveToken divisor) {
            AsteriskTypographySyntaxToken astrix = new AsteriskTypographySyntaxToken() {
                Text = multiply.Text,
                Similarity = multiply.Similarity
            };

            return MultiplicationSecondOrderArithmeticToken.Reduce(state, dividend, astrix, divisor);
        }

        public static Phrase Reduce(IStateNLP state, FloatNumericPrimitiveToken multiplier, AsteriskTypographySyntaxToken asterisk, FloatNumericPrimitiveToken multiplicand) {
            return new Phrase() {
                new FloatNumericPrimitiveToken() {
                    Value = multiplier.ToFloat() * multiplicand.ToFloat(),
                    Text = String.Format("{0} {1} {2}", multiplier.Text, asterisk.Text, multiplicand.Text),
                    Similarity = (multiplier.Similarity + asterisk.Similarity + multiplicand.Similarity) / 3.0F
                }
            };
        }

        /*
        [RefactoringTokenMethod]
        public static SentenceNLP GreaterThanLessThanLogicalFloatFloat(ProconState state, SentenceNLP sentence, AsteriskTypographySyntaxToken asterisk, FloatNumberToken numberA, FloatNumberToken numberB) {
            sentence.ReplaceRange(0, sentence.Count, new SentenceNLP() { numberA, new MultiplicationSecondOrderArithmeticToken() { Value = asterisk.ToString(), MatchedText = asterisk.ToString(), Similarity = asterisk.Similarity }, numberB });

            return sentence;
        }
        */
    }
}
