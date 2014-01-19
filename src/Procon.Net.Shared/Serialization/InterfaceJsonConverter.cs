using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Procon.Net.Shared.Serialization {
    /// <summary>
    /// A converter of a single interface to a concrete type. Useful if only a single (or default)
    /// concrete type is used.
    /// </summary>
    public class InterfaceJsonConverter<I,C> : JsonConverter where C : new() {
        public override bool CanConvert(Type objectType) {
            return typeof(I).IsAssignableFrom(objectType);
        }

        /// <summary>
        /// Converts type I to type C
        /// </summary>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
            var value = new C();

            serializer.Populate(JObject.Load(reader).CreateReader(), value);

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
