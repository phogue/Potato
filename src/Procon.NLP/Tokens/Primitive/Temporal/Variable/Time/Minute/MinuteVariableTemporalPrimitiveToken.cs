using System;

namespace Procon.Nlp.Tokens.Primitive.Temporal.Variable.Time.Minute {
    using Procon.Nlp.Tokens.Primitive.Temporal.Units;
    using Procon.Nlp.Tokens.Primitive.Numeric;
    using Procon.Nlp.Tokens.Syntax.Adjectives;
    using Procon.Nlp.Tokens.Syntax.Articles;
    using Procon.Nlp.Utils;

    public class MinuteVariableTemporalPrimitiveToken : TimeVariableTemporalPrimitiveToken {

        public static Phrase Reduce(IStateNlp state, FloatNumericPrimitiveToken number, MinutesUnitTemporalPrimitiveToken minutes) {
            return new Phrase() {
                new MinuteVariableTemporalPrimitiveToken() {
                    Pattern = new DateTimePatternNlp() {
                        Rule = TimeType.Relative,
                        Minute = (int)number.ToFloat().ConvertTo(typeof(int))
                    },
                    Text = String.Format("{0} {1}", number.Text, minutes.Text),
                    Similarity = (minutes.Similarity + number.Similarity) / 2.0F
                }
            };
        }

        public static Phrase Reduce(IStateNlp state, IndefiniteArticlesSyntaxToken article, MinutesUnitTemporalPrimitiveToken minutes) {
            return new Phrase() {
                new MinuteVariableTemporalPrimitiveToken() {
                    Pattern = new DateTimePatternNlp() {
                        Rule = TimeType.Relative,
                        Minute = 1
                    },
                    Text = String.Format("{0} {1}", article.Text, minutes.Text),
                    Similarity = (minutes.Similarity + article.Similarity) / 2.0F
                }
            };
        }

        public static Phrase Reduce(IStateNlp state, EveryAdjectiveSyntaxToken every, MinutesUnitTemporalPrimitiveToken minutes) {
            return new Phrase() {
                new MinutesUnitTemporalPrimitiveToken() {
                    Pattern = new DateTimePatternNlp() {
                        Rule = TimeType.Relative,
                        Modifier = TimeModifier.Interval,
                        Minute = 1
                    },
                    Text = String.Format("{0} {1}", every.Text, minutes.Text),
                    Similarity = (minutes.Similarity + every.Similarity) / 2.0F
                }
            };
        }
    }
}
