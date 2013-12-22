using System;

namespace Procon.Database.Shared.Builders.Modifiers {
    /// <summary>
    /// Unsigned attribute for integer values. The lack of this attribute implies Signed.
    /// </summary>
    [Serializable]
    public class Unsigned : FieldModifier {
    }
}
