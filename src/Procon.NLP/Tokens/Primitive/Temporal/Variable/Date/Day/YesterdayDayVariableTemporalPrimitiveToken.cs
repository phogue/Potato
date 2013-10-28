using System;

namespace Procon.Nlp.Tokens.Primitive.Temporal.Variable.Date.Day {
    public class YesterdayDayVariableTemporalPrimitiveToken : DateVariableTemporalPrimitiveToken {

        public new static Phrase Parse(IStateNlp state, Phrase phrase) {
            return TokenReflection.CreateDescendants<YesterdayDayVariableTemporalPrimitiveToken>(state, phrase);
        }

        public YesterdayDayVariableTemporalPrimitiveToken() {
            DateTime dt = DateTime.Now.AddDays(-1);

            this.Pattern = new DateTimePatternNlp() {
                Rule = TimeType.Definitive,
                Year = dt.Year,
                Month = dt.Month,
                Day = dt.Day
            };
        }
    }
}
