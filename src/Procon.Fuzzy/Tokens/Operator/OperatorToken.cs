using System.Linq.Expressions;

namespace Procon.Fuzzy.Tokens.Operator {
    public class OperatorToken : Token {
        /// <summary>
        /// The underlying ling expression type to use when building the linq expression
        /// during execution.
        /// </summary>
        public ExpressionType ExpressionType { get; set; }
    }
}