namespace Procon.Fuzzy.Tokens.Syntax.Typography {
    public class AsteriskTypographySyntaxToken : TypographySyntaxToken {
        public static Phrase Parse(IFuzzyState state, Phrase phrase) {
            return TokenReflection.CreateDescendants<AsteriskTypographySyntaxToken>(state, phrase);
        }
    }
}