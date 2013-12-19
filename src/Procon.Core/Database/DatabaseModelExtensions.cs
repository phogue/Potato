using Newtonsoft.Json.Linq;
using Procon.Database.Serialization.Builders.Methods;
using Procon.Database.Serialization.Builders.Values;

namespace Procon.Core.Database {
    /// <summary>
    /// Static serialization of a database model
    /// </summary>
    public static class DatabaseModelExtensions {
        /*
        /// <summary>
        /// Serializes a model to a Save query to issue to the database.
        /// </summary>
        /// <typeparam name="T">The type of model to serialize</typeparam>
        /// <param name="model">The model/data to serialize</param>
        /// <returns>A new save query with property assignments attached</returns>
        public static Save ToSaveQuery<T>(this DatabaseModel<T> model) where T : DatabaseModel<T>, new() {
            Save save = new Save();

            save.AddRange(new DocumentValue().FromJObject(JObject.FromObject(model)));

            return save;
        }

        /// <summary>
        /// Probably not as useful as Save since this would set all variables within a document, not as useful for DateTime fields.
        /// </summary>
        /// <typeparam name="T">The type of model to serialize</typeparam>
        /// <param name="model">The model/data to serialize</param>
        /// <returns>A new modify query with property assignments attached</returns>
        public static Modify ToModifyQuery<T>(this DatabaseModel<T> model) where T : DatabaseModel<T>, new() {
            Modify modify = new Modify();

            modify.AddRange(new DocumentValue().FromJObject(JObject.FromObject(model)));

            return modify;
        }
         * */
    }
}
