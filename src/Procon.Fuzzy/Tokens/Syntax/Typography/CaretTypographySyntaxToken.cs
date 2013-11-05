
namespace Procon.Fuzzy.Tokens.Syntax.Typography {
    public class CaretTypographySyntaxToken : TypographySyntaxToken {
        public static Phrase Parse(IFuzzyState state, Phrase phrase) {
            return TokenReflection.CreateDescendants<CaretTypographySyntaxToken>(state, phrase);
        }
    }
}