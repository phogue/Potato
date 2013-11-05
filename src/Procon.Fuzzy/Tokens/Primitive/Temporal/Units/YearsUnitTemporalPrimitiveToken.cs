namespace Procon.Fuzzy.Tokens.Primitive.Temporal.Units {
    public class YearsUnitTemporalPrimitiveToken : UnitTemporalPrimitiveToken {
        public static Phrase Parse(IFuzzyState state, Phrase phrase) {
            return TokenReflection.CreateDescendants<YearsUnitTemporalPrimitiveToken>(state, phrase);
        }
    }
}