using System;
using System.Collections.Generic;

namespace Procon.Nlp.Tokens.Operator.Arithmetic.FirstOrder {
    using Procon.Nlp.Tokens.Primitive.Numeric;
    using Procon.Nlp.Tokens.Operator.Logical;
    using Procon.Nlp.Tokens.Primitive.Numeric.Cardinal;
    using Procon.Nlp.Tokens.Syntax.Punctuation.Parentheses;

    public class FirstOrderArithmeticToken : ArithmeticOperatorToken {

        public static Phrase ReduceOpenParenthesesNumberClosedParentheses(IStateNlp state, Dictionary<String, Token> parameters) {
            OpenParenthesesPunctuationSyntaxToken open = (OpenParenthesesPunctuationSyntaxToken)parameters["open"];
            FloatNumericPrimitiveToken number = (FloatNumericPrimitiveToken)parameters["number"];
            ClosedParenthesesPunctuationSyntaxToken closed = (ClosedParenthesesPunctuationSyntaxToken)parameters["closed"];

            return new Phrase() {
                new FloatNumericPrimitiveToken() {
                    Value = number.ToFloat(),
                    Text = String.Format("{0} {1} {2}", open.Text, number.Text, closed.Text),
                    Similarity = (open.Similarity + number.Similarity + closed.Similarity) / 3.0F
                }
            };
        }

        public static Phrase ReduceMultiplierMultiplicand(IStateNlp state, Dictionary<String, Token> parameters) {
            FloatNumericPrimitiveToken multiplier = (FloatNumericPrimitiveToken)parameters["multiplier"];
            MultiplicandCardinalNumericPrimitiveToken multiplicand = (MultiplicandCardinalNumericPrimitiveToken)parameters["multiplicand"];

            return new Phrase() {
                new FloatNumericPrimitiveToken() {
                    Value = multiplier.ToFloat() * multiplicand.ToFloat(),
                    Text = String.Format("{0} {1}", multiplier.Text, multiplicand.Text),
                    Similarity = (multiplier.Similarity + multiplicand.Similarity) / 2.0F
                }
            };
        }

        public static Phrase ReduceNumberAndNumber(IStateNlp state, Dictionary<String, Token> parameters) {
            FloatNumericPrimitiveToken addend1 = (FloatNumericPrimitiveToken)parameters["addend1"];
            AndLogicalOperatorToken and = (AndLogicalOperatorToken)parameters["and"];
            FloatNumericPrimitiveToken addend2 = (FloatNumericPrimitiveToken)parameters["addend2"];

            return new Phrase() {
                new FloatNumericPrimitiveToken() {
                    Value = addend1.ToFloat() + addend2.ToFloat(),
                    Text = String.Format("{0} {1} {2}", addend1.Text, and.Text, addend2.Text),
                    Similarity = (addend1.Similarity + and.Similarity + addend2.Similarity) / 3.0F
                }
            };
        }

        public static Phrase ReduceNumberNumber(IStateNlp state, Dictionary<String, Token> parameters) {
            FloatNumericPrimitiveToken addend1 = (FloatNumericPrimitiveToken)parameters["addend1"];
            FloatNumericPrimitiveToken addend2 = (FloatNumericPrimitiveToken)parameters["addend2"];

            return new Phrase() {
                new FloatNumericPrimitiveToken() {
                    Value = addend1.ToFloat() + addend2.ToFloat(),
                    Text = String.Format("{0} {1}", addend1.Text, addend2.Text),
                    Similarity = (addend1.Similarity + addend2.Similarity) / 2.0F
                }
            };
        }

    }
}
