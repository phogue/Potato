
namespace Procon.Fuzzy.Tokens.Syntax.Prepositions.Adpositions {
    public class ForAdpositionsPrepositionsSyntaxToken : AdpositionsPrepositionsSyntaxToken {
        public static Phrase Parse(IFuzzyState state, Phrase phrase) {
            return TokenReflection.CreateDescendants<ForAdpositionsPrepositionsSyntaxToken>(state, phrase);
        }
    }
}