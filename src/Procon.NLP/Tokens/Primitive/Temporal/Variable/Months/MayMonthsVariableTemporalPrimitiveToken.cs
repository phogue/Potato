using System;

namespace Procon.Nlp.Tokens.Primitive.Temporal.Variable.Months {
    public class MayMonthsVariableTemporalPrimitiveToken : MonthMonthsVariableTemporalPrimitiveToken {

        public static Phrase Parse(IStateNlp state, Phrase phrase) {
            return TokenReflection.CreateDescendants<MayMonthsVariableTemporalPrimitiveToken>(state, phrase);
        }

        public MayMonthsVariableTemporalPrimitiveToken() {
            this.Pattern = new DateTimePatternNlp() {
                Rule = TimeType.Definitive,
                Year = DateTime.Now.Month <= 5 ? DateTime.Now.Year : DateTime.Now.Year + 1,
                Month = 5
            };
        }
    }
}
