using System;
using System.Collections.Generic;

namespace Procon.Net.Shared.Models {
    [Serializable]
    public sealed class KillModel : NetworkModel {

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
