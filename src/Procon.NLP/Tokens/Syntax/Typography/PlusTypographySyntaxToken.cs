namespace Procon.Nlp.Tokens.Syntax.Typography {
    public class PlusTypographySyntaxToken : TypographySyntaxToken {

        public static Phrase Parse(IStateNlp state, Phrase phrase) {
            return TokenReflection.CreateDescendants<PlusTypographySyntaxToken>(state, phrase);
        }
    }
}
