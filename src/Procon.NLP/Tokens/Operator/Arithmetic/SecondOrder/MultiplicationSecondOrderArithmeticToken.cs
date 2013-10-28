using System;

namespace Procon.Nlp.Tokens.Operator.Arithmetic.SecondOrder {
    using Procon.Nlp.Tokens.Primitive.Numeric;
    using Procon.Nlp.Tokens.Syntax.Typography;

    public class MultiplicationSecondOrderArithmeticToken : SecondOrderArithmeticToken {
        public new static Phrase Parse(IStateNlp state, Phrase phrase) {
            return TokenReflection.CreateDescendants<MultiplicationSecondOrderArithmeticToken>(state, phrase);
        }

        public static Phrase Reduce(IStateNlp state, FloatNumericPrimitiveToken multiplier, MultiplicationSecondOrderArithmeticToken multiply, FloatNumericPrimitiveToken multiplicand) {
            AsteriskTypographySyntaxToken astrix = new AsteriskTypographySyntaxToken() {
                Text = multiply.Text,
                Similarity = multiply.Similarity
            };

            return MultiplicationSecondOrderArithmeticToken.Reduce(state, multiplier, astrix, multiplicand);
        }

        public static Phrase Reduce(IStateNlp state, FloatNumericPrimitiveToken multiplier, AsteriskTypographySyntaxToken asterisk, FloatNumericPrimitiveToken multiplicand) {
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
