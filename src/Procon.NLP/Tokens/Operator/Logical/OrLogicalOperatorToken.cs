namespace Procon.Nlp.Tokens.Operator.Logical {
    public class OrLogicalOperatorToken : LogicalOperatorToken {

        public new static Phrase Parse(IStateNlp state, Phrase phrase) {
            return TokenReflection.CreateDescendants<OrLogicalOperatorToken>(state, phrase);
        }

        public OrLogicalOperatorToken() {
            this.ExpressionType = System.Linq.Expressions.ExpressionType.OrElse;
        }
    }
}
