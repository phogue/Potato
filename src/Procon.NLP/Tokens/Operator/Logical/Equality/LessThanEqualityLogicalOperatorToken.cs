namespace Procon.Nlp.Tokens.Operator.Logical.Equality {
    public class LessThanEqualityLogicalOperatorToken : EqualityLogicalOperatorToken {

        public new static Phrase Parse(IStateNlp state, Phrase phrase) {
            return TokenReflection.CreateDescendants<LessThanEqualityLogicalOperatorToken>(state, phrase);
        }

        public LessThanEqualityLogicalOperatorToken() {
            this.ExpressionType = System.Linq.Expressions.ExpressionType.LessThan;
        }
    }
}
