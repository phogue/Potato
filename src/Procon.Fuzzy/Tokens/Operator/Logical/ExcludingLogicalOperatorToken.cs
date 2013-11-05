
namespace Procon.Fuzzy.Tokens.Operator.Logical {
    public class ExcludingLogicalOperatorToken : LogicalOperatorToken {
        public static Phrase Parse(IFuzzyState state, Phrase phrase) {
            return TokenReflection.CreateDescendants<ExcludingLogicalOperatorToken>(state, phrase);
        }

        public ExcludingLogicalOperatorToken() {
            this.ExpressionType = System.Linq.Expressions.ExpressionType.ExclusiveOr;
        }
    }
}