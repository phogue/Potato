
namespace Procon.Fuzzy.Tokens.Syntax.Punctuation.Parentheses {
    public class OpenParenthesesPunctuationSyntaxToken : ParenthesesPunctuationSyntaxToken {
        public static Phrase Parse(IFuzzyState state, Phrase phrase) {
            return TokenReflection.CreateDescendants<OpenParenthesesPunctuationSyntaxToken>(state, phrase);
        }
    }
}