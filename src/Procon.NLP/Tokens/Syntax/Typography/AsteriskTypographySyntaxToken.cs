namespace Procon.Nlp.Tokens.Syntax.Typography {
    public class AsteriskTypographySyntaxToken : TypographySyntaxToken {

        public new static Phrase Parse(IStateNlp state, Phrase phrase) {
            return TokenReflection.CreateDescendants<AsteriskTypographySyntaxToken>(state, phrase);
        }
    }
}
