using System;

namespace Procon.Database.Shared.Builders.Values {
    /// <summary>
    /// A date/time value
    /// </summary>
    [Serializable]
    public class DateTimeValue : Value {

        /// <summary>
        /// The text attached to this object
        /// </summary>
        public DateTime Data { get; set; }

        /// <summary>
        /// Casts the text into a simple object
        /// </summary>
        /// <returns></returns>
        public override object ToObject() {
            return this.Data;
        }
    }
}
