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
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Potato.Database.Shared;
using Potato.Database.Shared.Builders.Methods.Data;
using Potato.Database.Shared.Builders.Values;

namespace Potato.Core.Shared.Database {
    /// <summary>
    /// The base database model, handling serialization to/from database queries/results.
    /// </summary>
    public abstract class DatabaseModel<T> where T : new() {
        /// <summary>
        /// Deserializes a returned database results object into a list of models
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static List<T> FromQuery(IDatabaseObject data) {
            var value = data as CollectionValue;

            // Wrap the DocumentValue into a CollectionValue.
            if (value == null) {
                value = new CollectionValue();

                if (data != null) {
                    value.Add(data);
                }
            }

            return value.ToJArray().ToObject<List<T>>();
        }

        /// <summary>
        /// Deserializes a returned database results object into a list of models and returns the first model
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static T FirstFromQuery(IDatabaseObject data) {
            return FromQuery(data).FirstOrDefault();
        }

        /// <summary>
        /// Serializes a model to a Save query to issue to the database.
        /// </summary>
        /// <returns>A new save query with property assignments attached</returns>
        public Save ToSaveQuery() {
            var save = new Save();

            save.AddRange(new DocumentValue().FromJObject(JObject.FromObject(this)));

            return save;
        }

        /// <summary>
        /// Probably not as useful as Save since this would set all variables within a document, not as useful for DateTime fields.
        /// </summary>
        /// <returns>A new modify query with property assignments attached</returns>
        public Modify ToModifyQuery() {
            var modify = new Modify();

            modify.AddRange(new DocumentValue().FromJObject(JObject.FromObject(this)));

            return modify;
        }
    }
}
