using System;
using System.Collections.Generic;

namespace Procon.Nlp.Tokens.Primitive.Temporal.Variable.Date.Month {
    using Procon.Nlp.Utils;
    using Procon.Nlp.Tokens.Primitive.Temporal.Units;
    using Procon.Nlp.Tokens.Primitive.Temporal.Variable.Months;
    using Procon.Nlp.Tokens.Primitive.Numeric;
    using Procon.Nlp.Tokens.Syntax.Articles;
    using Procon.Nlp.Tokens.Syntax.Adjectives;

    public class MonthVariableTemporalPrimitiveToken : DateVariableTemporalPrimitiveToken {

        public static Phrase ReduceNumberMonths(IStateNlp state, Dictionary<String, Token> parameters) {
            FloatNumericPrimitiveToken number = (FloatNumericPrimitiveToken)parameters["number"];
            MonthsUnitTemporalPrimitiveToken months = (MonthsUnitTemporalPrimitiveToken)parameters["months"];

            return new Phrase() {
                new MonthVariableTemporalPrimitiveToken() {
                    Pattern = new DateTimePatternNlp() {
                        Rule = TimeType.Relative,// Rule = TimeType.Definitive,
                        Month = (int)number.ToFloat().ConvertTo(typeof(int))
                    },
                    Text = String.Format("{0} {1}", number.Text, months.Text),
                    Similarity = (months.Similarity + number.Similarity) / 2.0F
                }
            };
        }

        public static Phrase ReduceOrdinalMonths(IStateNlp state, Dictionary<String, Token> parameters) {
            OrdinalNumericPrimitiveToken ordinal = (OrdinalNumericPrimitiveToken)parameters["ordinal"];
            MonthMonthsVariableTemporalPrimitiveToken months = (MonthMonthsVariableTemporalPrimitiveToken)parameters["months"];

            DateTimePatternNlp pattern = months.Pattern;
            pattern.Day = (int)ordinal.ToFloat().ConvertTo(typeof(int));

            return new Phrase() {
                new MonthVariableTemporalPrimitiveToken() {
                    Pattern = pattern,
                    Text = String.Format("{0} {1}", ordinal.Text, months.Text),
                    Similarity = (months.Similarity + ordinal.Similarity) / 2.0F
                }
            };
        }

        public static Phrase ReduceArticleMonths(IStateNlp state, Dictionary<String, Token> parameters) {
            IndefiniteArticlesSyntaxToken article = (IndefiniteArticlesSyntaxToken)parameters["article"];
            MonthsUnitTemporalPrimitiveToken months = (MonthsUnitTemporalPrimitiveToken)parameters["months"];

            return new Phrase() {
                new MonthVariableTemporalPrimitiveToken() {
                    Pattern = new DateTimePatternNlp() {
                        Rule = TimeType.Relative,
                        Month = 1
                    },
                    Text = String.Format("{0} {1}", article.Text, months.Text),
                    Similarity = (months.Similarity + article.Similarity) / 2.0F
                }
            };
        }

        public static Phrase ReduceNextMonths(IStateNlp state, Dictionary<String, Token> parameters) {
            NextAdjectiveSyntaxToken next = (NextAdjectiveSyntaxToken)parameters["next"];
            MonthsUnitTemporalPrimitiveToken months = (MonthsUnitTemporalPrimitiveToken)parameters["months"];

            return new Phrase() {
                new MonthVariableTemporalPrimitiveToken() {
                    Pattern = new DateTimePatternNlp() {
                        Rule = TimeType.Relative,
                        Month = 1
                    },
                    Text = String.Format("{0} {1}", next.Text, months.Text),
                    Similarity = (months.Similarity + next.Similarity) / 2.0F
                }
            };
        }

        public static Phrase ReduceLastMonths(IStateNlp state, Dictionary<String, Token> parameters) {
            LastAdjectiveSyntaxToken last = (LastAdjectiveSyntaxToken)parameters["last"];
            MonthsUnitTemporalPrimitiveToken months = (MonthsUnitTemporalPrimitiveToken)parameters["months"];

            return new Phrase() {
                new MonthVariableTemporalPrimitiveToken() {
                    Pattern = new DateTimePatternNlp() {
                        Rule = TimeType.Relative,
                        Month = -1
                    },
                    Text = String.Format("{0} {1}", last.Text, months.Text),
                    Similarity = (months.Similarity + last.Similarity) / 2.0F
                }
            };
        }

        public static Phrase ReduceEveryMonths(IStateNlp state, Dictionary<String, Token> parameters) {
            EveryAdjectiveSyntaxToken every = (EveryAdjectiveSyntaxToken)parameters["every"];
            MonthsUnitTemporalPrimitiveToken months = (MonthsUnitTemporalPrimitiveToken)parameters["months"];

            return new Phrase() {
                new MinutesUnitTemporalPrimitiveToken() {
                    Pattern = new DateTimePatternNlp() {
                        Rule = TimeType.Relative,
                        Modifier = TimeModifier.Interval,
                        Month = 1
                    },
                    Text = String.Format("{0} {1}", every.Text, months.Text),
                    Similarity = (months.Similarity + every.Similarity) / 2.0F
                }
            };
        }
    }
}
