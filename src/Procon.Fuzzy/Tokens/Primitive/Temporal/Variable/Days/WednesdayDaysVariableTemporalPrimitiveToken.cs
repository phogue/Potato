using System;

namespace Procon.Fuzzy.Tokens.Primitive.Temporal.Variable.Days {
    public class WednesdayDaysVariableTemporalPrimitiveToken : DaysVariableTemporalPrimitiveToken {
        public static Phrase Parse(IFuzzyState state, Phrase phrase) {
            return TokenReflection.CreateDescendants<WednesdayDaysVariableTemporalPrimitiveToken>(state, phrase);
        }

        public WednesdayDaysVariableTemporalPrimitiveToken() {
            DateTime dt = DateTime.Now;
            while (dt.DayOfWeek != DayOfWeek.Wednesday) {
                dt = dt.AddDays(1);
            }

            this.Pattern = new FuzzyDateTimePattern() {
                Rule = TimeType.Definitive,
                Year = dt.Year,
                Month = dt.Month,
                Day = dt.Day,
                DayOfWeek = DayOfWeek.Wednesday
            };
        }
    }
}