using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Procon.Database.Shared;
using Procon.Database.Shared.Builders.Methods.Data;
using Procon.Database.Shared.Builders.Values;

namespace Procon.Core.Shared.Database {
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
            CollectionValue value = data as CollectionValue;

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
            return DatabaseModel<T>.FromQuery(data).FirstOrDefault();
        }

        /// <summary>
        /// Serializes a model to a Save query to issue to the database.
        /// </summary>
        /// <returns>A new save query with property assignments attached</returns>
        public Save ToSaveQuery() {
            Save save = new Save();

            save.AddRange(new DocumentValue().FromJObject(JObject.FromObject(this)));

            return save;
        }

        /// <summary>
        /// Probably not as useful as Save since this would set all variables within a document, not as useful for DateTime fields.
        /// </summary>
        /// <returns>A new modify query with property assignments attached</returns>
        public Modify ToModifyQuery() {
            Modify modify = new Modify();

            modify.AddRange(new DocumentValue().FromJObject(JObject.FromObject(this)));

            return modify;
        }
    }
}
