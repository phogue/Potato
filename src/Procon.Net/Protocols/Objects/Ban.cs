using System;
using System.Collections.Generic;

namespace Procon.Net.Protocols.Objects {
    [Serializable]
    public sealed class Ban : NetworkAction {

        /// <summary>
        /// The time parameters to ban the player
        /// </summary>
        public TimeSubset Time { get; set; }

        public Ban() : base() {
            this.Scope.Players = new List<Player>();

            this.Time = new TimeSubset();
        }
    }
}
