using System;
using System.Collections.Generic;

namespace Procon.Net.Shared.Models {
    [Serializable]
    public sealed class MoveModel : NetworkModel {

        public MoveModel() {
            this.Scope.Players = new List<PlayerModel>();
            this.Then.Groups = new List<GroupingModel>();
            this.Now.Groups = new List<GroupingModel>();
        }
    }
}
