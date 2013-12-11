using System;
using System.Collections.Generic;
using Procon.Fuzzy.Tokens.Primitive.Numeric;
using Procon.Fuzzy.Tokens.Syntax.Punctuation;

namespace Procon.Fuzzy.Tokens.Operator.Arithmetic.ThirdOrder {

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