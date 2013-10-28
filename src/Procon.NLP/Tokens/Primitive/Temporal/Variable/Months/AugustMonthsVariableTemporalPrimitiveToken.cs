using System;

namespace Procon.Nlp.Tokens.Primitive.Temporal.Variable.Months {
    public class AugustMonthsVariableTemporalPrimitiveToken : MonthMonthsVariableTemporalPrimitiveToken {

        public new static Phrase Parse(IStateNlp state, Phrase phrase) {
            return TokenReflection.CreateDescendants<AugustMonthsVariableTemporalPrimitiveToken>(state, phrase);
        }

        public AugustMonthsVariableTemporalPrimitiveToken() {
            this.Pattern = new DateTimePatternNlp() {
                Rule = TimeType.Definitive,
                Year = DateTime.Now.Month <= 8 ? DateTime.Now.Year : DateTime.Now.Year + 1,
                Month = 8
            };
        }
    }
}
