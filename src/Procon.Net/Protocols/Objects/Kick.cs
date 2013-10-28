using System;

namespace Procon.Net.Protocols.Objects {
    [Serializable]
    public sealed class Kick : NetworkAction {

        /// <summary>
        /// The target player to kick
        /// </summary>
        public Player Target { get; set; }

        public Kick() : base() {
            this.Target = new Player();
        }
    }
}
