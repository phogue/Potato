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
using System.Linq.Expressions;
using System.Text;
using System.Reflection;
using System.ComponentModel;

namespace Procon.NLP.Tokens.Reduction {
    using Procon.NLP.Utils;
    using Procon.NLP.Tokens.Object;
    using Procon.NLP.Tokens.Primitive;
    using Procon.NLP.Tokens.Primitive.Numeric;
    using Procon.NLP.Tokens.Operator;
    using Procon.NLP.Tokens.Operator.Logical;
    using Procon.NLP.Tokens.Operator.Logical.Equality;

    public class PropertyReductionToken : ReductionToken {

        public static Phrase Reduce(IStateNLP state, PropertyObjectToken property, EqualityLogicalOperatorToken equality, FloatNumericPrimitiveToken number) {

            ReductionToken reducedToken = new PropertyReductionToken() {
                Text = String.Format("{0} {1} {2}", property.Text, equality.Text, number.Text),
                Similarity = 100.0F
            };
            
            PropertyInfo propertyInfo = state.GetPropertyInfo(property.PropertyName);
            
            if (propertyInfo != null) {
                reducedToken.LinqExpression = Expression.MakeBinary(equality.ExpressionType,
                                                            Expression.MakeMemberAccess(state.LinqParameterExpressions[propertyInfo.ReflectedType], propertyInfo),
                                                            Expression.Constant(number.ToFloat().ConvertTo(propertyInfo.PropertyType))
                                                            );
            }

            return new Phrase() { reducedToken };
        }

    }
}
