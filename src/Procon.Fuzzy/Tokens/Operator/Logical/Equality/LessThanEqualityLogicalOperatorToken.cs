namespace Procon.Fuzzy.Tokens.Operator.Logical.Equality {
    public class LessThanEqualityLogicalOperatorToken : EqualityLogicalOperatorToken {
        public static Phrase Parse(IFuzzyState state, Phrase phrase) {
            return TokenReflection.CreateDescendants<LessThanEqualityLogicalOperatorToken>(state, phrase);
        }
    }
}