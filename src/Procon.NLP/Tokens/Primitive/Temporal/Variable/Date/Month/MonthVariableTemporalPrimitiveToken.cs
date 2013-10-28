using System;

namespace Procon.Nlp.Tokens.Primitive.Temporal.Variable.Date.Month {
    using Procon.Nlp.Utils;
    using Procon.Nlp.Tokens.Primitive.Temporal.Units;
    using Procon.Nlp.Tokens.Primitive.Temporal.Variable.Months;
    using Procon.Nlp.Tokens.Primitive.Numeric;
    using Procon.Nlp.Tokens.Syntax.Articles;
    using Procon.Nlp.Tokens.Syntax.Adjectives;

    public class MonthVariableTemporalPrimitiveToken : DateVariableTemporalPrimitiveToken {

        public static Phrase Reduce(IStateNlp state, FloatNumericPrimitiveToken number, MonthsUnitTemporalPrimitiveToken month) {
            return new Phrase() {
                new MonthVariableTemporalPrimitiveToken() {
                    Pattern = new DateTimePatternNlp() {
                        Rule = TimeType.Relative,// Rule = TimeType.Definitive,
                        Month = (int)number.ToFloat().ConvertTo(typeof(int))
                    },
                    Text = String.Format("{0} {1}", number.Text, month.Text),
                    Similarity = (month.Similarity + number.Similarity) / 2.0F
                }
            };
        }

        public static Phrase Reduce(IStateNlp state, OrdinalNumericPrimitiveToken ordinal, MonthMonthsVariableTemporalPrimitiveToken month) {

            DateTimePatternNlp pattern = month.Pattern;
            pattern.Day = (int)ordinal.ToFloat().ConvertTo(typeof(int));

            return new Phrase() {
                new MonthVariableTemporalPrimitiveToken() {
                    Pattern = pattern,
                    Text = String.Format("{0} {1}", ordinal.Text, month.Text),
                    Similarity = (month.Similarity + ordinal.Similarity) / 2.0F
                }
            };
        }

        public static Phrase Reduce(IStateNlp state, IndefiniteArticlesSyntaxToken article, MonthsUnitTemporalPrimitiveToken month) {
            return new Phrase() {
                new MonthVariableTemporalPrimitiveToken() {
                    Pattern = new DateTimePatternNlp() {
                        Rule = TimeType.Relative,
                        Month = 1
                    },
                    Text = String.Format("{0} {1}", article.Text, month.Text),
                    Similarity = (month.Similarity + article.Similarity) / 2.0F
                }
            };
        }

        public static Phrase Reduce(IStateNlp state, NextAdjectiveSyntaxToken next, MonthsUnitTemporalPrimitiveToken month) {
            return new Phrase() {
                new MonthVariableTemporalPrimitiveToken() {
                    Pattern = new DateTimePatternNlp() {
                        Rule = TimeType.Relative,
                        Month = 1
                    },
                    Text = String.Format("{0} {1}", next.Text, month.Text),
                    Similarity = (month.Similarity + next.Similarity) / 2.0F
                }
            };
        }

        public static Phrase Reduce(IStateNlp state, LastAdjectiveSyntaxToken last, MonthsUnitTemporalPrimitiveToken month) {
            return new Phrase() {
                new MonthVariableTemporalPrimitiveToken() {
                    Pattern = new DateTimePatternNlp() {
                        Rule = TimeType.Relative,
                        Month = -1
                    },
                    Text = String.Format("{0} {1}", last.Text, month.Text),
                    Similarity = (month.Similarity + last.Similarity) / 2.0F
                }
            };
        }

        public static Phrase Reduce(IStateNlp state, EveryAdjectiveSyntaxToken every, MonthsUnitTemporalPrimitiveToken month) {
            return new Phrase() {
                new MinutesUnitTemporalPrimitiveToken() {
                    Pattern = new DateTimePatternNlp() {
                        Rule = TimeType.Relative,
                        Modifier = TimeModifier.Interval,
                        Month = 1
                    },
                    Text = String.Format("{0} {1}", every.Text, month.Text),
                    Similarity = (month.Similarity + every.Similarity) / 2.0F
                }
            };
        }
    }
}
