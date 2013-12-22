using System;
using System.Collections.Generic;
using Procon.Net.Shared.Models;

namespace Procon.Net.Shared.Actions {
    [Serializable]
    public sealed class Kick : NetworkAction {

        public Kick() : base() {
            // Target
            this.Scope.Content = new List<String>();
            this.Scope.Players = new List<Player>();
        }
    }
}
