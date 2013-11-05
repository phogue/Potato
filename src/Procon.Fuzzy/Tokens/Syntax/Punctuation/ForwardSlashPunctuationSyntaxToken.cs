
namespace Procon.Fuzzy.Tokens.Syntax.Punctuation {
    public class ForwardSlashPunctuationSyntaxToken : PunctuationSyntaxToken {
        public static Phrase Parse(IFuzzyState state, Phrase phrase) {
            return TokenReflection.CreateDescendants<ForwardSlashPunctuationSyntaxToken>(state, phrase);
        }
    }
}