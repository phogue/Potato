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
using Potato.Fuzzy.Tokens.Syntax.Punctuation;

namespace Potato.Fuzzy.Tokens.Operator.Arithmetic.ThirdOrder {

    public class SubtractionThirdOrderArithmeticOperatorToken : ThirdOrderArithmeticOperatorToken {
        public static Phrase Parse(IFuzzyState state, Phrase phrase) {
            return TokenReflection.CreateDescendants<SubtractionThirdOrderArithmeticOperatorToken>(state, phrase);
        }

        public static Phrase ReduceNumberSubtractionNumber(IFuzzyState state, Dictionary<String, Token> parameters) {
            FloatNumericPrimitiveToken minuend = (FloatNumericPrimitiveToken) parameters["minuend"];
            SubtractionThirdOrderArithmeticOperatorToken subtraction = (SubtractionThirdOrderArithmeticOperatorToken) parameters["subtraction"];
            FloatNumericPrimitiveToken subtrahend = (FloatNumericPrimitiveToken) parameters["subtrahend"];

            HyphenPunctuationSyntaxToken hyphen = new HyphenPunctuationSyntaxToken() {
                Text = subtraction.Text,
                Similarity = subtraction.Similarity
            };

            return SubtractionThirdOrderArithmeticOperatorToken.ReduceNumberHyphenNumber(state, new Dictionary<String, Token>() {
                {"minuend", minuend},
                {"hyphen", hyphen},
                {"subtrahend", subtrahend}
            });
        }

        public static Phrase ReduceNumberHyphenNumber(IFuzzyState state, Dictionary<String, Token> parameters) {
            FloatNumericPrimitiveToken minuend = (FloatNumericPrimitiveToken) parameters["minuend"];
            HyphenPunctuationSyntaxToken hyphen = (HyphenPunctuationSyntaxToken) parameters["hyphen"];
            FloatNumericPrimitiveToken subtrahend = (FloatNumericPrimitiveToken) parameters["subtrahend"];

            return new Phrase() {
                new FloatNumericPrimitiveToken() {
                    Value = minuend.ToFloat() - subtrahend.ToFloat(),
                    Text = String.Format("{0} {1} {2}", minuend.Text, hyphen.Text, subtrahend.Text),
                    Similarity = (minuend.Similarity + hyphen.Similarity + subtrahend.Similarity) / 3.0F
                }
            };
        }
    }
}