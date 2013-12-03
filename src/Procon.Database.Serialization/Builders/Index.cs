using System;

namespace Procon.Database.Serialization.Builders {
    /// <summary>
    /// An index on a field
    /// </summary>
    [Serializable]
    public class Index : DatabaseObject {
        /// <summary>
        /// The name of the index
        /// </summary>
        public String Name { get; set; }
    }
}
