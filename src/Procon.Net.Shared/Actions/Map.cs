using System;
using Procon.Net.Shared.Models;

namespace Procon.Net.Shared.Actions {

    [Serializable]
    public sealed class Map : NetworkAction {

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
        public GameMode GameMode { get; set; }
    }
}
