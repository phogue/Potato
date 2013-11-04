namespace Procon.Nlp.Tokens.Syntax.Adjectives {
    public class EveryAdjectiveSyntaxToken : AdjectiveSyntaxToken {

        public static Phrase Parse(IStateNlp state, Phrase phrase) {
            return TokenReflection.CreateDescendants<EveryAdjectiveSyntaxToken>(state, phrase);
        }
    }
}
