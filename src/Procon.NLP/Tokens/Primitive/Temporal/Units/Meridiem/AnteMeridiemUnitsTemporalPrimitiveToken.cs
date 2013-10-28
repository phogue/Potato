namespace Procon.Nlp.Tokens.Primitive.Temporal.Units.Meridiem {
    public class AnteMeridiemUnitsTemporalPrimitiveToken : MeridiemUnitsTemporalPrimitiveToken {

        public new static Phrase Parse(IStateNlp state, Phrase phrase) {
            return TokenReflection.CreateDescendants<AnteMeridiemUnitsTemporalPrimitiveToken>(state, phrase);
        }
    }
}
