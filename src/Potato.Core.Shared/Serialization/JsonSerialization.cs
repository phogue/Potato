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
using Newtonsoft.Json;
using Potato.Core.Shared.Events;
using Potato.Net.Shared.Serialization;

namespace Potato.Core.Shared.Serialization {
    /// <summary>
    /// Handles serialization within the Potato.Core.* assembly to json, with
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
                new InterfaceJsonConverter<IConfigCommand, ConfigCommand>(),
                new InterfaceJsonConverter<ICommandParameter, CommandParameter>(),
                new InterfaceJsonConverter<ICommandDispatch, CommandDispatch>()
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
