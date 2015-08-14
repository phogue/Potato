#region Copyright
// Copyright 2015 Geoff Green.
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
using System.Collections.Generic;
using Potato.Net.Shared;

namespace Potato.Core.Shared.Plugins {
    /// <summary>
    /// Remoting interface for Potato.Core to communicate with remote Plugin.
    /// </summary>
    public interface ISandboxPluginController : ICoreController {
        /// <summary>
        /// Creates an instance of a type in an assembly.
        /// </summary>
        /// <param name="assemblyFile"></param>
        /// <param name="typeName"></param>
        /// <returns></returns>
        IPluginController Create(string assemblyFile, string typeName);

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
        /// Remote proxy to propogate the protocol event across all enabled plugins and avoid multiple remoting calls.
        /// </summary>
        void ProtocolEvent(List<IProtocolEventArgs> items);

        /// <summary>
        /// Remote proxy to propogate the client event across all enabled plugins and avoid multiple remoting calls.
        /// </summary>
        void ClientEvent(List<IClientEventArgs> items);

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
