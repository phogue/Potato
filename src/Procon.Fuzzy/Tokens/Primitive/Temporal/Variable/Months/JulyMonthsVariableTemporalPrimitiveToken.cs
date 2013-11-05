using System;

namespace Procon.Fuzzy.Tokens.Primitive.Temporal.Variable.Months {
    public class JulyMonthsVariableTemporalPrimitiveToken : MonthMonthsVariableTemporalPrimitiveToken {
        public static Phrase Parse(IFuzzyState state, Phrase phrase) {
            return TokenReflection.CreateDescendants<JulyMonthsVariableTemporalPrimitiveToken>(state, phrase);
        }

        public JulyMonthsVariableTemporalPrimitiveToken() {
            this.Pattern = new FuzzyDateTimePattern() {
                Rule = TimeType.Definitive,
                Year = DateTime.Now.Month <= 7 ? DateTime.Now.Year : DateTime.Now.Year + 1,
                Month = 7
            };
        }
    }
}