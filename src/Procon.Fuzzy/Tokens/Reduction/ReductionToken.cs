using System.Linq.Expressions;

namespace Procon.Fuzzy.Tokens.Reduction {
    public class ReductionToken : Token {
        /// <summary>
        /// The parameter expression that matches this objects/properties/methods type
        /// </summary>
        public ParameterExpression Parameter { get; set; }

        /// <summary>
        /// The expression set or built up during parsing/reduction.
        /// </summary>
        public Expression LinqExpression { get; set; }
    }
}