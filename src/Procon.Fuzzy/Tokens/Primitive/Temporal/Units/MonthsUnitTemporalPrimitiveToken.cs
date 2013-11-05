
namespace Procon.Fuzzy.Tokens.Primitive.Temporal.Units {
    public class MonthsUnitTemporalPrimitiveToken : UnitTemporalPrimitiveToken {
        public static Phrase Parse(IFuzzyState state, Phrase phrase) {
            return TokenReflection.CreateDescendants<MonthsUnitTemporalPrimitiveToken>(state, phrase);
        }
    }
}