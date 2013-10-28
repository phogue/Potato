using System;

namespace Procon.Nlp.Tokens.Primitive.Temporal.Variable.Months {
    using Syntax.Adjectives;

    public class MonthMonthsVariableTemporalPrimitiveToken : DateTimeTemporalPrimitiveToken {

        public static Phrase Reduce(IStateNlp state, AdjectiveSyntaxToken adjective, MonthMonthsVariableTemporalPrimitiveToken month) {

            DateTime newDateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

            if (adjective is LastAdjectiveSyntaxToken) {
                // Today - Maximum of 7
                do {
                    newDateTime = newDateTime.AddMonths(-1);
                } while (newDateTime.Month != month.Pattern.Month);
            }
            else if (adjective is NextAdjectiveSyntaxToken) {
                // Today + Maximum of 7
                do {
                    newDateTime = newDateTime.AddMonths(1);
                } while (newDateTime.Month != month.Pattern.Month);
            }
            else if (adjective is ThisAdjectiveSyntaxToken) {
                // Today or +7
                while (newDateTime.Month != month.Pattern.Month) {
                    newDateTime = newDateTime.AddMonths(1);
                }
            }

            return new Phrase() {
                new MonthMonthsVariableTemporalPrimitiveToken() {
                    Pattern = new DateTimePatternNlp() {
                        Year = newDateTime.Year,
                        Month = newDateTime.Month
                    },
                    Text = String.Format("{0} {1}", adjective.Text, month.Text),
                    Similarity = (month.Similarity + adjective.Similarity) / 2.0F
                }
            };
        }
    }
}
