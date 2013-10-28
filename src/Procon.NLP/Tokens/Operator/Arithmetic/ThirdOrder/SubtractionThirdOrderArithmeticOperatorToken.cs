using System;

namespace Procon.Nlp.Tokens.Operator.Arithmetic.ThirdOrder {
    using Procon.Nlp.Tokens.Primitive.Numeric;
    using Procon.Nlp.Tokens.Syntax.Punctuation;

    public class SubtractionThirdOrderArithmeticOperatorToken : ThirdOrderArithmeticOperatorToken {

        public new static Phrase Parse(IStateNlp state, Phrase phrase) {
            return TokenReflection.CreateDescendants<SubtractionThirdOrderArithmeticOperatorToken>(state, phrase);
        }

        public static Phrase Reduce(IStateNlp state, FloatNumericPrimitiveToken dividend, SubtractionThirdOrderArithmeticOperatorToken subtraction, FloatNumericPrimitiveToken divisor) {
            HyphenPunctuationSyntaxToken subtract = new HyphenPunctuationSyntaxToken() {
                Text = subtraction.Text,
                Similarity = subtraction.Similarity
            };

            return SubtractionThirdOrderArithmeticOperatorToken.Reduce(state, dividend, subtract, divisor);
        }

        public static Phrase Reduce(IStateNlp state, FloatNumericPrimitiveToken minuend, HyphenPunctuationSyntaxToken subtract, FloatNumericPrimitiveToken subtrahend) {
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
