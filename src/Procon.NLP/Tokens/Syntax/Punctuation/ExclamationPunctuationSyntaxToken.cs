namespace Procon.Nlp.Tokens.Syntax.Punctuation {
    public class ExclamationPunctuationSyntaxToken : PunctuationSyntaxToken {

        public static Phrase Parse(IStateNlp state, Phrase phrase) {
            return TokenReflection.CreateDescendants<ExclamationPunctuationSyntaxToken>(state, phrase);
        }
    }
}
