
namespace Procon.Fuzzy.Tokens.Operator.Logical.Equality {
    public class EqualsEqualityLogicalOperatorToken : EqualityLogicalOperatorToken {
        public static Phrase Parse(IFuzzyState state, Phrase phrase) {
            return TokenReflection.CreateDescendants<EqualsEqualityLogicalOperatorToken>(state, phrase);
        }

        public EqualsEqualityLogicalOperatorToken() {
            this.ExpressionType = System.Linq.Expressions.ExpressionType.Equal;
        }
    }
}