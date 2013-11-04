namespace Procon.Nlp.Tokens.Primitive.Temporal.Units.Meridiem {
    public class PostMeridiemUnitsTemporalPrimitiveToken : MeridiemUnitsTemporalPrimitiveToken {

        public static Phrase Parse(IStateNlp state, Phrase phrase) {
            return TokenReflection.CreateDescendants<PostMeridiemUnitsTemporalPrimitiveToken>(state, phrase);
        }
    }
}
