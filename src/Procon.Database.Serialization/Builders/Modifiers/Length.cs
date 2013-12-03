using System;

namespace Procon.Database.Serialization.Builders.Modifiers {
    /// <summary>
    /// The length to apply to a type, like string length
    /// </summary>
    [Serializable]
    public class Length : Modifier {

        /// <summary>
        /// The length of this attribute.
        /// </summary>
        public int Value { get; set; }
    }
}
