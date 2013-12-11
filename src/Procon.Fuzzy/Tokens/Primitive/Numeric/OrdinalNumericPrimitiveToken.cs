namespace Procon.Fuzzy.Tokens.Primitive.Numeric {
    public class OrdinalNumericPrimitiveToken : FloatNumericPrimitiveToken {
        public new static Phrase Parse(IFuzzyState state, Phrase phrase) {
            return TokenReflection.CreateDescendants<OrdinalNumericPrimitiveToken>(state, phrase);
        }
    }
}