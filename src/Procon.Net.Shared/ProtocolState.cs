using System;
using System.Collections.Generic;
using Procon.Net.Shared.Models;
using Procon.Net.Shared.Truths;

namespace Procon.Net.Shared {
    /// <summary>
    /// The current snapshot of the server with all details Procon has captured or inferred.
    /// </summary>
    [Serializable]
    public sealed class ProtocolState : IProtocolState {
        public List<Player> Players { get; set; }

        public List<Map> Maps { get; set; }

        public List<Ban> Bans { get; set; }

        public List<Map> MapPool { get; set; }

        public List<GameMode> GameModePool { get; set; }

        public List<Grouping> Groupings { get; set; }

        public List<Item> Items { get; set; } 

        public Settings Settings { get; set; }

        public ITruth Support { get; set; }

        /// <summary>
        /// Initializes the gamestate with the default values.
        /// </summary>
        public ProtocolState() {
            this.Players = new List<Player>();
            this.Maps = new List<Map>();
            this.Bans = new List<Ban>();

            this.MapPool = new List<Map>();
            this.GameModePool = new List<GameMode>();
            this.Groupings = new List<Grouping>();
            this.Items = new List<Item>();

            this.Settings = new Settings();

            this.Support = new Tree();
        }
    }
}