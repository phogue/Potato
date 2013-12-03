using System;

namespace Procon.Database.Serialization.Builders.Attributes {
    [Serializable]
    public class Length : Attribute {

        /// <summary>
        /// The length of this attribute.
        /// </summary>
        public int Value { get; set; }
    }
}
