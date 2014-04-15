#region Copyright
// Copyright 2014 Myrcon Pty. Ltd.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
using System;
using System.Collections.Generic;
using Potato.Fuzzy.Tokens.Primitive.Numeric;
using Potato.Fuzzy.Tokens.Syntax.Typography;

namespace Potato.Fuzzy.Tokens.Operator.Arithmetic.SecondOrder {

    public class MultiplicationSecondOrderArithmeticToken : SecondOrderArithmeticToken {
        public static Phrase Parse(IFuzzyState state, Phrase phrase) {
            return TokenReflection.CreateDescendants<MultiplicationSecondOrderArithmeticToken>(state, phrase);
        }

        public static Phrase ReduceMultiplierMultiplyMultiplicand(IFuzzyState state, Dictionary<String, Token> parameters) {
            FloatNumericPrimitiveToken multiplier = (FloatNumericPrimitiveToken) parameters["multiplier"];
            MultiplicationSecondOrderArithmeticToken multiply = (MultiplicationSecondOrderArithmeticToken) parameters["multiply"];
            FloatNumericPrimitiveToken multiplicand = (FloatNumericPrimitiveToken) parameters["multiplicand"];

            AsteriskTypographySyntaxToken asterisk = new AsteriskTypographySyntaxToken() {
                Text = multiply.Text,
                Similarity = multiply.Similarity
            };

            return MultiplicationSecondOrderArithmeticToken.ReduceMultiplierAsteriskMultiplicand(state, new Dictionary<String, Token>() {
                {"multiplier", multiplier},
                {"asterisk", asterisk},
                {"multiplicand", multiplicand}
            });
        }

        public static Phrase ReduceMultiplierAsteriskMultiplicand(IFuzzyState state, Dictionary<String, Token> parameters) {
            FloatNumericPrimitiveToken multiplier = (FloatNumericPrimitiveToken) parameters["multiplier"];
            AsteriskTypographySyntaxToken asterisk = (AsteriskTypographySyntaxToken) parameters["asterisk"];
            FloatNumericPrimitiveToken multiplicand = (FloatNumericPrimitiveToken) parameters["multiplicand"];

            return new Phrase() {
                new FloatNumericPrimitiveToken() {
                    Value = multiplier.ToFloat() * multiplicand.ToFloat(),
                    Text = String.Format("{0} {1} {2}", multiplier.Text, asterisk.Text, multiplicand.Text),
                    Similarity = (multiplier.Similarity + asterisk.Similarity + multiplicand.Similarity) / 3.0F
                }
            };
        }
    }
}