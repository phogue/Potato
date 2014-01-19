using System.Collections.Generic;

namespace Procon.Net.Shared.Models {
    /// <summary>
    /// A config used by a protocol to descibe additional meta data.
    /// </summary>
    public class ProtocolConfigModel : NetworkModel {
        /// <summary>
        /// List of available maps for this game
        /// </summary>
        public List<Map> MapPool { get; set; }

        /// <summary>
        /// List of available game modes for this game.
        /// </summary>
        public List<GameMode> GameModes { get; set; }

        /// <summary>
        /// List of groupings for this game
        /// </summary>
        public List<Grouping> Groupings { get; set; }

        /// <summary>
        /// List of items for this game.
        /// </summary>
        public List<Item> Items { get; set; }

        /// <summary>
        /// Parses this config into a game object.
        /// </summary>
        /// <param name="game">The game to load this config into</param>
        public virtual void Parse(IProtocol game) {
            game.State.MapPool = this.MapPool ?? new List<Map>();
            game.State.GameModePool = this.GameModes ?? new List<GameMode>();
            game.State.Groupings = this.Groupings ?? new List<Grouping>();
            game.State.Items = this.Items ?? new List<Item>();
        }
    }
}
