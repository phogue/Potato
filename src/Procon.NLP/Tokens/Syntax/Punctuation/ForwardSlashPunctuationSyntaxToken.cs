namespace Procon.Nlp.Tokens.Syntax.Punctuation {
    public class ForwardSlashPunctuationSyntaxToken : PunctuationSyntaxToken {

        public new static Phrase Parse(IStateNlp state, Phrase phrase) {
            return TokenReflection.CreateDescendants<ForwardSlashPunctuationSyntaxToken>(state, phrase);
        }
    }
}
