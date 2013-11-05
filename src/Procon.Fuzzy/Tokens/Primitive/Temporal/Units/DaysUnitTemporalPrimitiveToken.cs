
namespace Procon.Fuzzy.Tokens.Primitive.Temporal.Units {
    public class DaysUnitTemporalPrimitiveToken : UnitTemporalPrimitiveToken {
        public static Phrase Parse(IFuzzyState state, Phrase phrase) {
            return TokenReflection.CreateDescendants<DaysUnitTemporalPrimitiveToken>(state, phrase);
        }
    }
}