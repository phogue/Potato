namespace Procon.Nlp.Tokens.Syntax.Articles {
    public class DefiniteArticlesSyntaxToken : ArticlesSyntaxToken {

        public static Phrase Parse(IStateNlp state, Phrase phrase) {
            return TokenReflection.CreateDescendants<DefiniteArticlesSyntaxToken>(state, phrase);
        }
    }
}
