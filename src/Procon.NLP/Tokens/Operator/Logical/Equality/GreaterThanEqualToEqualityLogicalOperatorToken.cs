namespace Procon.Nlp.Tokens.Operator.Logical.Equality {
    public class GreaterThanEqualToEqualityLogicalOperatorToken : EqualityLogicalOperatorToken {

        public new static Phrase Parse(IStateNlp state, Phrase phrase) {
            return TokenReflection.CreateDescendants<GreaterThanEqualToEqualityLogicalOperatorToken>(state, phrase);
        }

        public GreaterThanEqualToEqualityLogicalOperatorToken() {
            this.ExpressionType = System.Linq.Expressions.ExpressionType.GreaterThanOrEqual;
        }
    }
}
