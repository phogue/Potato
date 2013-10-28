namespace Procon.Nlp.Tokens.Primitive.Temporal.Units {
    public class HoursUnitTemporalPrimitiveToken : UnitTemporalPrimitiveToken {

        public new static Phrase Parse(IStateNlp state, Phrase phrase) {
            return TokenReflection.CreateDescendants<HoursUnitTemporalPrimitiveToken>(state, phrase);
        }
    }
}
