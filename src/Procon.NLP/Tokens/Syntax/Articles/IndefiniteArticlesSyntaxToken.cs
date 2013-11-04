namespace Procon.Nlp.Tokens.Syntax.Articles {
    public class IndefiniteArticlesSyntaxToken : ArticlesSyntaxToken {

        public static Phrase Parse(IStateNlp state, Phrase phrase) {
            return TokenReflection.CreateDescendants<IndefiniteArticlesSyntaxToken>(state, phrase);
        }
    }
}
