using System;

namespace Procon.Nlp.Tokens.Operator.Arithmetic.SecondOrder {
    using Procon.Nlp.Tokens.Primitive.Numeric;
    using Procon.Nlp.Tokens.Syntax.Typography;

    public class PowerSecondOrderArithmeticToken : SecondOrderArithmeticToken {
        public new static Phrase Parse(IStateNlp state, Phrase phrase) {
            return TokenReflection.CreateDescendants<PowerSecondOrderArithmeticToken>(state, phrase);
        }

        public static Phrase Reduce(IStateNlp state, FloatNumericPrimitiveToken dividend, PowerSecondOrderArithmeticToken power, FloatNumericPrimitiveToken divisor) {
            CaretTypographySyntaxToken caret = new CaretTypographySyntaxToken() {
                Text = power.Text,
                Similarity = power.Similarity
            };

            return PowerSecondOrderArithmeticToken.Reduce(state, dividend, caret, divisor);
        }

        public static Phrase Reduce(IStateNlp state, FloatNumericPrimitiveToken multiplier, CaretTypographySyntaxToken power, FloatNumericPrimitiveToken multiplicand) {
            return new Phrase() {
                new FloatNumericPrimitiveToken() {
                    Value = (float)Math.Pow(multiplier.ToFloat(), multiplicand.ToFloat()),
                    Text = String.Format("{0} {1} {2}", multiplier.Text, power.Text, multiplicand.Text),
                    Similarity = (multiplier.Similarity + power.Similarity + multiplicand.Similarity) / 3.0F
                }
            };
        }
    }
}
