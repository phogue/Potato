using System;

namespace Procon.Database.Serialization.Builders.Attributes {
    /// <summary>
    /// Descending attribute. The lack of this attribute implies Ascending.
    /// </summary>
    [Serializable]
    public class Descending : Attribute {
    }
}
