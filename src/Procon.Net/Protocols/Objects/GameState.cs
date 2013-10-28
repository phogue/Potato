using System;
using System.Collections.Generic;

namespace Procon.Net.Protocols.Objects {
    [Serializable]
    public class GameState {
        public GameState() {
            this.PlayerList = new PlayerList();
            this.MapList = new MapList();
            this.BanList = new BanList();

            this.MapPool = new List<Map>();
            this.GameModePool = new List<GameMode>();

            this.Settings = new Settings();
        }

        /// <summary>
        /// All current information about each player in the server
        /// </summary>
        public PlayerList PlayerList { get; set; }

        /// <summary>
        /// The current maplist
        /// </summary>
        public MapList MapList { get; set; }

        /// <summary>
        /// The current banlist
        /// </summary>
        public BanList BanList { get; set; }

        /// <summary>
        /// List of available maps for this game
        /// </summary>
        public List<Map> MapPool { get; set; }

        /// <summary>
        /// List of available game modes for this game.
        /// </summary>
        public List<GameMode> GameModePool { get; set; }

        /// <summary>
        /// Various settings that are sent by the server.
        /// </summary>
        public Settings Settings { get; set; }
    }
}