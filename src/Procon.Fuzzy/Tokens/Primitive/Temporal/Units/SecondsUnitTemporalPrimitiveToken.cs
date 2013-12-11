namespace Procon.Fuzzy.Tokens.Primitive.Temporal.Units {
    public class SecondsUnitTemporalPrimitiveToken : UnitTemporalPrimitiveToken {
        public static Phrase Parse(IFuzzyState state, Phrase phrase) {
            return TokenReflection.CreateDescendants<SecondsUnitTemporalPrimitiveToken>(state, phrase);
        }
    }
}