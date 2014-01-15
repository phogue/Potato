using System;
using Procon.Net.Shared;

namespace Procon.Core.Shared.Plugins {
    /// <summary>
    /// Remoting interface for Procon.Core to communicate with remote Plugin.
    /// </summary>
    public interface IRemotePluginController : ICoreController {
        /// <summary>
        /// Creates an instance of a type in an assembly.
        /// </summary>
        /// <param name="assemblyFile"></param>
        /// <param name="typeName"></param>
        /// <returns></returns>
        IPluginController Create(String assemblyFile, String typeName);

        /// <summary>
        /// Enables a plugin by it's guid, allowing it to accept events and commands.
        /// </summary>
        /// <param name="pluginGuid"></param>
        /// <returns>Returns true if the plugin was disabled and is now enabled. False will be returned if the plugin does not exist or was already enabled to begin with.</returns>
        bool TryEnablePlugin(Guid pluginGuid);

        /// <summary>
        /// Disables a plugin, denying it events and commands.
        /// </summary>
        /// <param name="pluginGuid"></param>
        /// <returns>Returns true if the plugin was enabled and is now disabled. False will be returned if the plugin does not exist or wasn't enabled to begin with.</returns>
        bool TryDisablePlugin(Guid pluginGuid);

        /// <summary>
        /// Remote proxy to propogate the game event across all enabled plugins and avoid multiple remoting calls.
        /// </summary>
        /// <param name="e"></param>
        void GameEvent(ProtocolEventArgs e);

        /// <summary>
        /// Remote proxy to propogate the client event across all enabled plugins and avoid multiple remoting calls.
        /// </summary>
        /// <param name="e"></param>
        void ClientEvent(ClientEventArgs e);

        /// <summary>
        /// Check if a plugin is marked as enabled or not
        /// </summary>
        /// <param name="pluginGuid"></param>
        /// <returns>True if a plugin is enabled, false otherwise.</returns>
        bool IsPluginEnabled(Guid pluginGuid);

        /// <summary>
        /// Shutdown all of the plugins, firing a 
        /// </summary>
        void Shutdown();
    }
}
