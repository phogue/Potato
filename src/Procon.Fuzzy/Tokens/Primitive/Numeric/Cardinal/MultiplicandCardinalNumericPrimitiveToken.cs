namespace Procon.Fuzzy.Tokens.Primitive.Numeric.Cardinal {
    public class MultiplicandCardinalNumericPrimitiveToken : CardinalNumericPrimitiveToken {
        public new static Phrase Parse(IFuzzyState state, Phrase phrase) {
            return TokenReflection.CreateDescendants<MultiplicandCardinalNumericPrimitiveToken>(state, phrase);
        }
    }
}