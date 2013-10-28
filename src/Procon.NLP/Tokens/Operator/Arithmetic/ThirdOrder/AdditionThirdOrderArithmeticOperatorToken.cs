using System;

namespace Procon.Nlp.Tokens.Operator.Arithmetic.ThirdOrder {
    using Procon.Nlp.Tokens.Primitive.Numeric;
    using Procon.Nlp.Tokens.Syntax.Typography;

    public class AdditionThirdOrderArithmeticOperatorToken : ThirdOrderArithmeticOperatorToken {
        public new static Phrase Parse(IStateNlp state, Phrase phrase) {
            return TokenReflection.CreateDescendants<AdditionThirdOrderArithmeticOperatorToken>(state, phrase);
        }

        public static Phrase Reduce(IStateNlp state, FloatNumericPrimitiveToken addend1, AdditionThirdOrderArithmeticOperatorToken addition, FloatNumericPrimitiveToken addend2) {
            PlusTypographySyntaxToken plus = new PlusTypographySyntaxToken() {
                Text = addition.Text,
                Similarity = addition.Similarity
            };

            return AdditionThirdOrderArithmeticOperatorToken.Reduce(state, addend1, plus, addend2);
        }

        public static Phrase Reduce(IStateNlp state, FloatNumericPrimitiveToken addend1, PlusTypographySyntaxToken plus, FloatNumericPrimitiveToken addend2) {
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
