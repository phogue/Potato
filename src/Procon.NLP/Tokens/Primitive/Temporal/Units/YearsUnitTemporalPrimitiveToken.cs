namespace Procon.Nlp.Tokens.Primitive.Temporal.Units {
    public class YearsUnitTemporalPrimitiveToken : UnitTemporalPrimitiveToken {

        public new static Phrase Parse(IStateNlp state, Phrase phrase) {
            return TokenReflection.CreateDescendants<YearsUnitTemporalPrimitiveToken>(state, phrase);
        }
    }
}
