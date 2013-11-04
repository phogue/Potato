using System;

namespace Procon.Nlp.Tokens.Primitive.Temporal.Variable.Months {
    public class SeptemberMonthsVariableTemporalPrimitiveToken : MonthMonthsVariableTemporalPrimitiveToken {

        public static Phrase Parse(IStateNlp state, Phrase phrase) {
            return TokenReflection.CreateDescendants<SeptemberMonthsVariableTemporalPrimitiveToken>(state, phrase);
        }

        public SeptemberMonthsVariableTemporalPrimitiveToken() {
            this.Pattern = new DateTimePatternNlp() {
                Rule = TimeType.Definitive,
                Year = DateTime.Now.Month <= 9 ? DateTime.Now.Year : DateTime.Now.Year + 1,
                Month = 9
            };
        }
    }
}
