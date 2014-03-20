using System;
using System.Collections.Generic;

namespace Procon.Core.Shared.Plugins {
    /// <summary>
    /// Handles passing simple variables from the plugin AppDomain back to Procon AppDomain
    /// </summary>
    public interface IPluginSetupResult {
        /// <summary>
        /// List of commands this plugin can handle
        /// </summary>
        List<String> Commands { get; set; } 
    }
}
