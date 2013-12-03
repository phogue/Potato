using System;

namespace Procon.Database.Serialization.Builders {
    public class StringValue : Value {
        public String Data { get; set; }

        public override object ToObject() {
            return this.Data;
        }
    }
}
