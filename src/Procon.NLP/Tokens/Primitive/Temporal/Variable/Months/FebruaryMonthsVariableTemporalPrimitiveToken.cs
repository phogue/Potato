using System;

namespace Procon.Nlp.Tokens.Primitive.Temporal.Variable.Months {
    public class FebruaryMonthsVariableTemporalPrimitiveToken : MonthMonthsVariableTemporalPrimitiveToken {

        public new static Phrase Parse(IStateNlp state, Phrase phrase) {
            return TokenReflection.CreateDescendants<FebruaryMonthsVariableTemporalPrimitiveToken>(state, phrase);
        }

        public FebruaryMonthsVariableTemporalPrimitiveToken() {
            this.Pattern = new DateTimePatternNlp() {
                Rule = TimeType.Definitive,
                Year = DateTime.Now.Month <= 2 ? DateTime.Now.Year : DateTime.Now.Year + 1,
                Month = 2
            };
        }
    }
}
