#region Copyright
// Copyright 2015 Geoff Green.
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

namespace Potato.Fuzzy.Tokens.Operator.Arithmetic.ThirdOrder {

    public class AdditionThirdOrderArithmeticOperatorToken : ThirdOrderArithmeticOperatorToken {
        public static Phrase Parse(IFuzzyState state, Phrase phrase) {
            return TokenReflection.CreateDescendants<AdditionThirdOrderArithmeticOperatorToken>(state, phrase);
        }

        public static Phrase ReduceNumberAdditionNumber(IFuzzyState state, Dictionary<string, Token> parameters) {
            var addend1 = (FloatNumericPrimitiveToken) parameters["addend1"];
            var addition = (AdditionThirdOrderArithmeticOperatorToken) parameters["addition"];
            var addend2 = (FloatNumericPrimitiveToken) parameters["addend2"];

            var plus = new PlusTypographySyntaxToken() {
                Text = addition.Text,
                Similarity = addition.Similarity
            };

            return ReduceNumberPlusNumber(state, new Dictionary<string, Token>() {
                {"addend1", addend1},
                {"plus", plus},
                {"addend2", addend2}
            });
        }

        public static Phrase ReduceNumberPlusNumber(IFuzzyState state, Dictionary<string, Token> parameters) {
            var addend1 = (FloatNumericPrimitiveToken) parameters["addend1"];
            var plus = (PlusTypographySyntaxToken) parameters["plus"];
            var addend2 = (FloatNumericPrimitiveToken) parameters["addend2"];

            return new Phrase() {
                new FloatNumericPrimitiveToken() {
                    Value = addend1.ToFloat() + addend2.ToFloat(),
                    Text = string.Format("{0} {1} {2}", addend1.Text, plus.Text, addend2.Text),
                    Similarity = (addend1.Similarity + plus.Similarity + addend2.Similarity) / 3.0F
                }
            };
        }
    }
}