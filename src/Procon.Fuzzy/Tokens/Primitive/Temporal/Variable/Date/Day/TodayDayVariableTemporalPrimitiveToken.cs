using System;

namespace Procon.Fuzzy.Tokens.Primitive.Temporal.Variable.Date.Day {
    public class TodayDayVariableTemporalPrimitiveToken : DateVariableTemporalPrimitiveToken {
        public new static Phrase Parse(IFuzzyState state, Phrase phrase) {
            return TokenReflection.CreateDescendants<TodayDayVariableTemporalPrimitiveToken>(state, phrase);
        }

        public TodayDayVariableTemporalPrimitiveToken() {
            this.Pattern = new FuzzyDateTimePattern() {
                Rule = TimeType.Definitive,
                Year = DateTime.Now.Year,
                Month = DateTime.Now.Month,
                Day = DateTime.Now.Day
            };
        }
    }
}