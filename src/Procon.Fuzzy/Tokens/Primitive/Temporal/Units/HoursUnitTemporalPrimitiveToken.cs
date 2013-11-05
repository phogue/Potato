
namespace Procon.Fuzzy.Tokens.Primitive.Temporal.Units {
    public class HoursUnitTemporalPrimitiveToken : UnitTemporalPrimitiveToken {
        public static Phrase Parse(IFuzzyState state, Phrase phrase) {
            return TokenReflection.CreateDescendants<HoursUnitTemporalPrimitiveToken>(state, phrase);
        }
    }
}