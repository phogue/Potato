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
using Procon.Fuzzy.Tokens.Primitive.Numeric;
using Procon.Fuzzy.Tokens.Syntax.Typography;

namespace Procon.Fuzzy.Tokens.Operator.Arithmetic.ThirdOrder {

    public class AdditionThirdOrderArithmeticOperatorToken : ThirdOrderArithmeticOperatorToken {
        public static Phrase Parse(IFuzzyState state, Phrase phrase) {
            return TokenReflection.CreateDescendants<AdditionThirdOrderArithmeticOperatorToken>(state, phrase);
        }

        public static Phrase ReduceNumberAdditionNumber(IFuzzyState state, Dictionary<String, Token> parameters) {
            FloatNumericPrimitiveToken addend1 = (FloatNumericPrimitiveToken) parameters["addend1"];
            AdditionThirdOrderArithmeticOperatorToken addition = (AdditionThirdOrderArithmeticOperatorToken) parameters["addition"];
            FloatNumericPrimitiveToken addend2 = (FloatNumericPrimitiveToken) parameters["addend2"];

            PlusTypographySyntaxToken plus = new PlusTypographySyntaxToken() {
                Text = addition.Text,
                Similarity = addition.Similarity
            };

            return AdditionThirdOrderArithmeticOperatorToken.ReduceNumberPlusNumber(state, new Dictionary<String, Token>() {
                {"addend1", addend1},
                {"plus", plus},
                {"addend2", addend2}
            });
        }

        public static Phrase ReduceNumberPlusNumber(IFuzzyState state, Dictionary<String, Token> parameters) {
            FloatNumericPrimitiveToken addend1 = (FloatNumericPrimitiveToken) parameters["addend1"];
            PlusTypographySyntaxToken plus = (PlusTypographySyntaxToken) parameters["plus"];
            FloatNumericPrimitiveToken addend2 = (FloatNumericPrimitiveToken) parameters["addend2"];

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