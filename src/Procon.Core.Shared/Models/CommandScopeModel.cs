using System;

namespace Procon.Core.Shared.Models {
    /// <summary>
    /// The scope or focus of the command.
    /// </summary>
    [Serializable]
    public class CommandScopeModel : CoreModel {
        /// <summary>
        /// The limiting connection guid
        /// </summary>
        public Guid ConnectionGuid { get; set; }

        /// <summary>
        /// The limiting plugin guid.
        /// </summary>
        public Guid PluginGuid { get; set; }

        /// <summary>
        /// Initializes the model with the default values.
        /// </summary>
        public CommandScopeModel() {
            this.ConnectionGuid = Guid.Empty;
            this.PluginGuid = Guid.Empty;
        }
    }
}
