namespace Procon.Nlp.Tokens.Primitive.Temporal.Units {
    public class WeeksUnitTemporalPrimitiveToken : UnitTemporalPrimitiveToken {

        public new static Phrase Parse(IStateNlp state, Phrase phrase) {
            return TokenReflection.CreateDescendants<WeeksUnitTemporalPrimitiveToken>(state, phrase);
        }
    }
}
