namespace Procon.Nlp.Tokens.Primitive.Temporal.Units {
    public class MinutesUnitTemporalPrimitiveToken : UnitTemporalPrimitiveToken {

        public static Phrase Parse(IStateNlp state, Phrase phrase) {
            return TokenReflection.CreateDescendants<MinutesUnitTemporalPrimitiveToken>(state, phrase);
        }
    }
}
