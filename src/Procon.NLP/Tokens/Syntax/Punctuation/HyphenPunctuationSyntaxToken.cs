namespace Procon.Nlp.Tokens.Syntax.Punctuation {
    public class HyphenPunctuationSyntaxToken : PunctuationSyntaxToken {

        public new static Phrase Parse(IStateNlp state, Phrase phrase) {
            return TokenReflection.CreateDescendants<HyphenPunctuationSyntaxToken>(state, phrase);
        }
    }
}
