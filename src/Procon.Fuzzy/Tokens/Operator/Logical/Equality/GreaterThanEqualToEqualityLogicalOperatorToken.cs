namespace Procon.Fuzzy.Tokens.Operator.Logical.Equality {
    public class GreaterThanEqualToEqualityLogicalOperatorToken : EqualityLogicalOperatorToken {
        public static Phrase Parse(IFuzzyState state, Phrase phrase) {
            return TokenReflection.CreateDescendants<GreaterThanEqualToEqualityLogicalOperatorToken>(state, phrase);
        }
    }
}