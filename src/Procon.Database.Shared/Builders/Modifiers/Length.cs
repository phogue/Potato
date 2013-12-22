using System;

namespace Procon.Database.Shared.Builders.Modifiers {
    /// <summary>
    /// The length to apply to a type, like string length
    /// </summary>
    [Serializable]
    public class Length : FieldModifier {

        /// <summary>
        /// The length of this attribute.
        /// </summary>
        public int Value { get; set; }
    }
}
