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
        /// Use default serialization
        /// </summary>
        public override bool CanWrite {
            get {
                return false;
            }
        }

        /// <summary>
        /// Converts type I to type C
        /// </summary>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) {
            var value = new C();

            serializer.Populate(JObject.Load(reader).CreateReader(), value);

            return value;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) {
            throw new NotImplementedException();
        }
    }
}
