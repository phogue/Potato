namespace Procon.Nlp.Tokens.Syntax.Punctuation.Parentheses {
    public class ClosedParenthesesPunctuationSyntaxToken : ParenthesesPunctuationSyntaxToken {

        public static Phrase Parse(IStateNlp state, Phrase phrase) {
            return TokenReflection.CreateDescendants<ClosedParenthesesPunctuationSyntaxToken>(state, phrase);
        }
    }
}
