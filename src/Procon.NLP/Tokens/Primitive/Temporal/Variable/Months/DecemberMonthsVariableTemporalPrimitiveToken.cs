using System;

namespace Procon.Nlp.Tokens.Primitive.Temporal.Variable.Months {
    public class DecemberMonthsVariableTemporalPrimitiveToken : MonthMonthsVariableTemporalPrimitiveToken {

        public static Phrase Parse(IStateNlp state, Phrase phrase) {
            return TokenReflection.CreateDescendants<DecemberMonthsVariableTemporalPrimitiveToken>(state, phrase);
        }

        public DecemberMonthsVariableTemporalPrimitiveToken() {
            this.Pattern = new DateTimePatternNlp() {
                Rule = TimeType.Definitive,
                Year = DateTime.Now.Month <= 12 ? DateTime.Now.Year : DateTime.Now.Year + 1,
                Month = 12
            };
        }
    }
}
