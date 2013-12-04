using System;

namespace Procon.Database.Serialization.Builders.Values {
    /// <summary>
    /// A numeric value, kept as a integer or float.
    /// </summary>
    [Serializable]
    public class NumericValue : Value {

        /// <summary>
        /// The integer value of this object
        /// </summary>
        public int? Integer { get; set; }

        /// <summary>
        /// The float value of this object
        /// </summary>
        public float? Float { get; set; }

        public override object ToObject() {
            return this.Integer ?? this.Float;
        }
    }
}
