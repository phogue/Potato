using System;

namespace Procon.Nlp.Tokens.Primitive.Temporal.Variable.Date.Day {
    public class TomorrowDayVariableTemporalPrimitiveToken : DateVariableTemporalPrimitiveToken {

        public new static Phrase Parse(IStateNlp state, Phrase phrase) {
            return TokenReflection.CreateDescendants<TomorrowDayVariableTemporalPrimitiveToken>(state, phrase);
        }

        public TomorrowDayVariableTemporalPrimitiveToken() {
            DateTime dt = DateTime.Now.AddDays(1);

            this.Pattern = new DateTimePatternNlp() {
                Rule = TimeType.Definitive,
                Year = dt.Year,
                Month = dt.Month,
                Day = dt.Day
            };
        }
    }
}
