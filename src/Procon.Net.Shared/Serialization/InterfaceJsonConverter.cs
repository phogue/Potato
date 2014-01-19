using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Procon.Net.Shared.Serialization {
    /// <summary>
    /// A converter of a single interface to a concrete type. Useful if only a single (or default)
    /// concrete type is used.
    /// </summary>
    public class InterfaceJsonConverter<I,C> : JsonConverter {
        public override bool CanConvert(Type objectType) {
            return typeof(I).IsAssignableFrom(objectType);
        }

        /// <summary>
        /// Converts type I to type C
        /// </summary>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
            object value = null;

            JObject token = serializer.Deserialize<JToken>(reader) as JObject;

            if (token != null) {
                value = token.ToObject<C>();
            }

            return value;
        }

        /// <summary>
        /// Default write serialization (I think)
        /// </summary>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
            JToken.FromObject(value).WriteTo(writer);
        }
    }
}
