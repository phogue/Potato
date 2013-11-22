using System;
using System.Collections.Generic;

namespace Procon.Net.Protocols.Objects {
    [Serializable]
    public sealed class Kick : NetworkAction {

        public Kick() : base() {
            // Target
            this.Scope.Content = new List<String>();
            this.Scope.Players = new List<Player>();
        }
    }
}
