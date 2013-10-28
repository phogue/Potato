using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Procon.Nlp.Utils {
    public class ExpressionBuilder<T> : List<Expression<Func<T, bool>>> {

        public ParameterExpression Parameter { get; set; }

        private Expression<Func<T, bool>> ToLambda(Expression expression) {
            return Expression.Lambda<Func<T, bool>>(Expression.Convert(expression, typeof(bool)), this.Parameter);
        }

        public void AddExpression(Expression expression) {
            this.Add(this.ToLambda(expression));
        }

        public ExpressionBuilder<T> Combine(ExpressionType expressionType) {
            while (this.Count >= 2 && this[0].Body is UnaryExpression && this[1].Body is UnaryExpression) {
                this[0] = this.ToLambda(Expression.MakeBinary(expressionType, ((UnaryExpression)this[0].Body).Operand, ((UnaryExpression)this[1].Body).Operand));
                this.RemoveAt(1);
            }

            return this;
        }

    }
}
