namespace Procon.Nlp.Tokens.Primitive.Numeric {
    public class OrdinalNumericPrimitiveToken : FloatNumericPrimitiveToken {

        public new static Phrase Parse(IStateNlp state, Phrase phrase) {
            return TokenReflection.CreateDescendants<OrdinalNumericPrimitiveToken>(state, phrase);
        }
    }
}
