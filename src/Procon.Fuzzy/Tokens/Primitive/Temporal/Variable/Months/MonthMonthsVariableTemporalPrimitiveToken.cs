using System;
using System.Collections.Generic;

namespace Procon.Fuzzy.Tokens.Primitive.Temporal.Variable.Months {
    using Syntax.Adjectives;

    public class MonthMonthsVariableTemporalPrimitiveToken : DateTimeTemporalPrimitiveToken {
        public static Phrase ReduceAdjectiveMonths(IFuzzyState state, Dictionary<String, Token> parameters) {
            AdjectiveSyntaxToken adjective = (AdjectiveSyntaxToken) parameters["adjective"];
            MonthMonthsVariableTemporalPrimitiveToken months = (MonthMonthsVariableTemporalPrimitiveToken) parameters["months"];

            DateTime newDateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

            if (adjective is LastAdjectiveSyntaxToken) {
                // Today - Maximum of 7
                do {
                    newDateTime = newDateTime.AddMonths(-1);
                } while (newDateTime.Month != months.Pattern.Month);
            }
            else if (adjective is NextAdjectiveSyntaxToken) {
                // Today + Maximum of 7
                do {
                    newDateTime = newDateTime.AddMonths(1);
                } while (newDateTime.Month != months.Pattern.Month);
            }
            else if (adjective is ThisAdjectiveSyntaxToken) {
                // Today or +7
                while (newDateTime.Month != months.Pattern.Month) {
                    newDateTime = newDateTime.AddMonths(1);
                }
            }

            return new Phrase() {
                new MonthMonthsVariableTemporalPrimitiveToken() {
                    Pattern = new FuzzyDateTimePattern() {
                        Year = newDateTime.Year,
                        Month = newDateTime.Month
                    },
                    Text = String.Format("{0} {1}", adjective.Text, months.Text),
                    Similarity = (months.Similarity + adjective.Similarity) / 2.0F
                }
            };
        }
    }
}