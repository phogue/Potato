using System;

namespace Procon.Database.Serialization.Builders {
    public class RawValue : Value {
        public String Data { get; set; }

        public override object ToObject() {
            return this.Data;
        }
    }
}
