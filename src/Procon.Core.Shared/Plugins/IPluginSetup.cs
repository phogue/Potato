using System;

namespace Procon.Core.Shared.Plugins {
    /// <summary>
    /// Handles passing simple variables from Procon AppDomain to the plugin AppDomain
    /// </summary>
    public interface IPluginSetup {
        /// <summary>
        /// The connection that owns this plugin instance.
        /// </summary>
        String ConnectionGuid { get; }

        /// <summary>
        /// Path to the default config-file of the plugin
        /// </summary>
        String ConfigDirectoryPath { get; }

        /// <summary>
        /// Path to the log-file directory of the plugin
        /// </summary>
        String LogDirectoryPath { get; }
    }
}
