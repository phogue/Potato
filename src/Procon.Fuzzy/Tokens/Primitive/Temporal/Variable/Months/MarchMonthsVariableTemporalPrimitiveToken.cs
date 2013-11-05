using System;

namespace Procon.Fuzzy.Tokens.Primitive.Temporal.Variable.Months {
    public class MarchMonthsVariableTemporalPrimitiveToken : MonthMonthsVariableTemporalPrimitiveToken {
        public static Phrase Parse(IFuzzyState state, Phrase phrase) {
            return TokenReflection.CreateDescendants<MarchMonthsVariableTemporalPrimitiveToken>(state, phrase);
        }

        public MarchMonthsVariableTemporalPrimitiveToken() {
            this.Pattern = new FuzzyDateTimePattern() {
                Rule = TimeType.Definitive,
                Year = DateTime.Now.Month <= 3 ? DateTime.Now.Year : DateTime.Now.Year + 1,
                Month = 3
            };
        }
    }
}