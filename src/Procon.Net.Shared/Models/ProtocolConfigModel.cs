using System;
using System.Collections.Generic;

namespace Procon.Net.Shared.Models {
    /// <summary>
    /// A config used by a protocol to descibe additional meta data.
    /// </summary>
    [Serializable]
    public class ProtocolConfigModel : NetworkModel {
        /// <summary>
        /// List of available maps for this game
        /// </summary>
        public List<MapModel> MapPool { get; set; }

        /// <summary>
        /// List of available game modes for this game.
        /// </summary>
        public List<GameModeModel> GameModes { get; set; }

        /// <summary>
        /// List of groupings for this game
        /// </summary>
        public List<GroupingModel> Groupings { get; set; }

        /// <summary>
        /// List of items for this game.
        /// </summary>
        public List<ItemModel> Items { get; set; }

        /// <summary>
        /// Parses this config into a game object.
        /// </summary>
        /// <param name="game">The game to load this config into</param>
        public virtual void Parse(IProtocol game) {
            game.State.MapPool = this.MapPool ?? new List<MapModel>();
            game.State.GameModePool = this.GameModes ?? new List<GameModeModel>();
            game.State.Groupings = this.Groupings ?? new List<GroupingModel>();
            game.State.Items = this.Items ?? new List<ItemModel>();
        }
    }
}
