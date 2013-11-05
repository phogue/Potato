using System;

namespace Procon.Fuzzy.Tokens.Primitive.Temporal.Variable.Date.Day {
    public class TomorrowDayVariableTemporalPrimitiveToken : DateVariableTemporalPrimitiveToken {
        public new static Phrase Parse(IFuzzyState state, Phrase phrase) {
            return TokenReflection.CreateDescendants<TomorrowDayVariableTemporalPrimitiveToken>(state, phrase);
        }

        public TomorrowDayVariableTemporalPrimitiveToken() {
            DateTime dt = DateTime.Now.AddDays(1);

            this.Pattern = new FuzzyDateTimePattern() {
                Rule = TimeType.Definitive,
                Year = dt.Year,
                Month = dt.Month,
                Day = dt.Day
            };
        }
    }
}