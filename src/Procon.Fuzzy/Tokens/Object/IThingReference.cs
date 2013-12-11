using System;

namespace Procon.Fuzzy.Tokens.Object {
    /// <summary>
    /// A reference to an object and how to interact with it.
    /// </summary>
    public interface IThingReference {

        /// <summary>
        /// Tests compatability with another thing reference, making sure both
        /// are the same type or can be used together.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        bool CompatibleWith(IThingReference other);

        /// <summary>
        /// Combines two thing references, used so keeping track of the references can be merged
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        IThingReference Union(IThingReference other);

        /// <summary>
        /// Returns only references in this object and not in the other reference
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        IThingReference Complement(IThingReference other);
    }
}
