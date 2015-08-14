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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Lifetime;
using System.Security;
using System.Security.Permissions;
using System.Text.RegularExpressions;
using System.Threading;
using Potato.Core.Shared;
using Potato.Core.Shared.Events;
using Potato.Core.Shared.Models;
using Potato.Core.Shared.Plugins;
using Potato.Net.Shared;
using Potato.Service.Shared;

namespace Potato.Core.Connections.Plugins {
    /// <summary>
    /// Manages loading and propogating plugin events, as well as callbacks from
    /// a plugin back to Potato.
    /// </summary>
    public class CorePluginController : AsynchronousCoreController, ISharedReferenceAccess, ICorePluginController {
        public List<PluginModel> LoadedPlugins { get; set; }

        /// <summary>
        /// The appdomain all of the plugins are loaded into.
        /// </summary>
        public AppDomain AppDomainSandbox { get; protected set; }

        /// <summary>
        /// Simple plugin factory reference to load classes into the app domain.
        /// </summary>
        public ISandboxPluginController PluginFactory { get; protected set; }

        /// <summary>
        /// The connection which owns this plugin app domain and the connection which the plugins control.
        /// </summary>
        public IConnectionController Connection { get; set; }

        /// <summary>
        /// Works between PluginController and RemotePluginController as a known type by a plugin
        /// assembly and Core, just bubbling (not tunneling) commands.
        /// </summary>
        public CorePluginControllerCallbackProxy CorePluginControllerCallbackProxy { get; set; }

        /// <summary>
        /// Throttles the stream of client events crossing the appdomain, grouping them into a 
        /// list every second and passing them through to the plugins.
        /// </summary>
        public IThrottledStream<IClientEventArgs> ClientEventStream { get; set; }
        
        /// <summary>
        /// Throttles the stream of protocol events crossing the appdomain, grouping them into a 
        /// list every second and passing them through to the plugins.
        /// </summary>
        public IThrottledStream<IProtocolEventArgs> ProtocolEventStream { get; set; }

        public SharedReferences Shared { get; private set; }
        
        /// <summary>
        /// Default Initialization
        /// </summary>
        public CorePluginController() : base() {
            Shared = new SharedReferences();
            LoadedPlugins = new List<PluginModel>();

            CorePluginControllerCallbackProxy = new CorePluginControllerCallbackProxy() {
                BubbleObjects = {
                    this
                }
            };

            CommandDispatchers.AddRange(new List<ICommandDispatch>() {
                new CommandDispatch() {
                    CommandType = CommandType.PluginsEnable,
                    Handler = EnablePlugin
                },
                new CommandDispatch() {
                    CommandType = CommandType.PluginsDisable,
                    Handler = DisablePlugin
                }
            });
        }

        /// <summary>
        /// Attempts to enable a plugin, returning false if the plugin does not exist or is already enabled.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public ICommandResult EnablePlugin(ICommand command, Dictionary<string, ICommandParameter> parameters) {
            ICommandResult result = null;

            if (Shared.Security.DispatchPermissionsCheck(command, command.Name).Success == true) {
                var pluginGuid = command.Scope.PluginGuid;

                if (LoadedPlugins.Count(plugin => plugin.PluginGuid == pluginGuid) > 0) {
                    if (PluginFactory.TryEnablePlugin(pluginGuid) == true) {
                        var plugin = LoadedPlugins.First(p => p.PluginGuid == pluginGuid);

                        plugin.IsEnabled = true;

                        result = new CommandResult() {
                            CommandResultType = CommandResultType.Success,
                            Success = true,
                            Message = string.Format("Plugin {0} has been enabled", pluginGuid),
                            Scope = {
                                Connections = new List<ConnectionModel>() {
                                    Connection != null ? Connection.ConnectionModel : null
                                },
                                Plugins = new List<PluginModel>() {
                                    plugin
                                }
                            }
                        };

                        if (Shared.Events != null) {
                            Shared.Events.Log(GenericEvent.ConvertToGenericEvent(result, GenericEventType.PluginsEnabled));
                        }
                    }
                    else {
                        result = new CommandResult() {
                            CommandResultType = CommandResultType.Failed,
                            Success = false
                        };
                    }
                }
                else {
                    result = new CommandResult() {
                        CommandResultType = CommandResultType.DoesNotExists,
                        Success = false
                    };
                }
            }
            else {
                result = CommandResult.InsufficientPermissions;
            }

            return result;
        }

        /// <summary>
        /// Attempts to disable a plugin, returning false if the plugin does not exist or is already disabled.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public ICommandResult DisablePlugin(ICommand command, Dictionary<string, ICommandParameter> parameters) {
            ICommandResult result = null;

            if (Shared.Security.DispatchPermissionsCheck(command, command.Name).Success == true) {
                var pluginGuid = command.Scope.PluginGuid;

                if (LoadedPlugins.Count(plugin => plugin.PluginGuid == pluginGuid) > 0) {
                    if (PluginFactory.TryDisablePlugin(pluginGuid) == true) {
                        var plugin = LoadedPlugins.First(p => p.PluginGuid == pluginGuid);

                        plugin.IsEnabled = false;

                        result = new CommandResult() {
                            CommandResultType = CommandResultType.Success,
                            Success = true,
                            Message = string.Format("Plugin {0} has been disabled", pluginGuid),
                            Scope = {
                                Connections = new List<ConnectionModel>() {
                                    Connection != null ? Connection.ConnectionModel : null
                                },
                                Plugins = new List<PluginModel>() {
                                    plugin
                                }
                            }
                        };

                        if (Shared.Events != null) {
                            Shared.Events.Log(GenericEvent.ConvertToGenericEvent(result, GenericEventType.PluginsDisabled));
                        }
                    }
                    else {
                        result = new CommandResult() {
                            CommandResultType = CommandResultType.Failed,
                            Success = false
                        };
                    }
                }
                else {
                    result = new CommandResult() {
                        CommandResultType = CommandResultType.DoesNotExists,
                        Success = false
                    };
                }
            }
            else {
                result = CommandResult.InsufficientPermissions;
            }

            return result;
        }

        /// <summary>
        /// Create the app domain setup options required to create the app domain.
        /// </summary>
        /// <returns></returns>
        protected AppDomainSetup CreateAppDomainSetup() {
            var setup = new AppDomainSetup {
                LoaderOptimization = LoaderOptimization.MultiDomain,
                ApplicationBase = AppDomain.CurrentDomain.BaseDirectory,
                PrivateBinPath = string.Join(";", new[] {
                    AppDomain.CurrentDomain.BaseDirectory,
                    Defines.PackageMyrconPotatoCoreLibNet40.FullName,
                    Defines.PackageMyrconPotatoSharedLibNet40.FullName
                })
            };

            // todo provide access to all non-core lib package paths.

            // [XpKiller] - Mono workaround.
            if (Type.GetType("Mono.Runtime") != null) {
                setup.PrivateBinPath = AppDomain.CurrentDomain.BaseDirectory;
            }

            return setup;
        }

        /// <summary>
        /// Creates the permissions set to apply to the app domain.
        /// </summary>
        /// <returns></returns>
        protected PermissionSet CreatePermissionSet() {
            var permissions = new PermissionSet(PermissionState.None);
            
            permissions.AddPermission(new SecurityPermission(SecurityPermissionFlag.Execution));

            permissions.AddPermission(new FileIOPermission(FileIOPermissionAccess.PathDiscovery, AppDomain.CurrentDomain.BaseDirectory));
            permissions.AddPermission(new FileIOPermission(FileIOPermissionAccess.AllAccess, Defines.LogsDirectory.FullName));
            permissions.AddPermission(new FileIOPermission(FileIOPermissionAccess.AllAccess, Path.Combine(Defines.ConfigsDirectory.FullName, Connection != null ? Connection.ConnectionModel.ConnectionGuid.ToString() : Guid.Empty.ToString())));
            permissions.AddPermission(new FileIOPermission(FileIOPermissionAccess.Read | FileIOPermissionAccess.PathDiscovery, Defines.ConfigsDirectory.FullName));

            foreach (var file in GetPluginAssemblies()) {
                var directory = Defines.PackageContainingPath(file.Directory != null ? file.Directory.FullName : "");

                // If we didn't just go up to the root directory.
                if (directory != null) {
                    permissions.AddPermission(new FileIOPermission(FileIOPermissionAccess.AllAccess, directory.FullName));
                }
            }

            return permissions;
        }

        /// <summary>
        /// Creates all of the directories required by plugins
        /// </summary>
        protected void CreateDirectories() {
            Directory.CreateDirectory(Path.Combine(Defines.ConfigsDirectory.FullName, Connection != null ? Connection.ConnectionModel.ConnectionGuid.ToString() : Guid.Empty.ToString()));
        }

        public override void WriteConfig(IConfig config, string password = null) {
            foreach (var plugin in LoadedPlugins.Where(plugin => plugin.IsEnabled == true)) {
                config.Append(new Command() {
                    CommandType = CommandType.PluginsEnable,
                    Scope = {
                        ConnectionGuid = Connection.ConnectionModel.ConnectionGuid,
                        PluginGuid = plugin.PluginGuid
                    }
                }.ToConfigCommand());
            }
        }

        /// <summary>
        /// Executes the commands specified in the config file and returns a reference itself.
        /// </summary>
        public override ICoreController Execute() {
            SetupPluginFactory();

            TunnelObjects.Add(PluginFactory);
            BubbleObjects.Add(Connection);

            // Load all the plugins.
            LoadPlugins();

            if (Connection != null) {
                Connection.ClientEvent += Connection_ClientEvent;
                Connection.ProtocolEvent += Connection_GameEvent;
            }

            // Return the base execution.
            return base.Execute();
        }

        private void Connection_ClientEvent(IClientEventArgs e) {
            if (ClientEventStream != null) {
                ClientEventStream.Call(e);
            }
        }

        private void Connection_GameEvent(IProtocolEventArgs e) {
            if (ProtocolEventStream != null) {
                ProtocolEventStream.Call(e);
            }
        }

        /// <summary>
        /// Sets everything up to load the plugins, creating the seperate appdomin and permission requirements
        /// </summary>
        protected void SetupPluginFactory() {
            var setup = CreateAppDomainSetup();

            var permissions = CreatePermissionSet();

            CreateDirectories();

            // Create the app domain and the plugin factory in the new domain.
            AppDomainSandbox = AppDomain.CreateDomain(string.Format("Potato.Plugins.{0}", Connection != null ? Connection.ConnectionModel.ConnectionGuid.ToString() : string.Empty), null, setup, permissions);

            PluginFactory = (ISandboxPluginController)AppDomainSandbox.CreateInstanceAndUnwrap(typeof(SandboxPluginController).Assembly.FullName, typeof(SandboxPluginController).FullName);

            PluginFactory.BubbleObjects = new List<ICoreController>() {
                CorePluginControllerCallbackProxy
            };

            ClientEventStream = new ThrottledStream<IClientEventArgs>() {
                FlushTo = PluginFactory.ClientEvent
            }.Start();
            
            ProtocolEventStream = new ThrottledStream<IProtocolEventArgs>() {
                FlushTo = PluginFactory.ProtocolEvent
            }.Start();
        }

        /// <summary>
        /// Fetches a list of assembly files to load.
        /// </summary>
        /// <returns></returns>
        protected List<FileInfo> GetPluginAssemblies() {
            return Directory.GetFiles(Defines.PackagesDirectory.FullName, @"*.Plugins.*.dll", SearchOption.AllDirectories)
                .Select(path => new FileInfo(path))
                .Where(file =>
                    file.Name != Defines.PotatoCoreDll &&
                    file.Name != Defines.PotatoNetDll &&
                    file.Name != Defines.PotatoFuzzyDll &&
                    file.Name != Defines.NewtonsoftJsonDll &&
                    file.Name != Defines.PotatoCoreSharedDll &&
                    file.Name != Defines.PotatoDatabaseSharedDll &&
                    file.Name != Defines.PotatoNetSharedDll)
                .Where(file => Regex.Matches(file.FullName, file.Name.Replace(file.Extension, string.Empty)).Cast<Match>().Count() >= 2)
                .ToList();
        } 

        /// <summary>
        /// Setup the plugins located in or in sub-folders of this directory.
        /// </summary>
        protected void LoadPlugins() {
            // If there are dll files in this directory, setup the plugins.
            foreach (var path in GetPluginAssemblies().Select(file => file.FullName)) {
                var plugin = new PluginModel() {
                    Name = new FileInfo(path).Name.Replace(".dll", "")
                };

                var proxy = PluginFactory.Create(path, plugin.Name + ".Program");

                if (proxy != null) {
                    plugin.PluginGuid = proxy.PluginGuid;

                    var connectionGuid = Connection != null ? Connection.ConnectionModel.ConnectionGuid : Guid.Empty;

                    var result = proxy.Setup(new PluginSetup() {
                        ConnectionGuid = connectionGuid.ToString(),
                        ConfigDirectoryPath = Path.Combine(Defines.ConfigsDirectory.FullName, connectionGuid.ToString(), plugin.PluginGuid.ToString()),
                        LogDirectoryPath = Path.Combine(Defines.LogsDirectory.FullName, connectionGuid.ToString(), plugin.PluginGuid.ToString())
                    });

                    plugin.Commands = result.Commands;
                    plugin.Title = result.Title;

                    // Tell the plugin it's ready to begin, everything is setup and ready 
                    // for it to start loading its config.
                    proxy.GenericEvent(new GenericEvent() {
                        GenericEventType = GenericEventType.PluginsLoaded
                    });
                }

                LoadedPlugins.Add(plugin);
            }
        }

        /// <summary>
        /// Renews the lease on the plugin factory as well as each loaded plugin proxy
        /// </summary>
        public override void Poke() {
            var lease = ((MarshalByRefObject)PluginFactory).GetLifetimeService() as ILease;

            if (lease != null) {
                lease.Renew(lease.InitialLeaseTime);
            }
        }

        public override ICommandResult PropogatePreview(ICommand command, CommandDirection direction) {
            ICommandResult synchronousResult = null;

            // If we're bubbling and we have not seen this command yet
            if (direction == CommandDirection.Bubble && AsyncStateModel.IsKnown(command.CommandGuid) == false) {
                var resultWait = new AutoResetEvent(false);

                BeginBubble(command, result => {
                    synchronousResult = result;

                    resultWait.Set();
                });

                // Wait here until we have a result to return to the plugin AppDomain.
                resultWait.WaitOne();
            }
            else {
                synchronousResult = base.PropogatePreview(command, direction);
            }

            return synchronousResult;
        }

        /// <summary>
        /// Disposes of all the plugins before calling the base dispose.
        /// </summary>
        public override void Dispose() {
            ClientEventStream.Stop();
            ProtocolEventStream.Stop();
            
            PluginFactory.Shutdown();

            LoadedPlugins.Clear();
            LoadedPlugins = null;

            AppDomain.Unload(AppDomainSandbox);
            AppDomainSandbox = null;
            PluginFactory = null;

            base.Dispose();
        }
    }
}
