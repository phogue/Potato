namespace Procon.Nlp.Tokens.Syntax.Punctuation.Parentheses {
    public class OpenParenthesesPunctuationSyntaxToken : ParenthesesPunctuationSyntaxToken {

        public static Phrase Parse(IStateNlp state, Phrase phrase) {
            return TokenReflection.CreateDescendants<OpenParenthesesPunctuationSyntaxToken>(state, phrase);
        }
    }
}
