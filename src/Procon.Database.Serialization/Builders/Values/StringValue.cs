using System;

namespace Procon.Database.Serialization.Builders.Values {
    /// <summary>
    /// A simple string value
    /// </summary>
    [Serializable]
    public class StringValue : Value {

        /// <summary>
        /// The text attached to this object
        /// </summary>
        public String Data { get; set; }

        /// <summary>
        /// Casts the text into a simple object
        /// </summary>
        /// <returns></returns>
        public override object ToObject() {
            return this.Data;
        }
    }
}
