namespace Procon.Nlp.Tokens.Operator.Logical.Equality {
    public class LessThanEqualToEqualityLogicalOperatorToken : EqualityLogicalOperatorToken {

        public new static Phrase Parse(IStateNlp state, Phrase phrase) {
            return TokenReflection.CreateDescendants<LessThanEqualToEqualityLogicalOperatorToken>(state, phrase);
        }

        public LessThanEqualToEqualityLogicalOperatorToken() {
            this.ExpressionType = System.Linq.Expressions.ExpressionType.LessThanOrEqual;
        }
    }
}
