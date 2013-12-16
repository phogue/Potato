using System.Collections.Generic;
using System.Linq;
using Procon.Fuzzy.Tokens.Object;
using Procon.Net.Models;

namespace Procon.Core.Connections.TextCommands.Parsers.Fuzzy {
    /// <summary>
    /// A reference for Procon.Fuzzy to use for player information
    /// </summary>
    public class PlayerThingReference : IThingReference {

        /// <summary>
        /// The player attached to this thing.
        /// </summary>
        public List<Player> Players { get; set; }

        public bool CompatibleWith(IThingReference other) {
            return other is PlayerThingReference || other is LocationThingReference;
        }

        public IThingReference Union(IThingReference other) {
            PlayerThingReference playerThingReference = other as PlayerThingReference;
            LocationThingReference locationThingReference = other as LocationThingReference;

            if (playerThingReference != null) {
                // Players and Players
                this.Players.AddRange(playerThingReference.Players.Where(player => this.Players.Contains(player) == false));
            }
            else if (locationThingReference != null) {
                // All players from australia
                this.Players.RemoveAll(player => locationThingReference.Locations.Any(location => location.CountryName != player.Location.CountryName));
            }

            return this;
        }

        public IThingReference Complement(IThingReference other) {
            PlayerThingReference playerThingReference = other as PlayerThingReference;
            LocationThingReference locationThingReference = other as LocationThingReference;

            if (playerThingReference != null) {
                // Players excluding Players
                this.Players.RemoveAll(player => playerThingReference.Players.Contains(player));
            }
            else if (locationThingReference != null) {
                // All players not from australia
                this.Players.RemoveAll(player => locationThingReference.Locations.Any(location => location.CountryName == player.Location.CountryName));

            }

            return this;
        }
    }
}
