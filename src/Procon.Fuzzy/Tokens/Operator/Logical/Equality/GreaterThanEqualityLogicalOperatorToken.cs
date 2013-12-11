namespace Procon.Fuzzy.Tokens.Operator.Logical.Equality {
    public class GreaterThanEqualityLogicalOperatorToken : EqualityLogicalOperatorToken {
        public static Phrase Parse(IFuzzyState state, Phrase phrase) {
            return TokenReflection.CreateDescendants<GreaterThanEqualityLogicalOperatorToken>(state, phrase);
        }
    }
}