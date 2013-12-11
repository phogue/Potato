using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Procon.Fuzzy.Tokens.Operator.Logical.Equality;
using Procon.Fuzzy.Tokens.Primitive.Numeric;

namespace Procon.Fuzzy.Tokens.Object {
    public interface INumericPropertyReference {

        /// <summary>
        /// What thing this property can relate to
        /// </summary>
        IThingReference ThingReference { get; set; }

        /// <summary>
        /// Checks if 
        /// </summary>
        /// <param name="thing"></param>
        /// <param name="comparator"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        bool RemoveAll(IThingReference thing, EqualityLogicalOperatorToken comparator, FloatNumericPrimitiveToken value);
    }
}
