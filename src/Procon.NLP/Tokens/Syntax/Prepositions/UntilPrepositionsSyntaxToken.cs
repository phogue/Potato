namespace Procon.Nlp.Tokens.Syntax.Prepositions {
    public class UntilPrepositionsSyntaxToken : PrepositionsSyntaxToken {

        public new static Phrase Parse(IStateNlp state, Phrase phrase) {
            return TokenReflection.CreateDescendants<UntilPrepositionsSyntaxToken>(state, phrase);
        }
    }
}