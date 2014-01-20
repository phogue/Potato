using System.Collections.Generic;
using Procon.Core.Shared;
using Procon.Core.Shared.Models;
using Procon.Core.Shared.Plugins;

namespace Procon.Core.Connections.Plugins {
    /// <summary>
    /// Manages loading and propogating plugin events, as well as callbacks from
    /// a plugin back to Procon.
    /// </summary>
    public interface ICorePluginController : ICoreController {
        /// <summary>
        /// List of plugins loaded in the app domain.
        /// </summary>
        List<PluginModel> LoadedPlugins { get; }
    }
}
