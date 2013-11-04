namespace Procon.Nlp.Tokens.Operator.Logical.Equality {
    public class EqualsEqualityLogicalOperatorToken : EqualityLogicalOperatorToken {

        public static Phrase Parse(IStateNlp state, Phrase phrase) {
            return TokenReflection.CreateDescendants<EqualsEqualityLogicalOperatorToken>(state, phrase);
        }

        public EqualsEqualityLogicalOperatorToken() {
            this.ExpressionType = System.Linq.Expressions.ExpressionType.Equal;
        }
    }
}
