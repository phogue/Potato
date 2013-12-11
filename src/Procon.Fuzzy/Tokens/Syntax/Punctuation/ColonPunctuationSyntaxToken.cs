namespace Procon.Fuzzy.Tokens.Syntax.Punctuation {
    public class ColonPunctuationSyntaxToken : PunctuationSyntaxToken {
        public static Phrase Parse(IFuzzyState state, Phrase phrase) {
            return TokenReflection.CreateDescendants<ColonPunctuationSyntaxToken>(state, phrase);
        }
    }
}