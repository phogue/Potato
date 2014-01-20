using System;
using System.Collections.Generic;

namespace Procon.Net.Shared.Models {
    /// <summary>
    /// A description of kicking/removing a player from the server
    /// </summary>
    [Serializable]
    public sealed class KickModel : NetworkModel {
        /// <summary>
        /// Initializes the underlying networkmodel with the required collections.
        /// </summary>
        public KickModel() : base() {
            this.Scope.Content = new List<String>();
            this.Scope.Players = new List<PlayerModel>();
        }
    }
}
