
namespace Procon.Fuzzy.Tokens.Syntax.Punctuation {
    public class CommaPunctuationSyntaxToken : PunctuationSyntaxToken {
        public static Phrase Parse(IFuzzyState state, Phrase phrase) {
            return TokenReflection.CreateDescendants<CommaPunctuationSyntaxToken>(state, phrase);
        }
    }
}