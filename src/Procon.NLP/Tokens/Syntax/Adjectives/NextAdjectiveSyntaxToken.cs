namespace Procon.Nlp.Tokens.Syntax.Adjectives {
    public class NextAdjectiveSyntaxToken : AdjectiveSyntaxToken {

        public new static Phrase Parse(IStateNlp state, Phrase phrase) {
            return TokenReflection.CreateDescendants<NextAdjectiveSyntaxToken>(state, phrase);
        }
    }
}
