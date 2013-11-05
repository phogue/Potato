
namespace Procon.Fuzzy.Tokens.Syntax.Punctuation {
    public class PeriodPunctuationSyntaxToken : PunctuationSyntaxToken {
        public static Phrase Parse(IFuzzyState state, Phrase phrase) {
            return TokenReflection.CreateDescendants<PeriodPunctuationSyntaxToken>(state, phrase);
        }
    }
}