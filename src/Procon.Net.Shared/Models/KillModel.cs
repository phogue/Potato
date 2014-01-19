using System;
using System.Collections.Generic;

namespace Procon.Net.Shared.Models {
    /// <summary>
    /// A description of a kill between server, procon or player(s)
    /// </summary>
    [Serializable]
    public sealed class KillModel : NetworkModel {
        /// <summary>
        /// Initializes the underlying networkmodel with the required collections.
        /// </summary>
        public KillModel() : base() {
            // Target
            this.Scope.Content = new List<String>();
            this.Scope.Players = new List<PlayerModel>();
            this.Scope.Items = new List<ItemModel>();
            this.Scope.HumanHitLocations = new List<HumanHitLocation>();

            // Killer
            this.Now.Players = new List<PlayerModel>();
            this.Now.Items = new List<ItemModel>();
        }
    }
}
