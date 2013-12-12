using Procon.Fuzzy.Tokens.Operator.Logical.Equality;
using Procon.Fuzzy.Tokens.Primitive.Numeric;

namespace Procon.Fuzzy.Tokens.Object {
    /// <summary>
    /// How to interact with a numeric property on a given thing
    /// </summary>
    public interface INumericPropertyReference {

        /// <summary>
        /// What thing this property can relate to
        /// </summary>
        IThingReference ThingReference { get; set; }

        /// <summary>
        /// Remove all references that do not match the comparator
        /// </summary>
        /// <param name="thing"></param>
        /// <param name="comparator"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        bool RemoveAll(IThingReference thing, EqualityLogicalOperatorToken comparator, FloatNumericPrimitiveToken value);
    }
}
