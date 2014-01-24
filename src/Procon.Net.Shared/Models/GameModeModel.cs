using System;
using System.Collections.Generic;

namespace Procon.Net.Shared.Models {
    /// <summary>
    /// A description of a game mode currently being played or attached to a map
    /// </summary>
    [Serializable]
    public sealed class GameModeModel : NetworkModel {
        /// <summary>
        /// This game mode's name as it is used via Rcon.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// This game mode's human-readable name.
        /// </summary>
        public string FriendlyName { get; set; }

        /// <summary>
        /// This game mode's number of teams, not including spectator/neutral.
        /// </summary>
        public int TeamCount { get; set; }

        /// <summary>
        /// List of groups to default a player to when moving them and such.
        /// </summary>
        public List<GroupModel> DefaultGroups { get; set; }

        /// <summary>
        /// Initializes the game mode with the default values.
        /// </summary>
        public GameModeModel() {
            this.Name = String.Empty;
            this.FriendlyName = String.Empty;
            this.TeamCount = 0;
        }
    }
}
