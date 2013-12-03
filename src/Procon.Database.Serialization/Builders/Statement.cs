using System;

namespace Procon.Database.Serialization.Builders {
    [Serializable]
    public abstract class Statement : DatabaseObject {

        public String Name { get; set; }

    }
}
