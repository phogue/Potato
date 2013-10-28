using System;

namespace Procon.Nlp.Tokens.Primitive.Temporal.Variable.Months {
    public class AprilMonthsVariableTemporalPrimitiveToken : MonthMonthsVariableTemporalPrimitiveToken {

        public new static Phrase Parse(IStateNlp state, Phrase phrase) {
            return TokenReflection.CreateDescendants<AprilMonthsVariableTemporalPrimitiveToken>(state, phrase);
        }

        public AprilMonthsVariableTemporalPrimitiveToken() {
            this.Pattern = new DateTimePatternNlp() {
                Rule = TimeType.Definitive,
                Year = DateTime.Now.Month <= 4 ? DateTime.Now.Year : DateTime.Now.Year + 1,
                Month = 4
            };
        }
    }
}
