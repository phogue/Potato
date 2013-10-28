using System;
using System.Xml.Linq;

namespace Procon.Net.Protocols.Objects {
    using Procon.Net.Utils;

    [Serializable]
    public sealed class GameMode : NetworkObject {

        public GameMode() {
            this.Name = String.Empty;
            this.FriendlyName = String.Empty;
            this.TeamCount = 0;
        }

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
        public GroupingList DefaultGroups { get; set; }

        /// <summary>Deserializes game mode information received via a network.</summary>
        public GameMode Deserialize(XElement element) {
            Name = element.ElementValue("name");
            FriendlyName = element.ElementValue("friendlyname");
            TeamCount = int.Parse(element.ElementValue("teamcount"));
            return this;
        }
    }
}
