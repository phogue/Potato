using System;
using System.IO;
using Newtonsoft.Json;

namespace Procon.Net.Shared.Serialization {
    /// <summary>
    /// Provides some easier methods for serializing/deserializing json strings
    /// </summary>
    public static class JsonSerializerExtensions {
        /// <summary>
        /// Serializes and returns a string from a JsonSerializer class
        /// </summary>
        public static String Serialize(this JsonSerializer serializer, Object value) {
            String data = "";

            using (var writer = new StringWriter()) {
                serializer.Serialize(writer, value);
                data = writer.ToString();
            }

            return data;
        }

        /// <summary>
        /// Deserializes from a string on a JsonSerializer class and returns the new object.
        /// </summary>
        public static T Deserialize<T>(this JsonSerializer serializer, String data) {
            T value = default(T);

            using (var text = new StringReader(data)) {
                using (var reader = new JsonTextReader(text)) {
                    value = serializer.Deserialize<T>(reader);
                }
            }

            return value;
        }
    }
}
