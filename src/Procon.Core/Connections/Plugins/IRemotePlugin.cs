using System;
using System.IO;

namespace Procon.Core.Connections.Plugins {
    using Procon.Core.Events;
    using Procon.Net;

    public interface IRemotePlugin : IExecutableBase, IDisposable {
        /// <summary>
        /// The Guid of the executing assembly. Used to uniquely identify this plugin. 
        /// </summary>
        Guid PluginGuid { get; }

        /// <summary>
        /// The connection that owns this plugin instance.
        /// </summary>
        Guid ConnectionGuid { get; set; }

        /// <summary>
        /// Path to the default config-file of the plugin
        /// </summary>
        DirectoryInfo ConfigDirectoryInfo { get; set; }

        /// <summary>
        /// Path to the log-file directory of the plugin
        /// </summary>
        DirectoryInfo LogDirectoryInfo { get; set; }

        /// <summary>
        /// The interface to callback from the plugin side to Procon.
        /// </summary>
        IPluginCallback PluginCallback { set; }

        void GameEvent(GameEventArgs e);

        void ClientEvent(ClientEventArgs e);

        void GenericEvent(GenericEventArgs e);
    }
}
