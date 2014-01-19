using System.Collections.Generic;
using Newtonsoft.Json;
using Procon.Net.Shared.Actions;

namespace Procon.Net.Shared.Serialization {
    /// <summary>
    /// Handles serialization within the Procon.Net.* assembly to json, with
    /// all converters so all objects/interfaces can be serialized within this 
    /// assembly.
    /// </summary>
    public static class Json {
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

        static Json() {
            Json.Converters = new List<JsonConverter>() {
                new InterfaceJsonConverter<IPacket, Packet>(),
                new InterfaceJsonConverter<INetworkAction, NetworkAction>(),
                new InterfaceJsonConverter<IClientEventData, ClientEventData>(),
                new InterfaceJsonConverter<IProtocolEventData, ProtocolEventData>()
            };

            Json.Minimal = new JsonSerializer();
            Json.Readable = new JsonSerializer();

            Json.Converters.ForEach(converter => {
                Json.Minimal.Converters.Add(converter);
                Json.Readable.Converters.Add(converter);
            });
        }
    }
}
