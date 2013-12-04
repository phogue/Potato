using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Procon.Database.Serialization.Builders.Statements;

namespace Procon.Database.Serialization.Builders.Values {
    /// <summary>
    /// An object of data, like a dictionary
    /// </summary>
    [Serializable]
    public class DocumentValue : Value {

        /// <summary>
        /// Converts this document into a base JObject
        /// </summary>
        /// <returns></returns>
        public JObject ToJObject() {
            JObject data = new JObject();

            foreach (Assignment assignment in this.Where(statement => statement is Assignment)) {
                Field field = assignment.FirstOrDefault(statement => statement is Field) as Field;
                Value value = assignment.FirstOrDefault(statement => statement is Value) as Value;

                if (field != null && value != null) {
                    DocumentValue document = value as DocumentValue;
                    CollectionValue collection = value as CollectionValue;

                    // If it's a sub document..
                    if (document != null) {
                        data.Add(new JProperty(field.Name, document.ToJObject()));
                    }
                    // If it's an array of values (no assignments)
                    else if (collection != null) {
                        data.Add(new JProperty(field.Name, collection.ToJArray()));
                    }
                    else {
                        data.Add(new JProperty(field.Name, value.ToObject()));
                    }
                }
            }

            return data;
        }

        public override object ToObject() {
            Dictionary<String, Object> data = new Dictionary<string, Object>();

            foreach (Assignment assignment in this.Where(statement => statement is Assignment)) {
                Field field = assignment.FirstOrDefault(statement => statement is Field) as Field;
                Value value = assignment.FirstOrDefault(statement => statement is Value) as Value;

                if (field != null && value != null && data.ContainsKey(field.Name) == false) {
                    data.Add(field.Name, value.ToObject());
                }
            }

            return data;
        }
    }
}
