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
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Potato.Database.Shared.Builders.Values {
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

        /// <summary>
        /// Converts a JObject to a series of assignment objects, set to this document.
        /// </summary>
        /// <param name="data">The data to convert</param>
        /// <returns>this</returns>
        public IDatabaseObject FromJObject(JArray data) {
            foreach (var item in data) {
                this.Add(new DocumentValue().FromJObject(item as JObject));
            }

            return this;
        }

        public override object ToObject() {
            return this.Where(statement => statement is Value).Cast<Value>().Select(statement => statement.ToObject()).ToList();
        }
    }
}
