using System;
using System.Collections.Generic;

namespace Procon.Net.Shared.Models {
    [Serializable]
    public sealed class Ban : NetworkModel {

        public Ban() : base() {
            this.Scope.Content = new List<String>();
            this.Scope.Players = new List<Player>();
            this.Scope.Times = new List<TimeSubset>();
        }
    }
}
