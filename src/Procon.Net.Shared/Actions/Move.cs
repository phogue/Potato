using System;
using System.Collections.Generic;
using Procon.Net.Shared.Models;

namespace Procon.Net.Shared.Actions {
    [Serializable]
    public sealed class Move : NetworkAction {

        public Move() {
            this.Scope.Players = new List<Player>();
            this.Then.Groups = new List<Grouping>();
            this.Now.Groups = new List<Grouping>();
        }
    }
}
