using System;

namespace Procon.Database.Serialization.Builders.Values {
    /// <summary>
    /// An unescaped string to insert into the query. This could
    /// potentially be a security concern, but depends on how you use it.
    /// </summary>
    [Serializable]
    public class RawValue : Value {
        /// <summary>
        /// The string data to insert inot the built query
        /// </summary>
        public String Data { get; set; }

        public override object ToObject() {
            return this.Data;
        }
    }
}
