using System;
using System.Collections.Generic;
using Procon.Fuzzy.Tokens.Primitive.Numeric;
using Procon.Fuzzy.Tokens.Syntax.Typography;

namespace Procon.Fuzzy.Tokens.Operator.Arithmetic.SecondOrder {

    public class PowerSecondOrderArithmeticToken : SecondOrderArithmeticToken {
        public static Phrase Parse(IFuzzyState state, Phrase phrase) {
            return TokenReflection.CreateDescendants<PowerSecondOrderArithmeticToken>(state, phrase);
        }

        public static Phrase ReduceMultiplierPowerMultiplicand(IFuzzyState state, Dictionary<String, Token> parameters) {
            FloatNumericPrimitiveToken multiplier = (FloatNumericPrimitiveToken) parameters["multiplier"];
            PowerSecondOrderArithmeticToken power = (PowerSecondOrderArithmeticToken) parameters["power"];
            FloatNumericPrimitiveToken multiplicand = (FloatNumericPrimitiveToken) parameters["multiplicand"];

            CaretTypographySyntaxToken caret = new CaretTypographySyntaxToken() {
                Text = power.Text,
                Similarity = power.Similarity
            };

            return PowerSecondOrderArithmeticToken.ReduceMultiplierCaretMultiplicand(state, new Dictionary<String, Token>() {
                {"multiplier", multiplier},
                {"caret", caret},
                {"multiplicand", multiplicand}
            });
        }

        public static Phrase ReduceMultiplierCaretMultiplicand(IFuzzyState state, Dictionary<String, Token> parameters) {
            FloatNumericPrimitiveToken multiplier = (FloatNumericPrimitiveToken) parameters["multiplier"];
            CaretTypographySyntaxToken caret = (CaretTypographySyntaxToken) parameters["caret"];
            FloatNumericPrimitiveToken multiplicand = (FloatNumericPrimitiveToken) parameters["multiplicand"];

            return new Phrase() {
                new FloatNumericPrimitiveToken() {
                    Value = (float) Math.Pow(multiplier.ToFloat(), multiplicand.ToFloat()),
                    Text = String.Format("{0} {1} {2}", multiplier.Text, caret.Text, multiplicand.Text),
                    Similarity = (multiplier.Similarity + caret.Similarity + multiplicand.Similarity) / 3.0F
                }
            };
        }
    }
}