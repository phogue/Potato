namespace Procon.Nlp.Tokens.Syntax.Adjectives {
    public class ThisAdjectiveSyntaxToken : AdjectiveSyntaxToken {

        public static Phrase Parse(IStateNlp state, Phrase phrase) {
            return TokenReflection.CreateDescendants<ThisAdjectiveSyntaxToken>(state, phrase);
        }
    }
}
