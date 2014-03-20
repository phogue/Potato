using System;
using System.Collections.Generic;

namespace Procon.Core.Shared.Models {
    /// <summary>
    /// Describes information about the plugin locally, so we don't need to remote for basic information.
    /// </summary>
    [Serializable]
    public class PluginModel : CoreModel {
        /// <summary>
        /// The name of the plugin, also used as it's namespace
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// The loaded plugin GUID
        /// </summary>
        public Guid PluginGuid { get; set; }

        /// <summary>
        /// If this plugin is enabled or not.
        /// </summary>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// A simple list of commands this plugin has registered handlers.
        /// </summary>
        public List<String> Commands { get; set; }
    }
}
