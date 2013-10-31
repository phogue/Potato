using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using Procon.Net.Utils;

namespace Procon.Net.Protocols.Objects {
    [Serializable]
    public class GameConfig {

        /// <summary>
        /// List of available maps for this game
        /// </summary>
        public List<Map> MapPool { get; set; }

        /// <summary>
        /// List of available game modes for this game.
        /// </summary>
        public List<GameMode> GameModePool { get; set; }

        public static T LoadConfig<T>(String configPath) where T : GameConfig {
            T config = default(T);

            try {
                if (File.Exists(configPath) == true) {
                    XDocument document = XDocument.Load(configPath);

                    config = document.Root.FromXElement<T>();
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
        /// <param name="gameConfigPath"></param>
        /// <param name="protocolProvider"></param>
        /// <param name="protocolName"></param>
        /// <returns></returns>
        public static String BuildConfigPath(String gameConfigPath, String protocolProvider, String protocolName) {
            String configPath = null;

            try {
                Directory.CreateDirectory(gameConfigPath);

                configPath = Path.Combine(Path.Combine(gameConfigPath, protocolProvider), protocolName + ".xml");
            }
            catch {
                configPath = null;
            }

            return configPath;
        }
    }
}
