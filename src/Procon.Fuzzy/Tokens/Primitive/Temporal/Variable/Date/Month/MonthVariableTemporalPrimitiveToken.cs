using System;
using System.Collections.Generic;

namespace Procon.Fuzzy.Tokens.Primitive.Temporal.Variable.Date.Month {
    using Procon.Fuzzy.Utils;
    using Procon.Fuzzy.Tokens.Primitive.Temporal.Units;
    using Procon.Fuzzy.Tokens.Primitive.Temporal.Variable.Months;
    using Procon.Fuzzy.Tokens.Primitive.Numeric;
    using Procon.Fuzzy.Tokens.Syntax.Articles;
    using Procon.Fuzzy.Tokens.Syntax.Adjectives;

    public class MonthVariableTemporalPrimitiveToken : DateVariableTemporalPrimitiveToken {
        public static Phrase ReduceNumberMonths(IFuzzyState state, Dictionary<String, Token> parameters) {
            FloatNumericPrimitiveToken number = (FloatNumericPrimitiveToken) parameters["number"];
            MonthsUnitTemporalPrimitiveToken months = (MonthsUnitTemporalPrimitiveToken) parameters["months"];

            return new Phrase() {
                new MonthVariableTemporalPrimitiveToken() {
                    Pattern = new FuzzyDateTimePattern() {
                        Rule = TimeType.Relative, // Rule = TimeType.Definitive,
                        Month = (int) number.ToFloat().ConvertTo(typeof (int))
                    },
                    Text = String.Format("{0} {1}", number.Text, months.Text),
                    Similarity = (months.Similarity + number.Similarity) / 2.0F
                }
            };
        }

        public static Phrase ReduceOrdinalMonths(IFuzzyState state, Dictionary<String, Token> parameters) {
            OrdinalNumericPrimitiveToken ordinal = (OrdinalNumericPrimitiveToken) parameters["ordinal"];
            MonthMonthsVariableTemporalPrimitiveToken months = (MonthMonthsVariableTemporalPrimitiveToken) parameters["months"];

            FuzzyDateTimePattern pattern = months.Pattern;
            pattern.Day = (int) ordinal.ToFloat().ConvertTo(typeof (int));

            return new Phrase() {
                new MonthVariableTemporalPrimitiveToken() {
                    Pattern = pattern,
                    Text = String.Format("{0} {1}", ordinal.Text, months.Text),
                    Similarity = (months.Similarity + ordinal.Similarity) / 2.0F
                }
            };
        }

        public static Phrase ReduceArticleMonths(IFuzzyState state, Dictionary<String, Token> parameters) {
            IndefiniteArticlesSyntaxToken article = (IndefiniteArticlesSyntaxToken) parameters["article"];
            MonthsUnitTemporalPrimitiveToken months = (MonthsUnitTemporalPrimitiveToken) parameters["months"];

            return new Phrase() {
                new MonthVariableTemporalPrimitiveToken() {
                    Pattern = new FuzzyDateTimePattern() {
                        Rule = TimeType.Relative,
                        Month = 1
                    },
                    Text = String.Format("{0} {1}", article.Text, months.Text),
                    Similarity = (months.Similarity + article.Similarity) / 2.0F
                }
            };
        }

        public static Phrase ReduceNextMonths(IFuzzyState state, Dictionary<String, Token> parameters) {
            NextAdjectiveSyntaxToken next = (NextAdjectiveSyntaxToken) parameters["next"];
            MonthsUnitTemporalPrimitiveToken months = (MonthsUnitTemporalPrimitiveToken) parameters["months"];

            return new Phrase() {
                new MonthVariableTemporalPrimitiveToken() {
                    Pattern = new FuzzyDateTimePattern() {
                        Rule = TimeType.Relative,
                        Month = 1
                    },
                    Text = String.Format("{0} {1}", next.Text, months.Text),
                    Similarity = (months.Similarity + next.Similarity) / 2.0F
                }
            };
        }

        public static Phrase ReduceLastMonths(IFuzzyState state, Dictionary<String, Token> parameters) {
            LastAdjectiveSyntaxToken last = (LastAdjectiveSyntaxToken) parameters["last"];
            MonthsUnitTemporalPrimitiveToken months = (MonthsUnitTemporalPrimitiveToken) parameters["months"];

            return new Phrase() {
                new MonthVariableTemporalPrimitiveToken() {
                    Pattern = new FuzzyDateTimePattern() {
                        Rule = TimeType.Relative,
                        Month = -1
                    },
                    Text = String.Format("{0} {1}", last.Text, months.Text),
                    Similarity = (months.Similarity + last.Similarity) / 2.0F
                }
            };
        }

        public static Phrase ReduceEveryMonths(IFuzzyState state, Dictionary<String, Token> parameters) {
            EveryAdjectiveSyntaxToken every = (EveryAdjectiveSyntaxToken) parameters["every"];
            MonthsUnitTemporalPrimitiveToken months = (MonthsUnitTemporalPrimitiveToken) parameters["months"];

            return new Phrase() {
                new MinutesUnitTemporalPrimitiveToken() {
                    Pattern = new FuzzyDateTimePattern() {
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