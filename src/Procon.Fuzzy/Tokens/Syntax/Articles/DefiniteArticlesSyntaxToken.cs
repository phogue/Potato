
namespace Procon.Fuzzy.Tokens.Syntax.Articles {
    public class DefiniteArticlesSyntaxToken : ArticlesSyntaxToken {
        public static Phrase Parse(IFuzzyState state, Phrase phrase) {
            return TokenReflection.CreateDescendants<DefiniteArticlesSyntaxToken>(state, phrase);
        }
    }
}