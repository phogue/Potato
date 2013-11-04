namespace Procon.Nlp.Tokens.Primitive.Temporal.Units {
    public class DaysUnitTemporalPrimitiveToken : UnitTemporalPrimitiveToken {

        public static Phrase Parse(IStateNlp state, Phrase phrase) {
            return TokenReflection.CreateDescendants<DaysUnitTemporalPrimitiveToken>(state, phrase);
        }
    }
}
