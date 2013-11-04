namespace Procon.Nlp.Tokens.Syntax.Punctuation {
    public class CommaPunctuationSyntaxToken : PunctuationSyntaxToken {

        public static Phrase Parse(IStateNlp state, Phrase phrase) {
            return TokenReflection.CreateDescendants<CommaPunctuationSyntaxToken>(state, phrase);
        }
    }
}
