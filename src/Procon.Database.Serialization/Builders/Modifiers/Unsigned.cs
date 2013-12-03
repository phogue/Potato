using System;

namespace Procon.Database.Serialization.Builders.Modifiers {
    /// <summary>
    /// Unsigned attribute for integer values. The lack of this attribute implies Signed.
    /// </summary>
    [Serializable]
    public class Unsigned : Modifier {
    }
}
