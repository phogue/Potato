
namespace Procon.Fuzzy.Tokens.Primitive.Temporal.Units.Meridiem {
    public class PostMeridiemUnitsTemporalPrimitiveToken : MeridiemUnitsTemporalPrimitiveToken {
        public static Phrase Parse(IFuzzyState state, Phrase phrase) {
            return TokenReflection.CreateDescendants<PostMeridiemUnitsTemporalPrimitiveToken>(state, phrase);
        }
    }
}