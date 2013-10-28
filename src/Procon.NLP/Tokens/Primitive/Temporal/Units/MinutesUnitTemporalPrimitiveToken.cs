namespace Procon.Nlp.Tokens.Primitive.Temporal.Units {
    public class MinutesUnitTemporalPrimitiveToken : UnitTemporalPrimitiveToken {

        public new static Phrase Parse(IStateNlp state, Phrase phrase) {
            return TokenReflection.CreateDescendants<MinutesUnitTemporalPrimitiveToken>(state, phrase);
        }
    }
}
