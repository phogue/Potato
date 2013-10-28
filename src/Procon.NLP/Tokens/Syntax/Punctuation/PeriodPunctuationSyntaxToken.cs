namespace Procon.Nlp.Tokens.Syntax.Punctuation {
    public class PeriodPunctuationSyntaxToken : PunctuationSyntaxToken {

        public new static Phrase Parse(IStateNlp state, Phrase phrase) {
            return TokenReflection.CreateDescendants<PeriodPunctuationSyntaxToken>(state, phrase);
        }
    }
}
