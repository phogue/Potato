using System;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Procon.Database.Serialization.Builders.Values {
    /// <summary>
    /// A list of documents
    /// </summary>
    [Serializable]
    public class CollectionValue : Value {

        /// <summary>
        /// Converts each sub-value attached to this object into a JObject,
        /// then returning a JArray of JObject's
        /// </summary>
        /// <returns>An array of the sub documents converted to JObject</returns>
        public JArray ToJArray() {
            JArray array = new JArray();

            foreach (Value value in this.Where(statement => statement is Value)) {
                DocumentValue document = value as DocumentValue;

                array.Add(document != null ? new JObject(document.ToJObject()) : new JObject(value.ToObject()));
            }

            return array;
        }

        public override object ToObject() {
            return this.Where(statement => statement is Value).Cast<Value>().Select(statement => statement.ToObject()).ToList();
        }
    }
}
