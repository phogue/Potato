// Copyright 2011 Geoffrey 'Phogue' Green
// 
// http://www.phogue.net
//  
// This file is part of Procon 2.
// 
// Procon 2 is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// Procon 2 is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Procon 2.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace Procon.NLP.Utils {
    public class ExpressionBuilder<T> : List<Expression<Func<T, bool>>> {

        public ParameterExpression Parameter { get; set; }

        public ExpressionBuilder() {
            // this.Parameter = Expression.Parameter(typeof(T), "x");
        }

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
