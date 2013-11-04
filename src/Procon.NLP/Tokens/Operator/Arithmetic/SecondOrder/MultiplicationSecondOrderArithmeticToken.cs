using System;
using System.Collections.Generic;

namespace Procon.Nlp.Tokens.Operator.Arithmetic.SecondOrder {
    using Procon.Nlp.Tokens.Primitive.Numeric;
    using Procon.Nlp.Tokens.Syntax.Typography;

    public class MultiplicationSecondOrderArithmeticToken : SecondOrderArithmeticToken {
        public static Phrase Parse(IStateNlp state, Phrase phrase) {
            return TokenReflection.CreateDescendants<MultiplicationSecondOrderArithmeticToken>(state, phrase);
        }

        public static Phrase ReduceMultiplierMultiplyMultiplicand(IStateNlp state, Dictionary<String, Token> parameters) {
            FloatNumericPrimitiveToken multiplier = (FloatNumericPrimitiveToken)parameters["multiplier"];
            MultiplicationSecondOrderArithmeticToken multiply = (MultiplicationSecondOrderArithmeticToken)parameters["multiply"];
            FloatNumericPrimitiveToken multiplicand = (FloatNumericPrimitiveToken)parameters["multiplicand"];

            AsteriskTypographySyntaxToken asterisk = new AsteriskTypographySyntaxToken() {
                Text = multiply.Text,
                Similarity = multiply.Similarity
            };

            return MultiplicationSecondOrderArithmeticToken.ReduceMultiplierAsteriskMultiplicand(state, new Dictionary<String, Token>() {
                { "multiplier", multiplier },
                { "asterisk", asterisk },
                { "multiplicand", multiplicand }
            });
        }

        public static Phrase ReduceMultiplierAsteriskMultiplicand(IStateNlp state, Dictionary<String, Token> parameters) {
            FloatNumericPrimitiveToken multiplier = (FloatNumericPrimitiveToken)parameters["multiplier"];
            AsteriskTypographySyntaxToken asterisk = (AsteriskTypographySyntaxToken)parameters["asterisk"];
            FloatNumericPrimitiveToken multiplicand = (FloatNumericPrimitiveToken)parameters["multiplicand"];

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
