namespace Procon.Fuzzy.Tokens.Syntax.Prepositions {
    public class OnPrepositionsSyntaxToken : PrepositionsSyntaxToken {
        public static Phrase Parse(IFuzzyState state, Phrase phrase) {
            return TokenReflection.CreateDescendants<OnPrepositionsSyntaxToken>(state, phrase);
        }
    }
}