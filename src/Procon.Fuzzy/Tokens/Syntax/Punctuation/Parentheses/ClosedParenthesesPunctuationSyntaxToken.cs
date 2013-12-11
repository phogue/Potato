namespace Procon.Fuzzy.Tokens.Syntax.Punctuation.Parentheses {
    public class ClosedParenthesesPunctuationSyntaxToken : ParenthesesPunctuationSyntaxToken {
        public static Phrase Parse(IFuzzyState state, Phrase phrase) {
            return TokenReflection.CreateDescendants<ClosedParenthesesPunctuationSyntaxToken>(state, phrase);
        }
    }
}