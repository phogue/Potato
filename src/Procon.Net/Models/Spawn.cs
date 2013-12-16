using System;

namespace Procon.Net.Models {
    [Serializable]
    public sealed class Spawn : NetworkObject {

        /// <summary>
        /// The player who spawned in
        /// </summary>
        public Player Player { get; set; }

        /// <summary>
        /// What role the player spawned in as.
        /// </summary>
        public Role Role { get; set; }

        /// <summary>
        /// The items the player spawned in with
        /// </summary>
        public Inventory Inventory { get; set; }
    }
}
