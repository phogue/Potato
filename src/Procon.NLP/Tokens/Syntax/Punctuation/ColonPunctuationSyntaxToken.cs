namespace Procon.Nlp.Tokens.Syntax.Punctuation {
    public class ColonPunctuationSyntaxToken : PunctuationSyntaxToken {

        public static Phrase Parse(IStateNlp state, Phrase phrase) {
            return TokenReflection.CreateDescendants<ColonPunctuationSyntaxToken>(state, phrase);
        }
    }
}
