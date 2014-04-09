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
