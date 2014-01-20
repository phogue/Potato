using System;
using Procon.Core.Shared.Events;
using Procon.Net.Shared;

namespace Procon.Core.Shared.Plugins {
    /// <summary>
    /// Interface for procon to communicate across the AppDomain to the plugin
    /// </summary>
    public interface IPluginController : ICoreController, IDisposable {
        /// <summary>
        /// The Guid of the executing assembly. Used to uniquely identify this plugin. 
        /// </summary>
        Guid PluginGuid { get; }

        /// <summary>
        /// Execute the controller
        /// </summary>
        ICoreController Execute();

        /// <summary>
        /// Creates and sets the more complex properties of this plugin.
        /// </summary>
        /// <param name="setup">The parameters to copy to the plugin app domain.</param>
        void Setup(IPluginSetup setup);

        /// <summary>
        /// Fired whenever an event is passed from the client, to the game layer
        /// then processed with as an event from the game server. (OnChat, OnKill etc.)
        /// </summary>
        /// <param name="e">Description of the game event</param>
        void GameEvent(IProtocolEventArgs e);

        /// <summary>
        /// Fired whenever an event is fired from the networking layer (packet sent/recv,
        /// connection state change or any socket errors)
        /// </summary>
        /// <param name="e">Description of the client event</param>
        void ClientEvent(IClientEventArgs e);

        /// <summary>
        /// Fired whenever an event occurs from Procon, but isolated to this particular plugin.
        /// </summary>
        /// <param name="e">Description of the generic event</param>
        void GenericEvent(GenericEvent e);
    }
}
