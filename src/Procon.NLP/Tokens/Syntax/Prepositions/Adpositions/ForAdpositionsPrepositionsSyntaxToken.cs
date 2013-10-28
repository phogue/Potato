namespace Procon.Nlp.Tokens.Syntax.Prepositions.Adpositions {
    public class ForAdpositionsPrepositionsSyntaxToken : AdpositionsPrepositionsSyntaxToken {

        public new static Phrase Parse(IStateNlp state, Phrase phrase) {
            return TokenReflection.CreateDescendants<ForAdpositionsPrepositionsSyntaxToken>(state, phrase);
        }
    }
}