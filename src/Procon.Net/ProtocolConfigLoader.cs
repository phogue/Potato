using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Procon.Net.Shared;
using Procon.Net.Shared.Models;

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
            if (config.GameModes != null && config.Groupings != null) {
                foreach (GameMode mode in config.GameModes) {
                    mode.DefaultGroups = config.Groupings.Where(known => mode.DefaultGroups.Any(group => group.Uid == known.Uid && group.Type == known.Type)).ToList();
                }
            }

            if (config.MapPool != null && config.GameModes != null) {
                foreach (Map map in config.MapPool) {
                    map.GameMode = config.GameModes.FirstOrDefault(known => known.Name == map.GameMode.Name);
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
                    JsonSerializer serializer = new JsonSerializer();

                    using (JsonReader reader = new JsonTextReader(new StreamReader(configPath))) {
                        config = serializer.Deserialize<T>(reader);
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
