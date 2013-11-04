namespace Procon.Nlp.Tokens.Primitive.Temporal.Units.Meridiem {
    public class AnteMeridiemUnitsTemporalPrimitiveToken : MeridiemUnitsTemporalPrimitiveToken {

        public static Phrase Parse(IStateNlp state, Phrase phrase) {
            return TokenReflection.CreateDescendants<AnteMeridiemUnitsTemporalPrimitiveToken>(state, phrase);
        }
    }
}
