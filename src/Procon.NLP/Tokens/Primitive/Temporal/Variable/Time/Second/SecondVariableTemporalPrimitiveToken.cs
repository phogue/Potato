using System;

namespace Procon.Nlp.Tokens.Primitive.Temporal.Variable.Time.Second {
    using Procon.Nlp.Tokens.Primitive.Temporal.Units;
    using Procon.Nlp.Tokens.Primitive.Numeric;
    using Procon.Nlp.Tokens.Syntax.Adjectives;
    using Procon.Nlp.Tokens.Syntax.Articles;
    using Procon.Nlp.Utils;

    public class SecondVariableTemporalPrimitiveToken : TimeVariableTemporalPrimitiveToken {

        public static Phrase Reduce(IStateNlp state, FloatNumericPrimitiveToken number, SecondsUnitTemporalPrimitiveToken seconds) {
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

        public static Phrase Reduce(IStateNlp state, IndefiniteArticlesSyntaxToken article, SecondsUnitTemporalPrimitiveToken seconds) {
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

        public static Phrase Reduce(IStateNlp state, EveryAdjectiveSyntaxToken every, SecondsUnitTemporalPrimitiveToken seconds) {
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
