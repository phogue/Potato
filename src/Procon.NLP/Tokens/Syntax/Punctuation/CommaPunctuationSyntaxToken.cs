namespace Procon.Nlp.Tokens.Syntax.Punctuation {
    public class CommaPunctuationSyntaxToken : PunctuationSyntaxToken {

        public new static Phrase Parse(IStateNlp state, Phrase phrase) {
            return TokenReflection.CreateDescendants<CommaPunctuationSyntaxToken>(state, phrase);
        }
    }
}
