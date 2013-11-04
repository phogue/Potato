namespace Procon.Nlp.Tokens.Primitive.Temporal.Units {
    public class HoursUnitTemporalPrimitiveToken : UnitTemporalPrimitiveToken {

        public static Phrase Parse(IStateNlp state, Phrase phrase) {
            return TokenReflection.CreateDescendants<HoursUnitTemporalPrimitiveToken>(state, phrase);
        }
    }
}
