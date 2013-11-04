namespace Procon.Nlp.Tokens.Primitive.Temporal.Units {
    public class WeeksUnitTemporalPrimitiveToken : UnitTemporalPrimitiveToken {

        public static Phrase Parse(IStateNlp state, Phrase phrase) {
            return TokenReflection.CreateDescendants<WeeksUnitTemporalPrimitiveToken>(state, phrase);
        }
    }
}
