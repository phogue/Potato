namespace Procon.Nlp.Tokens.Syntax.Punctuation {
    public class PeriodPunctuationSyntaxToken : PunctuationSyntaxToken {

        public static Phrase Parse(IStateNlp state, Phrase phrase) {
            return TokenReflection.CreateDescendants<PeriodPunctuationSyntaxToken>(state, phrase);
        }
    }
}
