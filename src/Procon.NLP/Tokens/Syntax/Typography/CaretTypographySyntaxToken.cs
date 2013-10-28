namespace Procon.Nlp.Tokens.Syntax.Typography {
    public class CaretTypographySyntaxToken : TypographySyntaxToken {

        public new static Phrase Parse(IStateNlp state, Phrase phrase) {
            return TokenReflection.CreateDescendants<CaretTypographySyntaxToken>(state, phrase);
        }
    }
}
