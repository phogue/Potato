using System;
using System.Collections.Generic;

namespace Procon.Net.Shared.Models {
    [Serializable]
    public sealed class KickModel : NetworkModel {

        public KickModel() : base() {
            // Target
            this.Scope.Content = new List<String>();
            this.Scope.Players = new List<PlayerModel>();
        }
    }
}
