using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Procon.Core.Shared.Events;
using Procon.Net.Shared.Serialization;

namespace Procon.Core.Shared.Serialization {
    /// <summary>
    /// Handles serialization within the Procon.Core.* assembly to json, with
    /// all converters so all objects/interfaces can be serialized within this 
    /// assembly.
    /// </summary>
    /// <remarks><para>This implementation is based on on the networking serialization</para></remarks>
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
            JsonSerialization.Converters = Net.Shared.Serialization.JsonSerialization.Converters.Union(new List<JsonConverter>() {
                new InterfaceJsonConverter<IGenericEvent, GenericEvent>(),
                new InterfaceJsonConverter<ICommandData, CommandData>(),
                new InterfaceJsonConverter<ICommandResult, CommandResult>(),
                new InterfaceJsonConverter<ICommand, Command>(),
                new InterfaceJsonConverter<ICommandParameter, CommandParameter>()
            }).ToList();

            JsonSerialization.Minimal = Net.Shared.Serialization.JsonSerialization.Minimal;
            JsonSerialization.Readable = Net.Shared.Serialization.JsonSerialization.Readable;

            JsonSerialization.Minimal.Converters.Clear();
            JsonSerialization.Readable.Converters.Clear();

            JsonSerialization.Converters.ForEach(converter => {
                JsonSerialization.Minimal.Converters.Add(converter);
                JsonSerialization.Readable.Converters.Add(converter);
            });
        }
    }
}
