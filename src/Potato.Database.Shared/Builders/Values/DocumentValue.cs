#region Copyright
// Copyright 2014 Myrcon Pty. Ltd.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Potato.Database.Shared.Builders.Statements;

namespace Potato.Database.Shared.Builders.Values {
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
            var data = new JObject();

            foreach (Assignment assignment in this.Where(statement => statement is Assignment)) {
                var field = assignment.FirstOrDefault(statement => statement is Field) as Field;
                var value = assignment.FirstOrDefault(statement => statement is Value) as Value;

                if (field != null && value != null) {
                    var document = value as DocumentValue;
                    var collection = value as CollectionValue;

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
                    Set(property.Key, value.Value);
                }
                else if (obj != null) {
                    var document = new DocumentValue();

                    document.FromJObject(obj);

                    Set(property.Key, document.ToObject());
                }
                else if (array != null) {
                    var collection = new CollectionValue();

                    collection.FromJObject(array);

                    Set(property.Key, collection.ToObject());
                }
            }

            return this;
        }

        public override object ToObject() {
            var data = new Dictionary<string, object>();

            foreach (Assignment assignment in this.Where(statement => statement is Assignment)) {
                var field = assignment.FirstOrDefault(statement => statement is Field) as Field;
                var value = assignment.FirstOrDefault(statement => statement is Value) as Value;

                if (field != null && value != null && data.ContainsKey(field.Name) == false) {
                    data.Add(field.Name, value.ToObject());
                }
            }

            return data;
        }
    }
}
