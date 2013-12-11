namespace Procon.Fuzzy.Tokens.Syntax.Adjectives {
    public class NextAdjectiveSyntaxToken : AdjectiveSyntaxToken {
        public static Phrase Parse(IFuzzyState state, Phrase phrase) {
            return TokenReflection.CreateDescendants<NextAdjectiveSyntaxToken>(state, phrase);
        }
    }
}