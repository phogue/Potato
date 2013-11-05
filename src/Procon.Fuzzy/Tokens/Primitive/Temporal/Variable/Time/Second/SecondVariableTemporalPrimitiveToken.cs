using System;
using System.Collections.Generic;

namespace Procon.Fuzzy.Tokens.Primitive.Temporal.Variable.Time.Second {
    using Procon.Fuzzy.Tokens.Primitive.Temporal.Units;
    using Procon.Fuzzy.Tokens.Primitive.Numeric;
    using Procon.Fuzzy.Tokens.Syntax.Adjectives;
    using Procon.Fuzzy.Tokens.Syntax.Articles;
    using Procon.Fuzzy.Utils;

    public class SecondVariableTemporalPrimitiveToken : TimeVariableTemporalPrimitiveToken {
        public static Phrase ReduceNumberSeconds(IFuzzyState state, Dictionary<String, Token> parameters) {
            FloatNumericPrimitiveToken number = (FloatNumericPrimitiveToken) parameters["number"];
            SecondsUnitTemporalPrimitiveToken seconds = (SecondsUnitTemporalPrimitiveToken) parameters["seconds"];

            return new Phrase() {
                new SecondVariableTemporalPrimitiveToken() {
                    Pattern = new FuzzyDateTimePattern() {
                        Rule = TimeType.Relative,
                        Second = (int) number.ToFloat().ConvertTo(typeof (int))
                    },
                    Text = String.Format("{0} {1}", number.Text, seconds.Text),
                    Similarity = (seconds.Similarity + number.Similarity) / 2.0F
                }
            };
        }

        public static Phrase ReduceArticleSeconds(IFuzzyState state, Dictionary<String, Token> parameters) {
            IndefiniteArticlesSyntaxToken article = (IndefiniteArticlesSyntaxToken) parameters["article"];
            SecondsUnitTemporalPrimitiveToken seconds = (SecondsUnitTemporalPrimitiveToken) parameters["seconds"];

            return new Phrase() {
                new SecondVariableTemporalPrimitiveToken() {
                    Pattern = new FuzzyDateTimePattern() {
                        Rule = TimeType.Relative,
                        Second = 1
                    },
                    Text = String.Format("{0} {1}", article.Text, seconds.Text),
                    Similarity = (seconds.Similarity + article.Similarity) / 2.0F
                }
            };
        }

        public static Phrase ReduceEverySeconds(IFuzzyState state, Dictionary<String, Token> parameters) {
            EveryAdjectiveSyntaxToken every = (EveryAdjectiveSyntaxToken) parameters["every"];
            SecondsUnitTemporalPrimitiveToken seconds = (SecondsUnitTemporalPrimitiveToken) parameters["seconds"];

            return new Phrase() {
                new SecondsUnitTemporalPrimitiveToken() {
                    Pattern = new FuzzyDateTimePattern() {
                        Rule = TimeType.Relative,
                        Modifier = TimeModifier.Interval,
                        Second = 1
                    },
                    Text = String.Format("{0} {1}", every.Text, seconds.Text),
                    Similarity = (seconds.Similarity + every.Similarity) / 2.0F
                }
            };
        }
    }
}