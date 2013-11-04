using System;

namespace Procon.Nlp.Tokens.Primitive.Temporal.Variable.Days {
    public class MondayDaysVariableTemporalPrimitiveToken : DaysVariableTemporalPrimitiveToken {

        public static Phrase Parse(IStateNlp state, Phrase phrase) {
            return TokenReflection.CreateDescendants<MondayDaysVariableTemporalPrimitiveToken>(state, phrase);
        }

        public MondayDaysVariableTemporalPrimitiveToken() {

            DateTime dt = DateTime.Now;
            while (dt.DayOfWeek != DayOfWeek.Monday) {
                dt = dt.AddDays(1);
            }

            this.Pattern = new DateTimePatternNlp() {
                Rule = TimeType.Definitive,
                Year = dt.Year,
                Month = dt.Month,
                Day = dt.Day,
                DayOfWeek = DayOfWeek.Monday
            };
        }
    }
}
