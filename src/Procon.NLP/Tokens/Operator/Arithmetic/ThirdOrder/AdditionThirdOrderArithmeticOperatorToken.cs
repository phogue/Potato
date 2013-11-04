using System;
using System.Collections.Generic;

namespace Procon.Nlp.Tokens.Operator.Arithmetic.ThirdOrder {
    using Procon.Nlp.Tokens.Primitive.Numeric;
    using Procon.Nlp.Tokens.Syntax.Typography;

    public class AdditionThirdOrderArithmeticOperatorToken : ThirdOrderArithmeticOperatorToken {
        public static Phrase Parse(IStateNlp state, Phrase phrase) {
            return TokenReflection.CreateDescendants<AdditionThirdOrderArithmeticOperatorToken>(state, phrase);
        }

        public static Phrase ReduceNumberAdditionNumber(IStateNlp state, Dictionary<String, Token> parameters) {
            FloatNumericPrimitiveToken addend1 = (FloatNumericPrimitiveToken)parameters["addend1"];
            AdditionThirdOrderArithmeticOperatorToken addition = (AdditionThirdOrderArithmeticOperatorToken)parameters["addition"];
            FloatNumericPrimitiveToken addend2 = (FloatNumericPrimitiveToken)parameters["addend2"];

            PlusTypographySyntaxToken plus = new PlusTypographySyntaxToken() {
                Text = addition.Text,
                Similarity = addition.Similarity
            };

            return AdditionThirdOrderArithmeticOperatorToken.ReduceNumberPlusNumber(state, new Dictionary<String, Token>() {
                { "addend1", addend1 },
                { "plus", plus },
                { "addend2", addend2 }
            });
        }

        public static Phrase ReduceNumberPlusNumber(IStateNlp state, Dictionary<String, Token> parameters) {
            FloatNumericPrimitiveToken addend1 = (FloatNumericPrimitiveToken)parameters["addend1"];
            PlusTypographySyntaxToken plus = (PlusTypographySyntaxToken)parameters["plus"];
            FloatNumericPrimitiveToken addend2 = (FloatNumericPrimitiveToken)parameters["addend2"];
            
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
