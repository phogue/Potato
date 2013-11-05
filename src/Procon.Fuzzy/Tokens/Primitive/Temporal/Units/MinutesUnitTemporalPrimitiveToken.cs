
namespace Procon.Fuzzy.Tokens.Primitive.Temporal.Units {
    public class MinutesUnitTemporalPrimitiveToken : UnitTemporalPrimitiveToken {
        public static Phrase Parse(IFuzzyState state, Phrase phrase) {
            return TokenReflection.CreateDescendants<MinutesUnitTemporalPrimitiveToken>(state, phrase);
        }
    }
}