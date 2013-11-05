using System;

namespace Procon.Fuzzy.Tokens.Primitive.Temporal.Variable.Days {
    public class MondayDaysVariableTemporalPrimitiveToken : DaysVariableTemporalPrimitiveToken {
        public static Phrase Parse(IFuzzyState state, Phrase phrase) {
            return TokenReflection.CreateDescendants<MondayDaysVariableTemporalPrimitiveToken>(state, phrase);
        }

        public MondayDaysVariableTemporalPrimitiveToken() {
            DateTime dt = DateTime.Now;
            while (dt.DayOfWeek != DayOfWeek.Monday) {
                dt = dt.AddDays(1);
            }

            this.Pattern = new FuzzyDateTimePattern() {
                Rule = TimeType.Definitive,
                Year = dt.Year,
                Month = dt.Month,
                Day = dt.Day,
                DayOfWeek = DayOfWeek.Monday
            };
        }
    }
}