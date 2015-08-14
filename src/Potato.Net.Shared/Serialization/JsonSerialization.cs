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
using Newtonsoft.Json;
using Potato.Net.Shared.Actions;

namespace Potato.Net.Shared.Serialization {
    /// <summary>
    /// Handles serialization within the Potato.Net.* assembly to json, with
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
            Converters = new List<JsonConverter>() {
                new InterfaceJsonConverter<IPacket, Packet>(),
                new InterfaceJsonConverter<INetworkAction, NetworkAction>(),
                new InterfaceJsonConverter<IClientEventData, ClientEventData>(),
                new InterfaceJsonConverter<IProtocolEventData, ProtocolEventData>(),
                new InterfaceJsonConverter<IProtocolEventArgs, ProtocolEventArgs>(),
                new InterfaceJsonConverter<IProtocolType, ProtocolType>(),
                new InterfaceJsonConverter<IProtocolAssemblyMetadata, ProtocolAssemblyMetadata>(),
                new InterfaceJsonConverter<IProtocolState, ProtocolState>(),
                new InterfaceJsonConverter<IProtocolStateDifference, ProtocolStateDifference>(),
                new InterfaceJsonConverter<IProtocolStateData, ProtocolState>()
            };

            Minimal = new JsonSerializer() {
                NullValueHandling = NullValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore,
                Formatting = Formatting.None,
                DateTimeZoneHandling = DateTimeZoneHandling.Local
            };

            Readable = new JsonSerializer() {
                NullValueHandling = NullValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                DefaultValueHandling = DefaultValueHandling.Ignore,
                Formatting = Formatting.Indented,
                DateTimeZoneHandling = DateTimeZoneHandling.Local
            };

            Converters.ForEach(converter => {
                Minimal.Converters.Add(converter);
                Readable.Converters.Add(converter);
            });
        }
    }
}
