using System;
using System.Collections.Generic;

namespace Procon.Nlp.Tokens.Operator.Arithmetic.SecondOrder {
    using Procon.Nlp.Tokens.Primitive.Numeric;
    using Procon.Nlp.Tokens.Syntax.Punctuation;
    public class DivisionSecondOrderArithmeticToken : SecondOrderArithmeticToken {

        public static Phrase Parse(IStateNlp state, Phrase phrase) {
            return TokenReflection.CreateDescendants<DivisionSecondOrderArithmeticToken>(state, phrase);
        }

        public static Phrase ReduceDividendDivideDivisor(IStateNlp state, Dictionary<String, Token> parameters) {
            FloatNumericPrimitiveToken dividend = (FloatNumericPrimitiveToken)parameters["dividend"];
            DivisionSecondOrderArithmeticToken divide = (DivisionSecondOrderArithmeticToken)parameters["divide"];
            FloatNumericPrimitiveToken divisor = (FloatNumericPrimitiveToken)parameters["divisor"];

            ForwardSlashPunctuationSyntaxToken forwardSlash = new ForwardSlashPunctuationSyntaxToken() {
                Text = divide.Text,
                Similarity = divide.Similarity
            };

            return DivisionSecondOrderArithmeticToken.ReduceDividendForwardSlashDivisor(state, new Dictionary<String, Token>() {
                { "dividend", dividend },
                { "forwardSlash", forwardSlash },
                { "divisor", divisor }
            });
        }

        public static Phrase ReduceDividendForwardSlashDivisor(IStateNlp state, Dictionary<String, Token> parameters) {
            FloatNumericPrimitiveToken dividend = (FloatNumericPrimitiveToken)parameters["dividend"];
            ForwardSlashPunctuationSyntaxToken forwardSlash = (ForwardSlashPunctuationSyntaxToken)parameters["forwardSlash"];
            FloatNumericPrimitiveToken divisor = (FloatNumericPrimitiveToken)parameters["divisor"];

            Phrase phrase = null;

            if (divisor.ToFloat() != 0.0F) {
                phrase = new Phrase() {
                    new FloatNumericPrimitiveToken() {
                        Value = dividend.ToFloat() / divisor.ToFloat(),
                        Text = String.Format("{0} {1} {2}", dividend.Text, forwardSlash.Text, divisor.Text),
                        Similarity = (dividend.Similarity + forwardSlash.Similarity + divisor.Similarity) / 3.0F
                    }
                };
            }
            else {
                // TODO: Create new token for exceptions
                phrase = new Phrase() {
                    new FloatNumericPrimitiveToken() {
                        Value = 0.0F,
                        Text = String.Format("{0} {1} {2}", dividend.Text, forwardSlash.Text, divisor.Text),
                        Similarity = (dividend.Similarity + forwardSlash.Similarity + divisor.Similarity) / 3.0F
                    }
                };
            }

            return phrase;
        }
    }
}
