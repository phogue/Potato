namespace Procon.Fuzzy.Tokens.Syntax.Punctuation {
    public class QuestionMarkPunctuationSyntaxToken : PunctuationSyntaxToken {
        public static Phrase Parse(IFuzzyState state, Phrase phrase) {
            return TokenReflection.CreateDescendants<QuestionMarkPunctuationSyntaxToken>(state, phrase);
        }
    }
}