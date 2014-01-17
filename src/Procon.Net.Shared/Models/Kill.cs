using System;
using System.Collections.Generic;

namespace Procon.Net.Shared.Models {
    [Serializable]
    public sealed class Kill : NetworkModel {

        public Kill() : base() {
            // Target
            this.Scope.Content = new List<String>();
            this.Scope.Players = new List<Player>();
            this.Scope.Items = new List<Item>();
            this.Scope.HumanHitLocations = new List<HumanHitLocation>();

            // Killer
            this.Now.Players = new List<Player>();
            this.Now.Items = new List<Item>();
        }
    }
}
