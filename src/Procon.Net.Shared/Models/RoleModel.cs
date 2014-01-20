using System;

namespace Procon.Net.Shared.Models {
    /// <summary>
    /// A role, describing a player
    /// </summary>
    [Serializable]
    public sealed class RoleModel : NetworkModel {
        /// <summary>
        /// The name of the role the player has taken
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// Initializes the role with empty values.
        /// </summary>
        public RoleModel() : base() {
            this.Name = String.Empty;
        }
    }
}
