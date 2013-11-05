
namespace Procon.Fuzzy.Tokens.Syntax.Prepositions.Adpositions {
    public class AtAdpositionsPrepositionsSyntaxToken : AdpositionsPrepositionsSyntaxToken {
        public static Phrase Parse(IFuzzyState state, Phrase phrase) {
            return TokenReflection.CreateDescendants<AtAdpositionsPrepositionsSyntaxToken>(state, phrase);
        }
    }
}