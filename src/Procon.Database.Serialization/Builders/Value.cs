using System;

namespace Procon.Database.Serialization.Builders {
    /// <summary>
    /// A value 
    /// </summary>
    [Serializable]
    public abstract class Value : DatabaseObject {
        /// <summary>
        /// Casts the text into a simple object
        /// </summary>
        /// <returns></returns>
        public abstract object ToObject();
    }
}
