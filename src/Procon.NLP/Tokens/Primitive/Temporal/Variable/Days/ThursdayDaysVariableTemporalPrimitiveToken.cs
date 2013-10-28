using System;

namespace Procon.Nlp.Tokens.Primitive.Temporal.Variable.Days {
    public class ThursdayDaysVariableTemporalPrimitiveToken : DaysVariableTemporalPrimitiveToken {

        public new static Phrase Parse(IStateNlp state, Phrase phrase) {
            return TokenReflection.CreateDescendants<ThursdayDaysVariableTemporalPrimitiveToken>(state, phrase);
        }

        public ThursdayDaysVariableTemporalPrimitiveToken() {

            DateTime dt = DateTime.Now;
            while (dt.DayOfWeek != DayOfWeek.Thursday) {
                dt = dt.AddDays(1);
            }

            this.Pattern = new DateTimePatternNlp() {
                Rule = TimeType.Definitive,
                Year = dt.Year,
                Month = dt.Month,
                Day = dt.Day,
                DayOfWeek = DayOfWeek.Thursday
            };
        }
    }
}
