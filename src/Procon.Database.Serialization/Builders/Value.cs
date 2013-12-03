using System;

namespace Procon.Database.Serialization.Builders {
    [Serializable]
    public abstract class Value : DatabaseObject {
        public abstract object ToObject();
    }
}
