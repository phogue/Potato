using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Remoting.Lifetime;
using System.Security;
using System.Security.Permissions;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Procon.Core.Events;
using Procon.Net;
using Procon.Service.Shared;

namespace Procon.Core.Connections.Plugins {
    /// <summary>
    /// Manages loading and propogating plugin events, as well as callbacks from
    /// a plugin back to Procon.
    /// </summary>
    public class PluginController : Executable, IRenewableLease {

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
        public List<HostPlugin> Plugins { get; protected set; }

        /// <summary>
        /// The connection which owns this plugin app domain and the connection which the plugins control.
        /// </summary>
        [XmlIgnore, JsonIgnore]
        public Connection Connection { get; set; }

        /// <summary>
        /// Default Initialization
        /// </summary>
        public PluginController() : base() {
            this.Plugins = new List<HostPlugin>();

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

                if (this.Plugins.Count(plugin => plugin.PluginGuid == pluginGuid) > 0) {
                    if (this.PluginFactory.TryEnablePlugin(pluginGuid) == true) {
                        result = new CommandResultArgs() {
                            Status = CommandResultType.Success,
                            Success = true,
                            Message = String.Format("Plugin {0} has been enabled", pluginGuid),
                            Scope = {
                                Connections = new List<Connection>() {
                                    this.Connection
                                },
                                Plugins = new List<HostPlugin>() {
                                    this.Plugins.First(plugin => plugin.PluginGuid == pluginGuid)
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

                if (this.Plugins.Count(plugin => plugin.PluginGuid == pluginGuid) > 0) {
                    if (this.PluginFactory.TryDisablePlugin(pluginGuid) == true) {
                        result = new CommandResultArgs() {
                            Status = CommandResultType.Success,
                            Success = true,
                            Message = String.Format("Plugin {0} has been disabled", pluginGuid),
                            Scope = {
                                Connections = new List<Connection>() {
                                    this.Connection
                                },
                                Plugins = new List<HostPlugin>() {
                                    this.Plugins.First(plugin => plugin.PluginGuid == pluginGuid)
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
            try {
                File.Copy(Defines.ProconDirectoryProconCoreDll, Path.Combine(pluginDirectory.FullName, Defines.ProconCoreDll), true);
                File.Copy(Defines.ProconDirectoryProconNetDll, Path.Combine(pluginDirectory.FullName, Defines.ProconNetDll), true);
                // File.Copy(Defines.ProconDirectoryNewtonsoftJsonNet35Dll, Path.Combine(pluginDirectory.FullName, Defines.NewtonsoftJsonNet35Dll), true);
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
            permissions.AddPermission(new FileIOPermission(FileIOPermissionAccess.AllAccess, Path.Combine(Defines.ConfigsDirectory, this.Connection != null ? this.Connection.ConnectionGuid.ToString() : Guid.Empty.ToString())));
            
            permissions.AddPermission(new ReflectionPermission(ReflectionPermissionFlag.MemberAccess));

            return permissions;
        }

        public override void WriteConfig(Config config) {
            foreach (HostPlugin plugin in this.Plugins.Where(plugin => plugin.IsEnabled == true)) {
                config.Root.Add(new Command() {
                    CommandType = CommandType.PluginsEnable,
                    Scope = {
                        ConnectionGuid = this.Connection.ConnectionGuid,
                        PluginGuid = plugin.PluginGuid
                    }
                }.ToConfigCommand());
            }
        }

        /// <summary>
        /// Executes the commands specified in the config file and returns a reference itself.
        /// </summary>
        public override ExecutableBase Execute() {
            this.SetupPluginFactory();

            this.TunnelObjects.Add(this.PluginFactory);
            this.BubbleObjects.Add(this.Connection);

            // Load all the plugins.
            this.LoadPlugins(new DirectoryInfo(Defines.PluginsDirectory));

            if (this.Connection != null && this.Connection.Game != null) {
                this.Connection.Game.ClientEvent += new Game.ClientEventHandler(Connection_ClientEvent);
                this.Connection.Game.GameEvent += new Game.GameEventHandler(Connection_GameEvent);
            }

            // Return the base execution.
            return base.Execute();
        }

        private void Connection_ClientEvent(Game sender, ClientEventArgs e) {
            if (this.PluginFactory != null) {
                this.PluginFactory.ClientEvent(e);
            }
        }

        private void Connection_GameEvent(Game sender, GameEventArgs e) {
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
            this.AppDomainSandbox = AppDomain.CreateDomain(String.Format("Procon.{0}.Plugin", this.Connection != null ? this.Connection.ConnectionGuid.ToString() : String.Empty), null, setup, permissions);

            this.PluginFactory = (IRemotePluginController)this.AppDomainSandbox.CreateInstanceAndUnwrap(Assembly.GetExecutingAssembly().FullName, typeof(RemotePluginController).FullName);

            this.PluginFactory.BubbleObjects = new List<IExecutableBase>() {
                this
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
                                                                  file.Name != Defines.NewtonsoftJsonDll);

            // If there are dll files in this directory, setup the plugins.
            foreach (String path in files.Select(file => file.FullName)) {
                this.Plugins.Add(new HostPlugin() {
                    Path = path,
                    PluginFactory = PluginFactory,
                    ConnectionGuid = this.Connection != null ? this.Connection.ConnectionGuid : Guid.Empty
                }.Execute() as HostPlugin);
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

            foreach (HostPlugin plugin in this.Plugins) {
                plugin.RenewLease();
            }
        }

        /// <summary>
        /// Disposes of all the plugins before calling the base dispose.
        /// </summary>
        public override void Dispose() {
            this.PluginFactory.Shutdown();

            foreach (HostPlugin plugin in this.Plugins) {
                plugin.Dispose();
            }

            this.Plugins.Clear();
            this.Plugins = null;

            AppDomain.Unload(this.AppDomainSandbox);
            this.AppDomainSandbox = null;
            this.PluginFactory = null;

            base.Dispose();
        }
    }
}
