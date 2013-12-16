using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using Procon.Net.Actions;
using Procon.Net.Models;
using Procon.Net.Utils;

namespace Procon.Net {
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

        /// <summary>
        /// Parses this config into a game object.
        /// </summary>
        /// <param name="game">The game to load this config into</param>
        public virtual void Parse(IGame game) {
            game.State.MapPool = this.MapPool;
            game.State.GameModePool = this.GameModePool;

            game.State.MapPool.ForEach(map => map.ActionType = NetworkActionType.NetworkMapPooled);
        }

        /// <summary>
        /// Proxy load config that first builds the path to the config file.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="gameConfigPath"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static T Load<T>(String gameConfigPath, IGameType type) where T : GameConfig {
            return GameConfig.Load<T>(GameConfig.Path(gameConfigPath, type.Provider, type.Type));
        }

        /// <summary>
        /// Loads and deserializes a config file into a new object
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="configPath"></param>
        /// <returns></returns>
        public static T Load<T>(String configPath) where T : GameConfig {
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
        /// <param name="gameConfigPath">The path to the config directory within Procon</param>
        /// <param name="protocolProvider">The name of the protocol provider (Myrcon)</param>
        /// <param name="protocolName">The name of the protocol (BF_4, BF_3)</param>
        /// <returns>The combined path of the config file</returns>
        public static String Path(String gameConfigPath, String protocolProvider, String protocolName) {
            String configPath = null;

            try {
                Directory.CreateDirectory(gameConfigPath);

                configPath = System.IO.Path.Combine(System.IO.Path.Combine(gameConfigPath, protocolProvider), protocolName + ".xml");
            }
            catch {
                configPath = null;
            }

            return configPath;
        }
    }
}
