using System;

namespace Procon.Fuzzy.Tokens.Primitive.Temporal.Variable.Months {
    public class NovemberMonthsVariableTemporalPrimitiveToken : MonthMonthsVariableTemporalPrimitiveToken {
        public static Phrase Parse(IFuzzyState state, Phrase phrase) {
            return TokenReflection.CreateDescendants<NovemberMonthsVariableTemporalPrimitiveToken>(state, phrase);
        }

        public NovemberMonthsVariableTemporalPrimitiveToken() {
            this.Pattern = new FuzzyDateTimePattern() {
                Rule = TimeType.Definitive,
                Year = DateTime.Now.Month <= 11 ? DateTime.Now.Year : DateTime.Now.Year + 1,
                Month = 11
            };
        }
    }
}