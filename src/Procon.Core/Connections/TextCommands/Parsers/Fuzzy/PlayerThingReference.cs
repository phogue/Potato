using System.Collections.Generic;
using System.Linq;
using Procon.Fuzzy.Tokens.Object;
using Procon.Net.Shared.Models;

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
            return other is PlayerThingReference || other is LocationThingReference || other is ItemThingReference;
        }

        public IThingReference Union(IThingReference other) {
            PlayerThingReference playerThingReference = other as PlayerThingReference;
            LocationThingReference locationThingReference = other as LocationThingReference;
            ItemThingReference itemThingReference = other as ItemThingReference;

            if (playerThingReference != null) {
                // Players and Players
                this.Players.AddRange(playerThingReference.Players.Where(player => this.Players.Contains(player) == false));
            }
            else if (locationThingReference != null) {
                // All players from australia
                this.Players.RemoveAll(player => locationThingReference.Locations.Any(location => location.CountryName != player.Location.CountryName));
            }
            else if (itemThingReference != null) {
                // All players using sniper rifle
                this.Players.RemoveAll(player => itemThingReference.Items.Any(item => player.Inventory.Items.Any(playerItem => item.Name == playerItem.Name)) == false);
            }

            return this;
        }

        public IThingReference Complement(IThingReference other) {
            PlayerThingReference playerThingReference = other as PlayerThingReference;
            LocationThingReference locationThingReference = other as LocationThingReference;
            ItemThingReference itemThingReference = other as ItemThingReference;

            if (playerThingReference != null) {
                // Players excluding Players
                this.Players.RemoveAll(player => playerThingReference.Players.Contains(player));
            }
            else if (locationThingReference != null) {
                // All players not from australia
                this.Players.RemoveAll(player => locationThingReference.Locations.Any(location => location.CountryName == player.Location.CountryName));
            }
            else if (itemThingReference != null) {
                // All players not using sniper rifle
                this.Players.RemoveAll(player => itemThingReference.Items.Any(item => player.Inventory.Items.Any(playerItem => item.Name == playerItem.Name)));
            }

            return this;
        }
    }
}
