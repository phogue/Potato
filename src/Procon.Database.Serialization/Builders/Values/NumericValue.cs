using System;

namespace Procon.Database.Serialization.Builders.Values {
    [Serializable]
    public class NumericValue : Value {

        public int? Integer { get; set; }

        public float? Float { get; set; }

        public override object ToObject() {
            return this.Integer ?? this.Float;
        }
    }
}
