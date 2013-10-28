using System;

namespace Procon.Nlp.Tokens.Primitive.Temporal.Variable.Date.Day {
    using Procon.Nlp.Utils;
    using Procon.Nlp.Tokens.Primitive.Temporal.Units;
    using Procon.Nlp.Tokens.Primitive.Numeric;
    using Procon.Nlp.Tokens.Syntax.Articles;

    public class DayVariableTemporalPrimitiveToken : DateVariableTemporalPrimitiveToken {

        public static Phrase Reduce(IStateNlp state, FloatNumericPrimitiveToken number, DaysUnitTemporalPrimitiveToken days) {
            return new Phrase() {
                new DayVariableTemporalPrimitiveToken() {
                    Pattern = new DateTimePatternNlp() {
                        Rule = TimeType.Relative,
                        Day = (int)number.ToFloat().ConvertTo(typeof(int))
                    },
                    Text = String.Format("{0} {1}", number.Text, days.Text),
                    Similarity = (days.Similarity + number.Similarity) / 2.0F
                }
            };
        }

        public static Phrase Reduce(IStateNlp state, FloatNumericPrimitiveToken number, WeeksUnitTemporalPrimitiveToken weeks) {
            return new Phrase() {
                new DayVariableTemporalPrimitiveToken() {
                    Pattern = new DateTimePatternNlp() {
                        Rule = TimeType.Relative,
                        Day = (int)number.ToFloat().ConvertTo(typeof(int)) * 7
                    },
                    Text = String.Format("{0} {1}", number.Text, weeks.Text),
                    Similarity = (weeks.Similarity + number.Similarity) / 2.0F
                }
            };
        }

        public static Phrase Reduce(IStateNlp state, IndefiniteArticlesSyntaxToken article, DaysUnitTemporalPrimitiveToken days) {
            return new Phrase() {
                new DayVariableTemporalPrimitiveToken() {
                    Pattern = new DateTimePatternNlp() {
                        Rule = TimeType.Relative,
                        Day = 1
                    },
                    Text = String.Format("{0} {1}", article.Text, days.Text),
                    Similarity = (days.Similarity + article.Similarity) / 2.0F
                }
            };
        }

        public static Phrase Reduce(IStateNlp state, IndefiniteArticlesSyntaxToken article, WeeksUnitTemporalPrimitiveToken weeks) {
            return new Phrase() {
                new DayVariableTemporalPrimitiveToken() {
                    Pattern = new DateTimePatternNlp() {
                        Rule = TimeType.Relative,
                        Day = 7
                    },
                    Text = String.Format("{0} {1}", article.Text, weeks.Text),
                    Similarity = (weeks.Similarity + article.Similarity) / 2.0F
                }
            };
        }
    }
}
