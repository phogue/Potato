namespace Procon.Nlp.Tokens.Syntax.Punctuation {
    public class HyphenPunctuationSyntaxToken : PunctuationSyntaxToken {

        public static Phrase Parse(IStateNlp state, Phrase phrase) {
            return TokenReflection.CreateDescendants<HyphenPunctuationSyntaxToken>(state, phrase);
        }
    }
}
