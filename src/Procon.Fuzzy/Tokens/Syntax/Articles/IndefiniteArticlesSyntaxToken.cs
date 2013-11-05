
namespace Procon.Fuzzy.Tokens.Syntax.Articles {
    public class IndefiniteArticlesSyntaxToken : ArticlesSyntaxToken {
        public static Phrase Parse(IFuzzyState state, Phrase phrase) {
            return TokenReflection.CreateDescendants<IndefiniteArticlesSyntaxToken>(state, phrase);
        }
    }
}