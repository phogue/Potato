using System;
using System.Collections.Generic;

namespace Procon.Net.Shared.Models {
    [Serializable]
    public sealed class Move : NetworkModel {

        public Move() {
            this.Scope.Players = new List<Player>();
            this.Then.Groups = new List<Grouping>();
            this.Now.Groups = new List<Grouping>();
        }
    }
}
