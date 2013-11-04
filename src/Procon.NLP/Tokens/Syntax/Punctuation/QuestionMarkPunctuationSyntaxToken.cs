namespace Procon.Nlp.Tokens.Syntax.Punctuation {
    public class QuestionMarkPunctuationSyntaxToken : PunctuationSyntaxToken {

        public static Phrase Parse(IStateNlp state, Phrase phrase) {
            return TokenReflection.CreateDescendants<QuestionMarkPunctuationSyntaxToken>(state, phrase);
        }
    }
}
