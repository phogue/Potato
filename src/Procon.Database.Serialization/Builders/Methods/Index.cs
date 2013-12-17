using System;

namespace Procon.Database.Serialization.Builders.Methods {
    /// <summary>
    /// An index on a field
    /// </summary>
    [Serializable]
    public class Index : Method {
        /// <summary>
        /// The name of the index
        /// </summary>
        public String Name { get; set; }
    }
}
