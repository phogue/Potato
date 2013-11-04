namespace Procon.Nlp.Tokens.Syntax.Prepositions {
    public class OnPrepositionsSyntaxToken : PrepositionsSyntaxToken {

        public static Phrase Parse(IStateNlp state, Phrase phrase) {
            return TokenReflection.CreateDescendants<OnPrepositionsSyntaxToken>(state, phrase);
        }
    }
}