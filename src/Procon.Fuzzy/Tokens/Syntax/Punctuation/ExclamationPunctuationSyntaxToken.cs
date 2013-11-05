
namespace Procon.Fuzzy.Tokens.Syntax.Punctuation {
    public class ExclamationPunctuationSyntaxToken : PunctuationSyntaxToken {
        public static Phrase Parse(IFuzzyState state, Phrase phrase) {
            return TokenReflection.CreateDescendants<ExclamationPunctuationSyntaxToken>(state, phrase);
        }
    }
}