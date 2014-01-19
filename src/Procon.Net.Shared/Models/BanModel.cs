using System;
using System.Collections.Generic;

namespace Procon.Net.Shared.Models {
    /// <summary>
    /// Model describing a banned player
    /// </summary>
    [Serializable]
    public sealed class BanModel : NetworkModel {
        /// <summary>
        /// Initializes the underlying networkmodel with the required collections.
        /// </summary>
        public BanModel() : base() {
            this.Scope.Content = new List<String>();
            this.Scope.Players = new List<PlayerModel>();
            this.Scope.Times = new List<TimeSubsetModel>();
        }
    }
}
