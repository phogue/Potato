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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Procon.Net.Shared;
using Procon.Net.Shared.Models;
using Procon.Net.Shared.Serialization;

namespace Procon.Net {
    /// <summary>
    /// A config loader used by a protocol to descibe additional meta data.
    /// </summary>
    [Serializable]
    public class ProtocolConfigLoader {
        /// <summary>
        /// Proxy load config that first builds the path to the config file.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="gameConfigPath"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static T Load<T>(String gameConfigPath, IProtocolType type) where T : ProtocolConfigModel {
            return ProtocolConfigLoader.Load<T>(ProtocolConfigLoader.Path(gameConfigPath, type.Provider, type.Type));
        }

        /// <summary>
        /// Populates references from other smaller objects, overwriting existing objects.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="config"></param>
        /// <returns></returns>
        public static T Populate<T>(T config) where T : ProtocolConfigModel {
            if (config.GameModes != null && config.Groups != null) {
                foreach (GameModeModel mode in config.GameModes) {
                    mode.DefaultGroups = config.Groups.Where(known => mode.DefaultGroups.Any(group => group.Uid == known.Uid && group.Type == known.Type)).ToList();
                }
            }

            if (config.MapPool != null && config.GameModes != null) {
                foreach (MapModel map in config.MapPool) {
                    map.GameMode = config.GameModes.FirstOrDefault(known => known.Name == map.GameMode.Name);

                    if (config.Groups != null) {
                        // Load in all default groupings, except for those explicitly defined.
                        map.Groups = map.Groups.Union(config.Groups.Where(existing => map.Groups.Any(group => group.Uid == existing.Uid && group.Type == existing.Type) == false)).ToList();
                    }
                }
            }

            return config;
        }

        /// <summary>
        /// Loads and deserializes a config file into a new object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configPath"></param>
        /// <returns></returns>
        public static T Load<T>(String configPath) where T : ProtocolConfigModel {
            T config = default(T);

            try {
                if (File.Exists(configPath) == true) {
                    using (var stream = new StreamReader(configPath)) {
                        using (JsonReader reader = new JsonTextReader(stream)) {
                            config = JsonSerialization.Minimal.Deserialize<T>(reader);
                        }
                    }

                    config = ProtocolConfigLoader.Populate(config);
                }
            }
            catch {
                config = default(T);
            }

            return config;
        }

        /// <summary>
        /// Ensures the directory exists and combines the config path
        /// </summary>
        /// <param name="gameConfigPath">The path to the config directory within Procon</param>
        /// <param name="protocolProvider">The name of the protocol provider (Myrcon)</param>
        /// <param name="protocolName">The name of the protocol (BF_4, BF_3)</param>
        /// <returns>The combined path of the config file</returns>
        public static String Path(String gameConfigPath, String protocolProvider, String protocolName) {
            String configPath = null;

            try {
                Directory.CreateDirectory(gameConfigPath);

                configPath = System.IO.Path.Combine(System.IO.Path.Combine(gameConfigPath, protocolProvider), protocolName + ".json");
            }
            catch {
                configPath = null;
            }

            return configPath;
        }
    }
}
