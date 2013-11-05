using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;

namespace Procon.Fuzzy.Tokens.Reduction {
    using Procon.Fuzzy.Utils;
    using Procon.Fuzzy.Tokens.Object;
    using Procon.Fuzzy.Tokens.Primitive.Numeric;
    using Procon.Fuzzy.Tokens.Operator.Logical.Equality;

    public class PropertyReductionToken : ReductionToken {
        public static Phrase ReducePropertyEqualityNumber(IFuzzyState state, Dictionary<String, Token> parameters) {
            PropertyObjectToken property = (PropertyObjectToken) parameters["property"];
            EqualityLogicalOperatorToken equality = (EqualityLogicalOperatorToken) parameters["equality"];
            FloatNumericPrimitiveToken number = (FloatNumericPrimitiveToken) parameters["number"];

            ReductionToken reducedToken = new PropertyReductionToken() {
                Text = String.Format("{0} {1} {2}", property.Text, equality.Text, number.Text),
                Similarity = 100.0F
            };

            PropertyInfo propertyInfo = state.GetPropertyInfo(property.PropertyName);

            if (propertyInfo != null) {
                reducedToken.Parameter = state.LinqParameterMappings[propertyInfo.ReflectedType].Parameter;

                reducedToken.LinqExpression = Expression.MakeBinary(equality.ExpressionType, Expression.MakeMemberAccess(reducedToken.Parameter, propertyInfo), Expression.Constant(number.ToFloat().ConvertTo(propertyInfo.PropertyType)));
            }

            return new Phrase() {
                reducedToken
            };
        }
    }
}