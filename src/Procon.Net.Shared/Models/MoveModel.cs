using System;
using System.Collections.Generic;

namespace Procon.Net.Shared.Models {
    /// <summary>
    /// A description of player movement, initiated by the player, server or procon.
    /// </summary>
    [Serializable]
    public sealed class MoveModel : NetworkModel {
        /// <summary>
        /// Initializes the underlying networkmodel with the required collections.
        /// </summary>
        public MoveModel() {
            this.Scope.Players = new List<PlayerModel>();
            this.Then.Groups = new List<GroupModel>();
            this.Now.Groups = new List<GroupModel>();
        }
    }
}
