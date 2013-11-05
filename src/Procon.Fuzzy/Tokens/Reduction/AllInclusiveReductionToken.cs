using System.Linq.Expressions;

namespace Procon.Fuzzy.Tokens.Reduction {
    public class AllInclusiveReductionToken : PropertyReductionToken {
        public static Phrase Parse(IFuzzyState state, Phrase phrase) {
            Phrase returnPhrase = TokenReflection.CreateDescendants<AllInclusiveReductionToken>(state, phrase);

            if (returnPhrase.Count > 0 && returnPhrase[0] is AllInclusiveReductionToken) {
                ((AllInclusiveReductionToken) returnPhrase[0]).LinqExpression = Expression.MakeBinary(ExpressionType.Equal, Expression.Constant(1), Expression.Constant(1));
            }

            return returnPhrase;
        }
    }
}