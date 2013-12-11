namespace Procon.Fuzzy.Tokens.Syntax.Typography {
    public class PlusTypographySyntaxToken : TypographySyntaxToken {
        public static Phrase Parse(IFuzzyState state, Phrase phrase) {
            return TokenReflection.CreateDescendants<PlusTypographySyntaxToken>(state, phrase);
        }
    }
}