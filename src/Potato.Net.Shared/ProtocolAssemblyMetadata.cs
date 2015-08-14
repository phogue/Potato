#region Copyright
// Copyright 2015 Geoff Green.
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
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Potato.Net.Shared.Serialization;

namespace Potato.Net.Shared {
    /// <summary>
    /// Wraps an assembly reference and supported protocol types.
    /// </summary>
    public class ProtocolAssemblyMetadata : IProtocolAssemblyMetadata {
        public string Name { get; set; }

        public FileInfo Assembly { get; set; }

        public FileInfo Meta { get; set; }

        public DirectoryInfo Directory { get; set; }

        public List<IProtocolType> ProtocolTypes { get; set; }

        /// <summary>
        /// Initalizes meta data with default values.
        /// </summary>
        public ProtocolAssemblyMetadata() {
            ProtocolTypes = new List<IProtocolType>();
        }

        /// <summary>
        /// Loads directory into a new assembly meta data object
        /// </summary>
        /// <param name="path">The directory path to load the meta data from.</param>
        /// <returns>True if the meta data loaded, false if an error occured or metadata/assembly is missing</returns>
        public bool Load(string path) {
            Directory = new DirectoryInfo(path);
            Name = Directory.Name;

            Assembly = new FileInfo(Path.Combine(Directory.FullName, Name + ".dll"));
            Meta = new FileInfo(Path.Combine(Directory.FullName, Name + ".json"));

            return Load();
        }

        /// <summary>
        /// Checks and loads existing information in the object 
        /// </summary>
        /// <returns></returns>
        public bool Load() {
            var loaded = true;

            if (Assembly != null && Assembly.Exists == true && Meta != null && Meta.Exists == true) {
                try {
                    using (var stream = new StreamReader(Meta.FullName)) {
                        using (JsonReader reader = new JsonTextReader(stream)) {
                            JsonSerialization.Minimal.Populate(reader, this);
                        }
                    }
                }
                catch {
                    loaded = false;
                }
            }
            else {
                loaded = false;
            }

            return loaded;
        }
    }
}
