namespace Procon.Nlp.Tokens.Syntax.Prepositions.Adpositions {
    public class InAdpositionsPrepositionsSyntaxToken : AdpositionsPrepositionsSyntaxToken {

        public static Phrase Parse(IStateNlp state, Phrase phrase) {
            return TokenReflection.CreateDescendants<InAdpositionsPrepositionsSyntaxToken>(state, phrase);
        }
    }
}