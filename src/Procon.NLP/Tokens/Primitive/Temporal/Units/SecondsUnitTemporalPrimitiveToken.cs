namespace Procon.Nlp.Tokens.Primitive.Temporal.Units {
    public class SecondsUnitTemporalPrimitiveToken : UnitTemporalPrimitiveToken {

        public static Phrase Parse(IStateNlp state, Phrase phrase) {
            return TokenReflection.CreateDescendants<SecondsUnitTemporalPrimitiveToken>(state, phrase);
        }
    }
}
