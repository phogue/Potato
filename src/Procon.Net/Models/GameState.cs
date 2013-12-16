using System;
using System.Collections.Generic;
using Procon.Net.Actions;
using Procon.Net.Collections;

namespace Procon.Net.Models {
    [Serializable]
    public class GameState {
        public GameState() {
            this.Players = new Players();
            this.Maps = new Maps();
            this.Bans = new Bans();

            this.MapPool = new List<Map>();
            this.GameModePool = new List<GameMode>();
            this.Groupings = new List<Grouping>();

            this.Settings = new Settings();
        }

        /// <summary>
        /// All current information about each player in the server
        /// </summary>
        public Players Players { get; set; }

        /// <summary>
        /// The current maplist
        /// </summary>
        public Maps Maps { get; set; }

        /// <summary>
        /// The current banlist
        /// </summary>
        public Bans Bans { get; set; }

        /// <summary>
        /// List of available maps for this game
        /// </summary>
        public List<Map> MapPool { get; set; }

        /// <summary>
        /// List of available game modes for this game.
        /// </summary>
        public List<GameMode> GameModePool { get; set; }

        /// <summary>
        /// List of potential groups available 
        /// </summary>
        public List<Grouping> Groupings { get; set; }

        /// <summary>
        /// List of potential items available in this game.
        /// </summary>
        public List<Item> Items { get; set; } 

        /// <summary>
        /// Various settings that are sent by the server.
        /// </summary>
        public Settings Settings { get; set; }
    }
}