using System;

namespace Procon.Nlp.Tokens.Operator.Arithmetic.SecondOrder {
    using Procon.Nlp.Tokens.Primitive.Numeric;
    using Procon.Nlp.Tokens.Syntax.Punctuation;
    public class DivisionSecondOrderArithmeticToken : SecondOrderArithmeticToken {

        public new static Phrase Parse(IStateNlp state, Phrase phrase) {
            return TokenReflection.CreateDescendants<DivisionSecondOrderArithmeticToken>(state, phrase);
        }

        public static Phrase Reduce(IStateNlp state, FloatNumericPrimitiveToken dividend, DivisionSecondOrderArithmeticToken divide, FloatNumericPrimitiveToken divisor) {
            ForwardSlashPunctuationSyntaxToken slash = new ForwardSlashPunctuationSyntaxToken() {
                Text = divide.Text,
                Similarity = divide.Similarity
            };

            return DivisionSecondOrderArithmeticToken.Reduce(state, dividend, slash, divisor);
        }

        public static Phrase Reduce(IStateNlp state, FloatNumericPrimitiveToken dividend, ForwardSlashPunctuationSyntaxToken asterisk, FloatNumericPrimitiveToken divisor) {
            Phrase phrase = null;

            if (divisor.ToFloat() != 0.0F) {
                phrase = new Phrase() {
                    new FloatNumericPrimitiveToken() {
                        Value = dividend.ToFloat() / divisor.ToFloat(),
                        Text = String.Format("{0} {1} {2}", dividend.Text, asterisk.Text, divisor.Text),
                        Similarity = (dividend.Similarity + asterisk.Similarity + divisor.Similarity) / 3.0F
                    }
                };
            }
            else {
                // TODO: Create new token for exceptions
                phrase = new Phrase() {
                    new FloatNumericPrimitiveToken() {
                        Value = 0.0F,
                        Text = String.Format("{0} {1} {2}", dividend.Text, asterisk.Text, divisor.Text),
                        Similarity = (dividend.Similarity + asterisk.Similarity + divisor.Similarity) / 3.0F
                    }
                };
            }

            return phrase;
        }
    }
}
