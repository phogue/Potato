namespace Procon.Nlp.Tokens.Syntax.Punctuation {
    public class ColonPunctuationSyntaxToken : PunctuationSyntaxToken {

        public new static Phrase Parse(IStateNlp state, Phrase phrase) {
            return TokenReflection.CreateDescendants<ColonPunctuationSyntaxToken>(state, phrase);
        }
    }
}
