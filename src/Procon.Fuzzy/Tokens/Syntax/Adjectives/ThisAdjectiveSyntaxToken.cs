namespace Procon.Fuzzy.Tokens.Syntax.Adjectives {
    public class ThisAdjectiveSyntaxToken : AdjectiveSyntaxToken {
        public static Phrase Parse(IFuzzyState state, Phrase phrase) {
            return TokenReflection.CreateDescendants<ThisAdjectiveSyntaxToken>(state, phrase);
        }
    }
}