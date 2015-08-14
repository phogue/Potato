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
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Reflection;
using Potato.Core.Shared.Events;
using Potato.Net.Shared;

namespace Potato.Core.Shared.Plugins {
    /// <summary>
    /// Controller to run on the plugin side of the AppDomain
    /// </summary>
    public class SandboxPluginController : CoreController, ISandboxPluginController {
        /// <summary>
        /// A list of loaded plugins. This can be considered a pool of plugins that
        /// exist, but not necessarily plugins that should recieve events and commands.
        /// </summary>
        protected ConcurrentDictionary<Guid, IPluginController> LoadedPlugins = new ConcurrentDictionary<Guid, IPluginController>();

        /// <summary>
        /// A list of plugins that are enabled which should recieve events and commands.
        /// </summary>
        protected ConcurrentDictionary<Guid, IPluginController> EnabledPlugins = new ConcurrentDictionary<Guid, IPluginController>();

        /// <summary>
        /// Creates an instance of a type in an assembly.
        /// </summary>
        /// <param name="assemblyFile"></param>
        /// <param name="typeName"></param>
        /// <returns></returns>
        public IPluginController Create(string assemblyFile, string typeName) {
            IPluginController loadedPlugin = null;

            try {
                loadedPlugin = (IPluginController)Activator.CreateInstanceFrom(
                    assemblyFile,
                    typeName,
                    false,
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.CreateInstance,
                    null,
                    null,
                    null,
                    null
                ).Unwrap();

                LoadedPlugins.TryAdd(loadedPlugin.PluginGuid, loadedPlugin);

                loadedPlugin.Execute();
            }
            // We don't do any exception logging here, as simply updating Potato may log a bunch of exceptions
            // for plugins that are deprecated or simply forgotten about by the user.
            // The exceptions wouldn't be terribly detailed anyway, it would just specify that a fault occured
            // while loading the assembly/type and ultimately the original developer needs to fix something.
            // I would also hope that beyond Beta we will not make breaking changes to the plugin interface,
            // differing from Potato 1 in generic behaviour for IPluginController/ICoreController
            catch {
                loadedPlugin = null;
            }

            return loadedPlugin;
        }

        /// <summary>
        /// Enables a plugin by it's guid, allowing it to accept events and commands.
        /// </summary>
        /// <param name="pluginGuid"></param>
        /// <returns>Returns true if the plugin was disabled and is now enabled. False will be returned if the plugin does not exist or was already enabled to begin with.</returns>
        public bool TryEnablePlugin(Guid pluginGuid) {
            var wasEnabled = false;

            // If we have the plugin and the plugin isn't already in the dictionary.
            if (LoadedPlugins.ContainsKey(pluginGuid) == true && EnabledPlugins.ContainsKey(pluginGuid) == false) {
                var plugin = LoadedPlugins[pluginGuid];

                EnabledPlugins.TryAdd(plugin.PluginGuid, plugin);

                plugin.BubbleObjects.Add(this);

                //plugin.PluginCallback = new List<IExecutableBase>() {
                //    this
                //};

                plugin.GenericEvent(new GenericEvent() {
                    GenericEventType = GenericEventType.PluginsEnabled
                });

                wasEnabled = true;
            }

            return wasEnabled;
        }

        /// <summary>
        /// Disables a plugin, denying it events and commands.
        /// </summary>
        /// <param name="pluginGuid"></param>
        /// <returns>Returns true if the plugin was enabled and is now disabled. False will be returned if the plugin does not exist or wasn't enabled to begin with.</returns>
        public bool TryDisablePlugin(Guid pluginGuid) {
            var wasDisabled = false;

            IPluginController plugin;

            if (EnabledPlugins.TryRemove(pluginGuid, out plugin) == true) {
                plugin.BubbleObjects.Clear();

                plugin.GenericEvent(new GenericEvent() {
                    GenericEventType = GenericEventType.PluginsDisabled
                });

                wasDisabled = true;
            }

            return wasDisabled;
        }

        public void ProtocolEvent(List<IProtocolEventArgs> items) {
            foreach (var item in items) {
                foreach (var enabledPlugin in EnabledPlugins) {
                    enabledPlugin.Value.GameEvent(item);
                }
            }
        }

        public void ClientEvent(List<IClientEventArgs> items) {
            foreach (var item in items) {
                foreach (var enabledPlugin in EnabledPlugins) {
                    enabledPlugin.Value.ClientEvent(item);
                }
            }
        }

        /// <summary>
        /// Check if a plugin is marked as enabled or not
        /// </summary>
        /// <param name="pluginGuid"></param>
        /// <returns>True if a plugin is enabled, false otherwise.</returns>
        public bool IsPluginEnabled(Guid pluginGuid) {
            return EnabledPlugins.ContainsKey(pluginGuid);
        }

        /// <summary>
        /// Shutdown all of the plugins, firing a 
        /// </summary>
        public void Shutdown() {
            foreach (var plugin in LoadedPlugins) {
                plugin.Value.GenericEvent(new GenericEvent() {
                    GenericEventType = GenericEventType.PluginsUnloading
                });

                plugin.Value.Dispose();
            }
        }

        /// <summary>
        /// Enforce an origin of plugin before we bubble any commands.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        protected override IList<ICoreController> BubbleExecutableObjects(ICommand command) {
            command.Origin = CommandOrigin.Plugin;

            return BubbleObjects ?? new List<ICoreController>();
        }

        protected override IList<ICoreController> TunnelExecutableObjects(ICommand command) {
            var list = new List<ICoreController>();

            if (command.Scope != null && command.Scope.PluginGuid != Guid.Empty) {
                IPluginController enabledPlugin;

                // Get the enabled plugin if it exists.
                if (EnabledPlugins.TryGetValue(command.Scope.PluginGuid, out enabledPlugin) == true) {
                    list.Add(enabledPlugin);
                }
                // else the plugin isn't enabled
                // todo potentially alter command result to say the plugin isn't enabled.
            }
            else {
                // Add all of the enabled plugins.
                list.AddRange(EnabledPlugins.Values);
            }

            return list;
        }
    }
}