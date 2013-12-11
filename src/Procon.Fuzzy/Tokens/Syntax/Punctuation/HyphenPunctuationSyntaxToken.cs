namespace Procon.Fuzzy.Tokens.Syntax.Punctuation {
    public class HyphenPunctuationSyntaxToken : PunctuationSyntaxToken {
        public static Phrase Parse(IFuzzyState state, Phrase phrase) {
            return TokenReflection.CreateDescendants<HyphenPunctuationSyntaxToken>(state, phrase);
        }
    }
}