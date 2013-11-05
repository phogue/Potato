using System;

namespace Procon.Fuzzy.Tokens.Primitive.Temporal.Variable.Days {
    public class SaturdayDaysVariableTemporalPrimitiveToken : DaysVariableTemporalPrimitiveToken {
        public static Phrase Parse(IFuzzyState state, Phrase phrase) {
            return TokenReflection.CreateDescendants<SaturdayDaysVariableTemporalPrimitiveToken>(state, phrase);
        }

        public SaturdayDaysVariableTemporalPrimitiveToken() {
            DateTime dt = DateTime.Now;
            while (dt.DayOfWeek != DayOfWeek.Saturday) {
                dt = dt.AddDays(1);
            }

            this.Pattern = new FuzzyDateTimePattern() {
                Rule = TimeType.Definitive,
                Year = dt.Year,
                Month = dt.Month,
                Day = dt.Day,
                DayOfWeek = DayOfWeek.Saturday
            };
        }
    }
}