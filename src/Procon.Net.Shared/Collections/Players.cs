using System;
using System.Collections.Generic;
using Procon.Net.Shared.Models;

namespace Procon.Net.Shared.Collections {
    [Serializable]
    public class Players : List<Player> {
        /// <summary>
        /// What range this playerlist object is holding
        /// </summary>
        public Groupings Subset { get; set; }
    }
}
