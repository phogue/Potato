using System;

namespace Procon.Net.Shared.Models {
    /// <summary>
    /// A description of a player creation
    /// </summary>
    [Serializable]
    public sealed class SpawnModel : NetworkModel {
        /// <summary>
        /// The player who spawned in
        /// </summary>
        public PlayerModel Player { get; set; }

        /// <summary>
        /// What role the player spawned in as.
        /// </summary>
        public RoleModel Role { get; set; }

        /// <summary>
        /// The items the player spawned in with
        /// </summary>
        public InventoryModel Inventory { get; set; }
    }
}
