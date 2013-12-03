using System;

namespace Procon.Database.Serialization.Builders.Values {
    [Serializable]
    public class StringValue : Value {
        public String Data { get; set; }

        public override object ToObject() {
            return this.Data;
        }
    }
}
