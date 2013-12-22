using System;

namespace Procon.Database.Shared.Builders.Modifiers {
    /// <summary>
    /// The field allows for null values. Without this field the field
    /// will be marked as not nullable, not allowing void values.
    /// </summary>
    [Serializable]
    public class Nullable : FieldModifier {
    }
}
