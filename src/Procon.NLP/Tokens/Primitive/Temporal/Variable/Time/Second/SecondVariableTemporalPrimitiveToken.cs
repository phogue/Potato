using System;
using System.Collections.Generic;

namespace Procon.Nlp.Tokens.Primitive.Temporal.Variable.Time.Second {
    using Procon.Nlp.Tokens.Primitive.Temporal.Units;
    using Procon.Nlp.Tokens.Primitive.Numeric;
    using Procon.Nlp.Tokens.Syntax.Adjectives;
    using Procon.Nlp.Tokens.Syntax.Articles;
    using Procon.Nlp.Utils;

    public class SecondVariableTemporalPrimitiveToken : TimeVariableTemporalPrimitiveToken {

        public static Phrase ReduceNumberSeconds(IStateNlp state, Dictionary<String, Token> parameters) {
            FloatNumericPrimitiveToken number = (FloatNumericPrimitiveToken)parameters["number"];
            SecondsUnitTemporalPrimitiveToken seconds = (SecondsUnitTemporalPrimitiveToken)parameters["seconds"];

            return new Phrase() {
                new SecondVariableTemporalPrimitiveToken() {
                    Pattern = new DateTimePatternNlp() {
                        Rule = TimeType.Relative,
                        Second = (int)number.ToFloat().ConvertTo(typeof(int))
                    },
                    Text = String.Format("{0} {1}", number.Text, seconds.Text),
                    Similarity = (seconds.Similarity + number.Similarity) / 2.0F
                }
            };
        }

        public static Phrase ReduceArticleSeconds(IStateNlp state, Dictionary<String, Token> parameters) {
            IndefiniteArticlesSyntaxToken article = (IndefiniteArticlesSyntaxToken)parameters["article"];
            SecondsUnitTemporalPrimitiveToken seconds = (SecondsUnitTemporalPrimitiveToken)parameters["seconds"];

            return new Phrase() {
                new SecondVariableTemporalPrimitiveToken() {
                    Pattern = new DateTimePatternNlp() {
                        Rule = TimeType.Relative,
                        Second = 1
                    },
                    Text = String.Format("{0} {1}", article.Text, seconds.Text),
                    Similarity = (seconds.Similarity + article.Similarity) / 2.0F
                }
            };
        }

        public static Phrase ReduceEverySeconds(IStateNlp state, Dictionary<String, Token> parameters) {
            EveryAdjectiveSyntaxToken every = (EveryAdjectiveSyntaxToken)parameters["every"];
            SecondsUnitTemporalPrimitiveToken seconds = (SecondsUnitTemporalPrimitiveToken)parameters["seconds"];

            return new Phrase() {
                new SecondsUnitTemporalPrimitiveToken() {
                    Pattern = new DateTimePatternNlp() {
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
