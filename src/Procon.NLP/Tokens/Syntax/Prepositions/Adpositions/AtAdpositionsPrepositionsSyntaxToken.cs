namespace Procon.Nlp.Tokens.Syntax.Prepositions.Adpositions {
    public class AtAdpositionsPrepositionsSyntaxToken : AdpositionsPrepositionsSyntaxToken {

        public static Phrase Parse(IStateNlp state, Phrase phrase) {
            return TokenReflection.CreateDescendants<AtAdpositionsPrepositionsSyntaxToken>(state, phrase);
        }
    }
}
