namespace Procon.Nlp.Tokens.Syntax.Punctuation.Parentheses {
    public class OpenParenthesesPunctuationSyntaxToken : ParenthesesPunctuationSyntaxToken {

        public new static Phrase Parse(IStateNlp state, Phrase phrase) {
            return TokenReflection.CreateDescendants<OpenParenthesesPunctuationSyntaxToken>(state, phrase);
        }
    }
}
