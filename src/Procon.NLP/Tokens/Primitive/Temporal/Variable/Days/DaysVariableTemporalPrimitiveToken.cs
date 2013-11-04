using System;
using System.Collections.Generic;

namespace Procon.Nlp.Tokens.Primitive.Temporal.Variable.Days {
    using Syntax.Adjectives;

    public class DaysVariableTemporalPrimitiveToken : DateTimeTemporalPrimitiveToken {
        public static Phrase ReduceAdjectiveDays(IStateNlp state, Dictionary<String, Token> parameters) { 
            AdjectiveSyntaxToken adjective = (AdjectiveSyntaxToken)parameters["adjective"];
            DaysVariableTemporalPrimitiveToken days = (DaysVariableTemporalPrimitiveToken)parameters["days"];


            DateTime newDateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

            if (adjective is LastAdjectiveSyntaxToken) {
                // Today - Maximum of 7
                do {
                    newDateTime = newDateTime.AddDays(-1);
                } while (newDateTime.DayOfWeek != days.Pattern.DayOfWeek);
            }
            else if (adjective is NextAdjectiveSyntaxToken) {
                // Today + Maximum of 7
                do {
                    newDateTime = newDateTime.AddDays(1);
                } while (newDateTime.DayOfWeek != days.Pattern.DayOfWeek);
            }
            else if (adjective is ThisAdjectiveSyntaxToken) {
                // Today or +7
                while (newDateTime.DayOfWeek != days.Pattern.DayOfWeek) {
                    newDateTime = newDateTime.AddDays(1);
                }
            }

            return new Phrase() {
                new DaysVariableTemporalPrimitiveToken() {
                    Pattern = new DateTimePatternNlp() {
                        Rule = TimeType.Definitive,
                        Year = newDateTime.Year,
                        Month = newDateTime.Month,
                        Day = newDateTime.Day
                    },
                    Text = String.Format("{0} {1}", adjective.Text, days.Text),
                    Similarity = (days.Similarity + adjective.Similarity) / 2.0F
                }
            };
        }
    }
}
