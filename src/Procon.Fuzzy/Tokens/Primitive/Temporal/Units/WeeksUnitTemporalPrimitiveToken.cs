namespace Procon.Fuzzy.Tokens.Primitive.Temporal.Units {
    public class WeeksUnitTemporalPrimitiveToken : UnitTemporalPrimitiveToken {
        public static Phrase Parse(IFuzzyState state, Phrase phrase) {
            return TokenReflection.CreateDescendants<WeeksUnitTemporalPrimitiveToken>(state, phrase);
        }
    }
}