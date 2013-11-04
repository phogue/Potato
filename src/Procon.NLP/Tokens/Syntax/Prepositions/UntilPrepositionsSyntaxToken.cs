namespace Procon.Nlp.Tokens.Syntax.Prepositions {
    public class UntilPrepositionsSyntaxToken : PrepositionsSyntaxToken {

        public static Phrase Parse(IStateNlp state, Phrase phrase) {
            return TokenReflection.CreateDescendants<UntilPrepositionsSyntaxToken>(state, phrase);
        }
    }
}