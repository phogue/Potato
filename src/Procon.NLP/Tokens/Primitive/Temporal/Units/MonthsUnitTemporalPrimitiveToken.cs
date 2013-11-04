namespace Procon.Nlp.Tokens.Primitive.Temporal.Units {
    public class MonthsUnitTemporalPrimitiveToken : UnitTemporalPrimitiveToken {

        public static Phrase Parse(IStateNlp state, Phrase phrase) {
            return TokenReflection.CreateDescendants<MonthsUnitTemporalPrimitiveToken>(state, phrase);
        }
    }
}
