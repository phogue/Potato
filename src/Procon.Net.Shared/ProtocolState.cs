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
        public List<PlayerModel> Players { get; set; }

        public List<MapModel> Maps { get; set; }

        public List<BanModel> Bans { get; set; }

        public List<MapModel> MapPool { get; set; }

        public List<GameModeModel> GameModePool { get; set; }

        public List<GroupingModel> Groupings { get; set; }

        public List<ItemModel> Items { get; set; } 

        public Settings Settings { get; set; }

        public ITruth Support { get; set; }

        /// <summary>
        /// Initializes the gamestate with the default values.
        /// </summary>
        public ProtocolState() {
            this.Players = new List<PlayerModel>();
            this.Maps = new List<MapModel>();
            this.Bans = new List<BanModel>();

            this.MapPool = new List<MapModel>();
            this.GameModePool = new List<GameModeModel>();
            this.Groupings = new List<GroupingModel>();
            this.Items = new List<ItemModel>();

            this.Settings = new Settings();

            this.Support = new Tree();
        }
    }
}