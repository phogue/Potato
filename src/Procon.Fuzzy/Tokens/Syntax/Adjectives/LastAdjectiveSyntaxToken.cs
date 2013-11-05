
namespace Procon.Fuzzy.Tokens.Syntax.Adjectives {
    public class LastAdjectiveSyntaxToken : AdjectiveSyntaxToken {
        public static Phrase Parse(IFuzzyState state, Phrase phrase) {
            return TokenReflection.CreateDescendants<LastAdjectiveSyntaxToken>(state, phrase);
        }
    }
}