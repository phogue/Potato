
namespace Procon.Fuzzy.Tokens.Syntax.Prepositions {
    public class UntilPrepositionsSyntaxToken : PrepositionsSyntaxToken {
        public static Phrase Parse(IFuzzyState state, Phrase phrase) {
            return TokenReflection.CreateDescendants<UntilPrepositionsSyntaxToken>(state, phrase);
        }
    }
}