
namespace Procon.Fuzzy.Tokens.Operator.Logical {
    public class NotLogicalOperatorToken : LogicalOperatorToken {
        public static Phrase Parse(IFuzzyState state, Phrase phrase) {
            return TokenReflection.CreateDescendants<NotLogicalOperatorToken>(state, phrase);
        }

        public NotLogicalOperatorToken() {
            this.ExpressionType = System.Linq.Expressions.ExpressionType.NotEqual;
        }
    }
}