using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Procon.Database.Shared.Builders.Statements;

namespace Procon.Database.Shared.Builders.Values {
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

        /// <summary>
        /// Converts a JObject to a series of assignment objects, set to this document.
        /// </summary>
        /// <param name="data">The data to convert</param>
        /// <returns>this</returns>
        public IDatabaseObject FromJObject(JObject data) {
            foreach (var property in data) {
                var value = property.Value as JValue;
                var obj = property.Value as JObject;
                var array = property.Value as JArray;

                if (value != null) {
                    this.Set(property.Key, value.Value);
                }
                else if (obj != null) {
                    DocumentValue document = new DocumentValue();

                    document.FromJObject(obj);

                    this.Set(property.Key, document.ToObject());
                }
                else if (array != null) {
                    CollectionValue collection = new CollectionValue();

                    collection.FromJObject(array);

                    this.Set(property.Key, collection.ToObject());
                }
            }

            return this;
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
