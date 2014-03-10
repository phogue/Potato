using System.Collections.Generic;
using Newtonsoft.Json;
using Procon.Net.Shared.Actions;

namespace Procon.Net.Shared.Serialization {
    /// <summary>
    /// Handles serialization within the Procon.Net.* assembly to json, with
    /// all converters so all objects/interfaces can be serialized within this 
    /// assembly.
    /// </summary>
    public static class JsonSerialization {
        /// <summary>
        /// Holds a serializer for producing transportable json
        /// </summary>
        public static readonly JsonSerializer Minimal;

        /// <summary>
        /// Holds a serializer for producing human readable json
        /// </summary>
        public static readonly JsonSerializer Readable;

        /// <summary>
        /// A list of converters 
        /// </summary>
        public static readonly List<JsonConverter> Converters;

        static JsonSerialization() {
            JsonSerialization.Converters = new List<JsonConverter>() {
                new InterfaceJsonConverter<IPacket, Packet>(),
                new InterfaceJsonConverter<INetworkAction, NetworkAction>(),
                new InterfaceJsonConverter<IClientEventData, ClientEventData>(),
                new InterfaceJsonConverter<IProtocolEventData, ProtocolEventData>(),
                new InterfaceJsonConverter<IProtocolEventArgs, ProtocolEventArgs>(),
                new InterfaceJsonConverter<IProtocolType, ProtocolType>(),
                new InterfaceJsonConverter<IProtocolState, ProtocolState>()
            };

            JsonSerialization.Minimal = new JsonSerializer() {
                NullValueHandling = NullValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore,
                Formatting = Formatting.None,
                DateTimeZoneHandling = DateTimeZoneHandling.Local
            };

            JsonSerialization.Readable = new JsonSerializer() {
                NullValueHandling = NullValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore,
                Formatting = Formatting.Indented,
                DateTimeZoneHandling = DateTimeZoneHandling.Local
            };

            JsonSerialization.Converters.ForEach(converter => {
                JsonSerialization.Minimal.Converters.Add(converter);
                JsonSerialization.Readable.Converters.Add(converter);
            });
        }
    }
}
