using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Procon.Nlp.Tokens.Reduction {
    using Procon.Nlp.Utils;
    using Procon.Nlp.Tokens.Object;
    using Procon.Nlp.Tokens.Primitive.Numeric;
    using Procon.Nlp.Tokens.Operator.Logical.Equality;

    public class PropertyReductionToken : ReductionToken {

        public static Phrase Reduce(IStateNlp state, PropertyObjectToken property, EqualityLogicalOperatorToken equality, FloatNumericPrimitiveToken number) {

            ReductionToken reducedToken = new PropertyReductionToken() {
                Text = String.Format("{0} {1} {2}", property.Text, equality.Text, number.Text),
                Similarity = 100.0F
            };
            
            PropertyInfo propertyInfo = state.GetPropertyInfo(property.PropertyName);
            
            if (propertyInfo != null) {
                reducedToken.Parameter = state.LinqParameterMappings[propertyInfo.ReflectedType].Parameter;

                reducedToken.LinqExpression = Expression.MakeBinary(equality.ExpressionType,
                                                            Expression.MakeMemberAccess(reducedToken.Parameter, propertyInfo),
                                                            Expression.Constant(number.ToFloat().ConvertTo(propertyInfo.PropertyType))
                                                            );
            }

            return new Phrase() { reducedToken };
        }

    }
}
