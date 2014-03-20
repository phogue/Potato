using System;
using System.Collections.Generic;

namespace Procon.Core.Shared.Plugins {
    /// <summary>
    /// Handles passing simple variables from the plugin AppDomain back to Procon AppDomain
    /// </summary>
    [Serializable]
    public class PluginSetupResult : IPluginSetupResult {
        public List<String> Commands { get; set; } 
    }
}
