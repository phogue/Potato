using System;

namespace Procon.Database.Serialization.Builders {
    /// <summary>
    /// A statement for sorting, selecting a field or assigning a value to a field
    /// </summary>
    [Serializable]
    public abstract class Statement : DatabaseObject {

        /// <summary>
        /// The name of a field
        /// </summary>
        public String Name { get; set; }
    }
}
