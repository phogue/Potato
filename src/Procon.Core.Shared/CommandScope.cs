using System;

namespace Procon.Core.Shared {

    [Serializable]
    public class CommandScope {

        /// <summary>
        /// The limiting connection guid
        /// </summary>
        public Guid ConnectionGuid { get; set; }

        /// <summary>
        /// The limiting plugin guid.
        /// </summary>
        public Guid PluginGuid { get; set; }

        public CommandScope() {
            this.ConnectionGuid = Guid.Empty;
            this.PluginGuid = Guid.Empty;
        }
    }
}
