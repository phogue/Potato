namespace Procon.Nlp.Tokens.Primitive.Numeric.Cardinal {
    public class MultiplicandCardinalNumericPrimitiveToken : CardinalNumericPrimitiveToken {

        public new static Phrase Parse(IStateNlp state, Phrase phrase) {
            return TokenReflection.CreateDescendants<MultiplicandCardinalNumericPrimitiveToken>(state, phrase);
        }
    }
}
