using System;
using System.Collections.Generic;

namespace Procon.Net.Shared.Models {
    /// <summary>
    /// A map within the protocol/game
    /// </summary>
    [Serializable]
    public sealed class MapModel : NetworkModel {
        /// <summary>
        /// This map's order as represented by a 0-based index.
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// This map's number of times it repeats before ending.
        /// </summary>
        public int Rounds { get; set; }

        /// <summary>
        /// This map's name as it is used via Rcon.
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// This map's human-readable name.
        /// </summary>
        public String FriendlyName { get; set; }

        /// <summary>
        /// This map's game mode.
        /// </summary>
        public GameModeModel GameMode { get; set; }

        /// <summary>
        /// This maps possible groups
        /// </summary>
        public List<GroupModel> Groups { get; set; }

        /// <summary>
        /// Initializes the map model with default values.
        /// </summary>
        public MapModel() {
            this.Groups = new List<GroupModel>();
        }
    }
}
