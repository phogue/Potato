using System;

namespace Procon.Fuzzy.Tokens.Primitive.Temporal.Variable.Days {
    public class SundayDaysVariableTemporalPrimitiveToken : DaysVariableTemporalPrimitiveToken {
        public static Phrase Parse(IFuzzyState state, Phrase phrase) {
            return TokenReflection.CreateDescendants<SundayDaysVariableTemporalPrimitiveToken>(state, phrase);
        }

        public SundayDaysVariableTemporalPrimitiveToken() {
            DateTime dt = DateTime.Now;
            while (dt.DayOfWeek != DayOfWeek.Sunday) {
                dt = dt.AddDays(1);
            }

            this.Pattern = new FuzzyDateTimePattern() {
                Rule = TimeType.Definitive,
                Year = dt.Year,
                Month = dt.Month,
                Day = dt.Day,
                DayOfWeek = DayOfWeek.Sunday
            };
        }
    }
}