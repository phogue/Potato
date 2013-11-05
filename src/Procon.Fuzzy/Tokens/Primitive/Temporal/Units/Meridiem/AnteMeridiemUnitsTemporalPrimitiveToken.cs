
namespace Procon.Fuzzy.Tokens.Primitive.Temporal.Units.Meridiem {
    public class AnteMeridiemUnitsTemporalPrimitiveToken : MeridiemUnitsTemporalPrimitiveToken {
        public static Phrase Parse(IFuzzyState state, Phrase phrase) {
            return TokenReflection.CreateDescendants<AnteMeridiemUnitsTemporalPrimitiveToken>(state, phrase);
        }
    }
}