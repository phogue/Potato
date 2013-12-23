﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Lifetime;
using System.Security;
using System.Security.Permissions;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Procon.Core.Shared;
using Procon.Core.Shared.Events;
using Procon.Core.Shared.Models;
using Procon.Core.Shared.Plugins;
using Procon.Net.Shared;
using Procon.Service.Shared;

namespace Procon.Core.Connections.Plugins {
    /// <summary>
    /// Manages loading and propogating plugin events, as well as callbacks from
    /// a plugin back to Procon.
    /// </summary>
    public class CorePluginController : SharedController, IRenewableLease {

        /// <summary>
        /// The appdomain all of the plugins are loaded into.
        /// </summary>
        [XmlIgnore, JsonIgnore]
        public AppDomain AppDomainSandbox { get; protected set; }

        /// <summary>
        /// Simple plugin factory reference to load classes into the app domain.
        /// </summary>
        [XmlIgnore, JsonIgnore]
        public IRemotePluginController PluginFactory { get; protected set; }

        /// <summary>
        /// List of plugins loaded in the app domain.
        /// </summary>
        [XmlIgnore, JsonIgnore]
        public List<PluginModel> LoadedPlugins { get; protected set; }

        /// <summary>
        /// The connection which owns this plugin app domain and the connection which the plugins control.
        /// </summary>
        [XmlIgnore, JsonIgnore]
        public ConnectionController Connection { get; set; }


        /// <summary>
        /// Works between PluginController and RemotePluginController as a known type by a plugin
        /// assembly and Core, just bubbling (not tunneling) commands.
        /// </summary>
        [XmlIgnore, JsonIgnore]
        public CorePluginControllerCallbackProxy CorePluginControllerCallbackProxy { get; set; }

        /// <summary>
        /// Default Initialization
        /// </summary>
        public CorePluginController() : base() {
            this.LoadedPlugins = new List<PluginModel>();

            this.CorePluginControllerCallbackProxy = new CorePluginControllerCallbackProxy() {
                BubbleObjects = {
                    this
                }
            };

            this.AppendDispatchHandlers(new Dictionary<CommandAttribute, CommandDispatchHandler>() {
                {
                    new CommandAttribute() {
                        CommandType = CommandType.PluginsEnable
                    },
                    new CommandDispatchHandler(this.EnablePlugin)
                }, {
                    new CommandAttribute() {
                        CommandType = CommandType.PluginsDisable
                    },
                    new CommandDispatchHandler(this.DisablePlugin)
                }
            });
        }

        /// <summary>
        /// Attempts to enable a plugin, returning false if the plugin does not exist or is already enabled.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public CommandResultArgs EnablePlugin(Command command, Dictionary<String, CommandParameter> parameters) {
            CommandResultArgs result = null;

            if (this.Security.DispatchPermissionsCheck(command, command.Name).Success == true) {
                Guid pluginGuid = command.Scope.PluginGuid;

                if (this.LoadedPlugins.Count(plugin => plugin.PluginGuid == pluginGuid) > 0) {
                    if (this.PluginFactory.TryEnablePlugin(pluginGuid) == true) {
                        PluginModel plugin = this.LoadedPlugins.First(p => p.PluginGuid == pluginGuid);

                        plugin.IsEnabled = true;

                        result = new CommandResultArgs() {
                            Status = CommandResultType.Success,
                            Success = true,
                            Message = String.Format("Plugin {0} has been enabled", pluginGuid),
                            Scope = {
                                Connections = new List<ConnectionModel>() {
                                    this.Connection != null ? this.Connection.ConnectionModel : null
                                },
                                Plugins = new List<PluginModel>() {
                                    plugin
                                }
                            }
                        };

                        if (this.Events != null) {
                            this.Events.Log(GenericEventArgs.ConvertToGenericEvent(result, GenericEventType.PluginsPluginEnabled));
                        }
                    }
                    else {
                        result = new CommandResultArgs() {
                            Status = CommandResultType.Failed,
                            Success = false
                        };
                    }
                }
                else {
                    result = new CommandResultArgs() {
                        Status = CommandResultType.DoesNotExists,
                        Success = false
                    };
                }
            }
            else {
                result = CommandResultArgs.InsufficientPermissions;
            }

            return result;
        }

        /// <summary>
        /// Attempts to disable a plugin, returning false if the plugin does not exist or is already disabled.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public CommandResultArgs DisablePlugin(Command command, Dictionary<String, CommandParameter> parameters) {
            CommandResultArgs result = null;

            if (this.Security.DispatchPermissionsCheck(command, command.Name).Success == true) {
                Guid pluginGuid = command.Scope.PluginGuid;

                if (this.LoadedPlugins.Count(plugin => plugin.PluginGuid == pluginGuid) > 0) {
                    if (this.PluginFactory.TryDisablePlugin(pluginGuid) == true) {
                        PluginModel plugin = this.LoadedPlugins.First(p => p.PluginGuid == pluginGuid);

                        plugin.IsEnabled = false;

                        result = new CommandResultArgs() {
                            Status = CommandResultType.Success,
                            Success = true,
                            Message = String.Format("Plugin {0} has been disabled", pluginGuid),
                            Scope = {
                                Connections = new List<ConnectionModel>() {
                                    this.Connection != null ? this.Connection.ConnectionModel : null
                                },
                                Plugins = new List<PluginModel>() {
                                    plugin
                                }
                            }
                        };

                        if (this.Events != null) {
                            this.Events.Log(GenericEventArgs.ConvertToGenericEvent(result, GenericEventType.PluginsPluginDisabled));
                        }
                    }
                    else {
                        result = new CommandResultArgs() {
                            Status = CommandResultType.Failed,
                            Success = false
                        };
                    }
                }
                else {
                    result = new CommandResultArgs() {
                        Status = CommandResultType.DoesNotExists,
                        Success = false
                    };
                }
            }
            else {
                result = CommandResultArgs.InsufficientPermissions;
            }

            return result;
        }

        /// <summary>
        /// Copies the necessary files to execute a plugin to the specified directory.
        /// </summary>
        protected void CreatePluginDirectory(FileSystemInfo pluginDirectory) {
            List<String> files = new List<String>() {
                Defines.ProconCoreSharedDll,
                Defines.ProconNetSharedDll,
                Defines.ProconDatabaseSharedDll,
                Defines.NewtonsoftJsonDll
            };

            try {
                files.ForEach(file => File.Copy(Path.Combine(Defines.BaseDirectory, file), Path.Combine(pluginDirectory.FullName, file), true));
            }
            catch (Exception) { }
        }

        /// <summary>
        /// Create the app domain setup options required to create the app domain.
        /// </summary>
        /// <returns></returns>
        protected AppDomainSetup CreateAppDomainSetup() {
            AppDomainSetup setup = new AppDomainSetup {
                LoaderOptimization = LoaderOptimization.MultiDomainHost,
                ApplicationBase = AppDomain.CurrentDomain.BaseDirectory
            };

            // [XpKiller] - Mono workaround.
            if (Type.GetType("Mono.Runtime") != null) {
                setup.PrivateBinPath = AppDomain.CurrentDomain.BaseDirectory;
            }

            // TODO: - The previous two lines used to use the constant: Defines.PLUGINS_DIRECTORY.
            // However, when I (Imisnew2) was doing core changes, I fubared up the config loading, causing
            // the plugins to load "Debug\Plugins\Localization" instead of "Debug\Localizations" due to the
            // directory being a compilation of CurrentDomain + BaseDirectory.  To counter this, we set the
            // app domains directory to this app domains directory.  Must set permissions or get phogue to
            // remember stuff later.
            
            return setup;
        }

        /// <summary>
        /// Creates the permissions set to apply to the app domain.
        /// </summary>
        /// <returns></returns>
        protected PermissionSet CreatePermissionSet() {
            PermissionSet permissions = new PermissionSet(PermissionState.None);
            
            permissions.AddPermission(new SecurityPermission(SecurityPermissionFlag.Execution));

            permissions.AddPermission(new FileIOPermission(FileIOPermissionAccess.PathDiscovery, AppDomain.CurrentDomain.BaseDirectory));
            permissions.AddPermission(new FileIOPermission(FileIOPermissionAccess.AllAccess, Defines.PluginsDirectory));
            permissions.AddPermission(new FileIOPermission(FileIOPermissionAccess.AllAccess, Defines.LogsDirectory));
            permissions.AddPermission(new FileIOPermission(FileIOPermissionAccess.Read | FileIOPermissionAccess.PathDiscovery, Defines.LocalizationDirectory));
            permissions.AddPermission(new FileIOPermission(FileIOPermissionAccess.Read | FileIOPermissionAccess.PathDiscovery, Defines.ConfigsDirectory));
            permissions.AddPermission(new FileIOPermission(FileIOPermissionAccess.AllAccess, Path.Combine(Defines.ConfigsDirectory, this.Connection != null ? this.Connection.ConnectionModel.ConnectionGuid.ToString() : Guid.Empty.ToString())));
            
            return permissions;
        }

        public override void WriteConfig(Config config) {
            foreach (PluginModel plugin in this.LoadedPlugins.Where(plugin => plugin.IsEnabled == true)) {
                config.Root.Add(new Command() {
                    CommandType = CommandType.PluginsEnable,
                    Scope = {
                        ConnectionGuid = this.Connection.ConnectionModel.ConnectionGuid,
                        PluginGuid = plugin.PluginGuid
                    }
                }.ToConfigCommand());
            }
        }

        /// <summary>
        /// Executes the commands specified in the config file and returns a reference itself.
        /// </summary>
        public override CoreController Execute() {
            this.SetupPluginFactory();

            this.TunnelObjects.Add(this.PluginFactory);
            this.BubbleObjects.Add(this.Connection);

            // Load all the plugins.
            this.LoadPlugins(new DirectoryInfo(Defines.PluginsDirectory));

            if (this.Connection != null && this.Connection.Game != null) {
                this.Connection.Game.ClientEvent += Connection_ClientEvent;
                this.Connection.Game.GameEvent += Connection_GameEvent;
            }

            // Return the base execution.
            return base.Execute();
        }

        private void Connection_ClientEvent(IGame sender, ClientEventArgs e) {
            if (this.PluginFactory != null) {
                this.PluginFactory.ClientEvent(e);
            }
        }

        private void Connection_GameEvent(IGame sender, GameEventArgs e) {
            if (this.PluginFactory != null) {
                this.PluginFactory.GameEvent(e);
            }
        }

        /// <summary>
        /// Sets everything up to load the plugins, creating the seperate appdomin and permission requirements
        /// </summary>
        protected void SetupPluginFactory() {
            // Make sure the plugins directory exists and set it up.
            Directory.CreateDirectory(Defines.PluginsDirectory);

            this.CreatePluginDirectory(new DirectoryInfo(Defines.PluginsDirectory));

            AppDomainSetup setup = this.CreateAppDomainSetup();

            PermissionSet permissions = this.CreatePermissionSet();

            // Create the app domain and the plugin factory in the new domain.
            this.AppDomainSandbox = AppDomain.CreateDomain(String.Format("Procon.{0}.Plugin", this.Connection != null ? this.Connection.ConnectionModel.ConnectionGuid.ToString() : String.Empty), null, setup, permissions);

            this.PluginFactory = (IRemotePluginController)this.AppDomainSandbox.CreateInstanceAndUnwrap(typeof(SandboxPluginController).Assembly.FullName, typeof(SandboxPluginController).FullName);

            this.PluginFactory.BubbleObjects = new List<ICoreController>() {
                this.CorePluginControllerCallbackProxy
            };
        }

        /// <summary>
        /// Setup the plugins located in or in sub-folders of this directory.
        /// </summary>
        protected void LoadPlugins(DirectoryInfo pluginDirectory) {
            // Find all the dll files recursively within the folder and folders within the specified directory.
            var files = pluginDirectory.GetFiles("*.dll", SearchOption.AllDirectories).Where(file =>
                                                                  file.Name != Defines.ProconCoreDll &&
                                                                  file.Name != Defines.ProconNetDll &&
                                                                  file.Name != Defines.ProconFuzzyDll &&
                                                                  file.Name != Defines.NewtonsoftJsonDll &&
                                                                  file.Name != Defines.ProconCoreSharedDll &&
                                                                  file.Name != Defines.ProconDatabaseSharedDll &&
                                                                  file.Name != Defines.ProconDatabaseSerializationDll &&
                                                                  file.Name != Defines.ProconNetSharedDll);

            // If there are dll files in this directory, setup the plugins.
            foreach (String path in files.Select(file => file.FullName)) {
                PluginModel plugin = new PluginModel() {
                    Name = new FileInfo(path).Name.Replace(".dll", "")
                };

                IPluginController proxy = this.PluginFactory.Create(path, plugin.Name + ".Program");

                if (proxy != null) {
                    plugin.PluginGuid = proxy.PluginGuid;

                    // register game specific call backs.
                    proxy.ConnectionGuid = this.Connection != null ? this.Connection.ConnectionModel.ConnectionGuid : Guid.Empty;

                    // check the plugin's config directory
                    proxy.ConfigDirectoryInfo = new DirectoryInfo(System.IO.Path.Combine(Defines.ConfigsDirectory, proxy.ConnectionGuid.ToString(), plugin.PluginGuid.ToString()));
                    proxy.ConfigDirectoryInfo.Create();

                    // check the plugin's log directory
                    proxy.LogDirectoryInfo = new DirectoryInfo(System.IO.Path.Combine(Defines.LogsDirectory, proxy.ConnectionGuid.ToString(), plugin.PluginGuid.ToString()));
                    proxy.LogDirectoryInfo.Create();

                    // Tell the plugin it's ready to begin, everything is setup and ready 
                    // for it to start loading its config.
                    proxy.GenericEvent(new GenericEventArgs() {
                        GenericEventType = GenericEventType.PluginsPluginLoaded
                    });
                }

                this.LoadedPlugins.Add(plugin);
            }
        }

        /// <summary>
        /// Renews the lease on the plugin factory as well as each loaded plugin proxy
        /// </summary>
        public void RenewLease() {
            ILease lease = ((MarshalByRefObject)this.PluginFactory).GetLifetimeService() as ILease;

            if (lease != null) {
                lease.Renew(lease.InitialLeaseTime);
            }

            lease = this.GetLifetimeService() as ILease;

            if (lease != null) {
                lease.Renew(lease.InitialLeaseTime);
            }
        }

        /// <summary>
        /// Disposes of all the plugins before calling the base dispose.
        /// </summary>
        public override void Dispose() {
            this.PluginFactory.Shutdown();

            this.LoadedPlugins.Clear();
            this.LoadedPlugins = null;

            AppDomain.Unload(this.AppDomainSandbox);
            this.AppDomainSandbox = null;
            this.PluginFactory = null;

            base.Dispose();
        }
    }
}