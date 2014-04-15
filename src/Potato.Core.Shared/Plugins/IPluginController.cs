#region Copyright
// Copyright 2014 Myrcon Pty. Ltd.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
using System;
using Potato.Core.Shared.Events;
using Potato.Net.Shared;

namespace Potato.Core.Shared.Plugins {
    /// <summary>
    /// Interface for Potato to communicate across the AppDomain to the plugin
    /// </summary>
    public interface IPluginController : ICoreController {
        /// <summary>
        /// The Guid of the executing assembly. Used to uniquely identify this plugin. 
        /// </summary>
        Guid PluginGuid { get; }

        /// <summary>
        /// Creates and sets the more complex properties of this plugin.
        /// </summary>
        /// <param name="setup">The parameters to copy to the plugin app domain.</param>
        IPluginSetupResult Setup(IPluginSetup setup);

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
        /// Fired whenever an event occurs from Potato, but isolated to this particular plugin.
        /// </summary>
        /// <param name="e">Description of the generic event</param>
        void GenericEvent(GenericEvent e);
    }
}
