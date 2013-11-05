using System;

namespace Procon.Fuzzy.Tokens.Primitive.Temporal.Variable.Months {
    public class FebruaryMonthsVariableTemporalPrimitiveToken : MonthMonthsVariableTemporalPrimitiveToken {
        public static Phrase Parse(IFuzzyState state, Phrase phrase) {
            return TokenReflection.CreateDescendants<FebruaryMonthsVariableTemporalPrimitiveToken>(state, phrase);
        }

        public FebruaryMonthsVariableTemporalPrimitiveToken() {
            this.Pattern = new FuzzyDateTimePattern() {
                Rule = TimeType.Definitive,
                Year = DateTime.Now.Month <= 2 ? DateTime.Now.Year : DateTime.Now.Year + 1,
                Month = 2
            };
        }
    }
}