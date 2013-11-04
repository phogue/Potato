namespace Procon.Nlp.Tokens.Operator.Logical {
    public class AndLogicalOperatorToken : LogicalOperatorToken {

        public static Phrase Parse(IStateNlp state, Phrase phrase) {
            return TokenReflection.CreateDescendants<AndLogicalOperatorToken>(state, phrase);
        }

        public AndLogicalOperatorToken() {
            this.ExpressionType = System.Linq.Expressions.ExpressionType.AndAlso;
        }

    }
}
