using System;
using System.Collections.Generic;
using Procon.Net.Models;

namespace Procon.Net.Actions {
    [Serializable]
    public sealed class Ban : NetworkAction {

        /// <summary>
        /// The time parameters to ban the player
        /// </summary>
        public TimeSubset Time { get; set; }

        public Ban() : base() {
            this.Scope.Content = new List<String>();
            this.Scope.Players = new List<Player>();

            this.Time = new TimeSubset();
        }
    }
}
