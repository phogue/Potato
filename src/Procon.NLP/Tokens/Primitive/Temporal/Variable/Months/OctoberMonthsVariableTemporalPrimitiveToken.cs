using System;

namespace Procon.Nlp.Tokens.Primitive.Temporal.Variable.Months {
    public class OctoberMonthsVariableTemporalPrimitiveToken : MonthMonthsVariableTemporalPrimitiveToken {

        public static Phrase Parse(IStateNlp state, Phrase phrase) {
            return TokenReflection.CreateDescendants<OctoberMonthsVariableTemporalPrimitiveToken>(state, phrase);
        }

        public OctoberMonthsVariableTemporalPrimitiveToken() {
            this.Pattern = new DateTimePatternNlp() {
                Rule = TimeType.Definitive,
                Year = DateTime.Now.Month <= 10 ? DateTime.Now.Year : DateTime.Now.Year + 1,
                Month = 10
            };
        }
    }
}
