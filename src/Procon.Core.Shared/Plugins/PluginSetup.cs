using System;

namespace Procon.Core.Shared.Plugins {
    /// <summary>
    /// Handles passing simple variables from Procon AppDomain to the plugin AppDomain
    /// </summary>
    [Serializable]
    public class PluginSetup : IPluginSetup {
        public String ConnectionGuid { get; set; }
        public String ConfigDirectoryPath { get; set; }
        public String LogDirectoryPath { get; set; }
    }
}
