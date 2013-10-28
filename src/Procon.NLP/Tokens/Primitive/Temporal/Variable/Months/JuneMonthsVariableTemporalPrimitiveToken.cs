using System;

namespace Procon.Nlp.Tokens.Primitive.Temporal.Variable.Months {
    public class JuneMonthsVariableTemporalPrimitiveToken : MonthMonthsVariableTemporalPrimitiveToken {

        public new static Phrase Parse(IStateNlp state, Phrase phrase) {
            return TokenReflection.CreateDescendants<JuneMonthsVariableTemporalPrimitiveToken>(state, phrase);
        }

        public JuneMonthsVariableTemporalPrimitiveToken() {
            this.Pattern = new DateTimePatternNlp() {
                Rule = TimeType.Definitive,
                Year = DateTime.Now.Month <= 6 ? DateTime.Now.Year : DateTime.Now.Year + 1,
                Month = 6
            };
        }
    }
}
