namespace Procon.Nlp.Tokens.Syntax.Adjectives {
    public class EveryAdjectiveSyntaxToken : AdjectiveSyntaxToken {

        public new static Phrase Parse(IStateNlp state, Phrase phrase) {
            return TokenReflection.CreateDescendants<EveryAdjectiveSyntaxToken>(state, phrase);
        }
    }
}
