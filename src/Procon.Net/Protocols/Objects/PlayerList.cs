using System;
using System.Collections.Generic;

namespace Procon.Net.Protocols.Objects {
    [Serializable]
    public class PlayerList : List<Player> {
        /// <summary>
        /// What range this playerlist object is holding
        /// </summary>
        public GroupingList Subset { get; set; }
    }
}
