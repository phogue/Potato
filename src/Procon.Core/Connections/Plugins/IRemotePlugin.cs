using System;
using System.Collections.Generic;
using System.IO;
using Procon.Core.Events;
using Procon.Net;

namespace Procon.Core.Connections.Plugins {
    /// <summary>
    /// Interface for procon to communicate across the AppDomain to the plugin
    /// </summary>
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
        IList<IExecutableBase> PluginCallback { set; }

        /// <summary>
        /// Fired whenever an event is passed from the client, to the game layer
        /// then processed with as an event from the game server. (OnChat, OnKill etc.)
        /// </summary>
        /// <param name="e">Description of the game event</param>
        void GameEvent(GameEventArgs e);

        /// <summary>
        /// Fired whenever an event is fired from the networking layer (packet sent/recv,
        /// connection state change or any socket errors)
        /// </summary>
        /// <param name="e">Description of the client event</param>
        void ClientEvent(ClientEventArgs e);

        /// <summary>
        /// Fired whenever an event occurs from Procon, but isolated to this particular plugin.
        /// </summary>
        /// <param name="e">Description of the generic event</param>
        void GenericEvent(GenericEventArgs e);
    }
}
