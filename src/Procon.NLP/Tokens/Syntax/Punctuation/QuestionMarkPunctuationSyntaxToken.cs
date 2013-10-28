namespace Procon.Nlp.Tokens.Syntax.Punctuation {
    public class QuestionMarkPunctuationSyntaxToken : PunctuationSyntaxToken {

        public new static Phrase Parse(IStateNlp state, Phrase phrase) {
            return TokenReflection.CreateDescendants<QuestionMarkPunctuationSyntaxToken>(state, phrase);
        }
    }
}
