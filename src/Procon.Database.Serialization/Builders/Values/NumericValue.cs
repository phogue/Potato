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
        public long? Long { get; set; }

        /// <summary>
        /// The float value of this object
        /// </summary>
        public double? Double { get; set; }

        /// <summary>
        /// Casts the text into a simple object
        /// </summary>
        /// <returns></returns>
        public override object ToObject() {
            return this.Long ?? this.Double;
        }
    }
}
