using System;
using System.Collections.Generic;

namespace Procon.Net.Shared.Models {
    [Serializable]
    public sealed class BanModel : NetworkModel {

        public BanModel() : base() {
            this.Scope.Content = new List<String>();
            this.Scope.Players = new List<PlayerModel>();
            this.Scope.Times = new List<TimeSubsetModel>();
        }
    }
}
