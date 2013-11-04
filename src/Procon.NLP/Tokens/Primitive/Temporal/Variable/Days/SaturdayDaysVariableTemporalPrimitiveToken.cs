using System;

namespace Procon.Nlp.Tokens.Primitive.Temporal.Variable.Days {
    public class SaturdayDaysVariableTemporalPrimitiveToken : DaysVariableTemporalPrimitiveToken {

        public static Phrase Parse(IStateNlp state, Phrase phrase) {
            return TokenReflection.CreateDescendants<SaturdayDaysVariableTemporalPrimitiveToken>(state, phrase);
        }

        public SaturdayDaysVariableTemporalPrimitiveToken() {

            DateTime dt = DateTime.Now;
            while (dt.DayOfWeek != DayOfWeek.Saturday) {
                dt = dt.AddDays(1);
            }

            this.Pattern = new DateTimePatternNlp() {
                Rule = TimeType.Definitive,
                Year = dt.Year,
                Month = dt.Month,
                Day = dt.Day,
                DayOfWeek = DayOfWeek.Saturday
            };
        }
    }
}
