namespace Procon.Nlp.Tokens.Syntax.Punctuation.Parentheses {
    public class ClosedParenthesesPunctuationSyntaxToken : ParenthesesPunctuationSyntaxToken {

        public new static Phrase Parse(IStateNlp state, Phrase phrase) {
            return TokenReflection.CreateDescendants<ClosedParenthesesPunctuationSyntaxToken>(state, phrase);
        }
    }
}
