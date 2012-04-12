// Copyright 2011 Geoffrey 'Phogue' Green
// 
// http://www.phogue.net
//  
// This file is part of Procon 2.
// 
// Procon 2 is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// Procon 2 is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Procon 2.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Net.Sockets;
using System.Reflection;
using System.ComponentModel;
using System.IO;
using System.Xml.Linq;

namespace Procon.Net {
    using Procon.Net.Attributes;
    using Procon.Net.Protocols;
    using Procon.Net.Protocols.Objects;
    using Procon.Net.Utils;
    using Procon.Net.Utils.PunkBuster;
    using Procon.Net.Utils.PunkBuster.Objects;

    public abstract class Game : IGame
    {
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public GameState State { get; protected set; }

        public abstract string Hostname { get; }
        public abstract ushort Port { get; }
        public string Password { get; set; }

        public string m_additional;
        public string Additional {
            get {
                return this.m_additional;
            }
            set {
                this.m_additional = value;

                // This way does not require System.Web reference.
                foreach (string item in this.m_additional.Split('&')) {
                    string[] kvp = item.Split('=');

                    if (kvp.Length == 2) {
                        PropertyInfo property = this.GetType().GetProperty(kvp[0], BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);

                        if (property != null) {

                            property.SetValue(this, System.Convert.ChangeType(Uri.UnescapeDataString(kvp[1]), property.PropertyType), null);
                        }
                    }
                }

                /*
                NameValueCollection queryString = HttpUtility.ParseQueryString(this.m_additional);

                foreach (string key in queryString.AllKeys) {
                    PropertyInfo property = this.GetType().GetProperty(key);

                    if (property != null) {
                        property.SetValue(this, queryString[key], null);
                    }
                }
                */
            }
        }
        
        //public abstract ConnectionState ConnectionState { get; }

        #region Events

        public delegate void GameEventHandler(Game sender, GameEventArgs e);
        public virtual event GameEventHandler GameEvent;

        public delegate void ClientEventHandler(Game sender, ClientEventArgs e);
        public virtual event ClientEventHandler ClientEvent;

        public delegate void ConnectionStateChangedHandler(Game sender, ConnectionState newState);

        #endregion

        public Game() {
        }

        #region Helper Methods

        public abstract void Raw(string format, params object[] args);

        // These may get transfered to a Interface used by Game and PacketFactory
        public abstract void Login(string password);

        public abstract void Action(ProtocolObject action);

        #endregion

        #region Core

        public abstract void Send(Packet packet);
        public abstract void AttemptConnection();
        public abstract void Shutdown();

        public abstract void Synchronize();

        #endregion

        #region Reflected Game Types

        private static Dictionary<GameType, Type> SUPPORTED_GAMES = null;
        private static List<Assembly> SUPPORTED_GAMES_ASSEMBLY = null;

        /**
         * Late loads Procon.Net.Protocols.*.dll's 
         */
        private static void LateBindGames() {

            Game.SUPPORTED_GAMES_ASSEMBLY = new List<Assembly>() {
                Assembly.GetAssembly(typeof(Game))
            };

            foreach (string protocol in Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "Procon.Net.Protocols.*.dll")) {
                try {
                    Game.SUPPORTED_GAMES_ASSEMBLY.Add(Assembly.LoadFile(protocol));
                }
                catch (Exception) { }
            }
        }

        public static Dictionary<GameType, Type> GetSupportedGames() {

            Dictionary<GameType, Type> games = Game.SUPPORTED_GAMES;

            if (games == null) {
                
                // Load the supported games
                Game.LateBindGames();

                Regex supportedGamesNamespame = new Regex(@"^Procon\.Net\.Protocols.*");

                Game.SUPPORTED_GAMES = games = (from gameClassType in Game.SUPPORTED_GAMES_ASSEMBLY.SelectMany(x=> x.GetTypes()) //Assembly.GetAssembly(typeof(Game)).GetTypes()
                                                let gameType = (gameClassType.GetCustomAttributes(typeof(GameAttribute), false) as IEnumerable<GameAttribute>).FirstOrDefault()
                                                where gameType != null &&
                                                      gameClassType != null &&
                                                      gameClassType.IsClass == true &&
                                                      gameClassType.IsAbstract == false &&
                                                      gameClassType.Namespace != null &&
                                                      supportedGamesNamespame.IsMatch(gameClassType.Namespace) == true &&
                                                      typeof(Game).IsAssignableFrom(gameClassType)
                                                select new {
                                                    Name = gameType.GameType,
                                                    Type = gameClassType
                                                }).ToDictionary(w => w.Name, w => w.Type);
            }

            return games;
        }

        #endregion

        #region Game Config Loading

        /// <summary>
        /// Config holding all games loaded from /Configs/Games/
        /// </summary>
        protected static XDocument GameConfig { get; set; }

        public string m_gameConfigPath;
        public string GameConfigPath {
            get {
                return this.m_gameConfigPath;
            }
            set {
                if (this.m_gameConfigPath != value) {
                    Game.GameConfig = null;

                    this.m_gameConfigPath = value;

                    this.LoadGameConfigs();
                }
            }
        }

        protected void LoadGameConfigs() {

            lock (this) {
                if (Game.GameConfig == null) {

                    try {
                        if (Directory.Exists(this.GameConfigPath) == false) {
                            Directory.CreateDirectory(this.GameConfigPath);
                        }

                        foreach (FileInfo file in new DirectoryInfo(this.GameConfigPath).GetFiles("*.xml")) {
                            try {
                                XDocument game = XDocument.Load(file.OpenText());

                                if (Game.GameConfig == null) {
                                    Game.GameConfig = game;
                                }
                                else {
                                    if (Game.GameConfig.Descendants("games").FirstOrDefault() != null && game.Descendants("games").FirstOrDefault() != null) {
                                        Game.GameConfig.Descendants("games").FirstOrDefault().Add(game.Descendants("games").FirstOrDefault());
                                    }
                                }
                            }
                            catch (Exception) {
                                Console.WriteLine("Error loading {0}", file.FullName);
                            }
                        }
                    }
                    catch (Exception) { }
                }
            }
        }

        #endregion

    }
}
