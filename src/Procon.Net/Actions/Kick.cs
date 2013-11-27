using System;
using System.Collections.Generic;
using Procon.Net.Data;

namespace Procon.Net.Actions {
    [Serializable]
    public sealed class Kick : NetworkAction {

        public Kick() : base() {
            // Target
            this.Scope.Content = new List<String>();
            this.Scope.Players = new List<Player>();
        }
    }
}
