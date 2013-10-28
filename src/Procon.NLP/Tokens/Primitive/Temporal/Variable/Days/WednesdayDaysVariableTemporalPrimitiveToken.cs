using System;

namespace Procon.Nlp.Tokens.Primitive.Temporal.Variable.Days {
    public class WednesdayDaysVariableTemporalPrimitiveToken : DaysVariableTemporalPrimitiveToken {

        public new static Phrase Parse(IStateNlp state, Phrase phrase) {
            return TokenReflection.CreateDescendants<WednesdayDaysVariableTemporalPrimitiveToken>(state, phrase);
        }

        public WednesdayDaysVariableTemporalPrimitiveToken() {

            DateTime dt = DateTime.Now;
            while (dt.DayOfWeek != DayOfWeek.Wednesday) {
                dt = dt.AddDays(1);
            }

            this.Pattern = new DateTimePatternNlp() {
                Rule = TimeType.Definitive,
                Year = dt.Year,
                Month = dt.Month,
                Day = dt.Day,
                DayOfWeek = DayOfWeek.Wednesday
            };
        }
    }
}
