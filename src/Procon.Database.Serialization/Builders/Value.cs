using System;

namespace Procon.Database.Serialization.Builders {
    /// <summary>
    /// A value 
    /// </summary>
    [Serializable]
    public abstract class Value : DatabaseObject {
        public abstract object ToObject();
    }
}
