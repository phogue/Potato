using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Procon.Core.Shared.Models {
    [Serializable]
    public class PluginModel : CoreModel {
        /// <summary>
        /// The name of the plugin, also used as it's namespace
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// The loaded plugin GUID
        /// todo This remotes once, which isn't very expensive if used in moderation, but perhaps we should cache this result if this ever becomes a hot spot?
        /// </summary>
        public Guid PluginGuid { get; set; }

        /// <summary>
        /// If this plugin is enabled or not.
        /// todo This remotes twice, which isn't very expensive if used in moderation, but perhaps we should cache this result if this ever becomes a hot spot?
        /// </summary>
        public bool IsEnabled { get; set; }
    }
}
