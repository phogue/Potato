using System;
using System.Collections.Generic;

namespace Procon.Net.Protocols.Objects {
    [Serializable]
    public sealed class Move : NetworkAction {

        public Move() {
            this.Scope.Players = new List<Player>();
            this.Then.Groups = new List<Grouping>();
            this.Now.Groups = new List<Grouping>();
        }
    }
}
