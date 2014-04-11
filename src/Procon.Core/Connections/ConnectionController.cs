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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security;
using System.Security.Permissions;
using Procon.Core.Connections.Plugins;
using Procon.Core.Connections.TextCommands;
using Procon.Core.Shared;
using Procon.Core.Shared.Events;
using Procon.Core.Shared.Models;
using Procon.Net.Shared;
using Procon.Net.Shared.Actions;
using Procon.Net.Shared.Models;
using Procon.Net.Shared.Sandbox;
using Procon.Net.Shared.Utils;
using Procon.Service.Shared;

namespace Procon.Core.Connections {
    /// <summary>
    /// Handles connections, plugins and text commands for a single game server.
    /// </summary>
    [Serializable]
    public class ConnectionController : CoreController, ISharedReferenceAccess, IConnectionController {
        public ConnectionModel ConnectionModel { get; set; }

        /// <summary>
        ///  The actual game object
        /// </summary>
        public ISandboxProtocolController Protocol { get; set; }

        /// <summary>
        /// Fired when a protocol event is recieved from the protocol appdomain.
        /// </summary>
        public event Action<IProtocolEventArgs> ProtocolEvent;

        /// <summary>
        /// Fired when a client event is recieved from the protocol appdomain.
        /// </summary>
        public event Action<IClientEventArgs> ClientEvent;

        /// <summary>
        /// The appdomain where the protocol is loaded and operates in.
        /// </summary>
        public AppDomain AppDomainSandbox { get; protected set; }

        /// <summary>
        /// Simple protocol factory reference to load classes into the app domain.
        /// </summary>
        public ISandboxProtocolController ProtocolFactory { get; protected set; }

        /// <summary>
        /// The controller to load up and manage plugins
        /// </summary>
        public ICorePluginController Plugins { get; set; }

        /// <summary>
        /// Proxy to the active protocol state
        /// </summary>
        public IProtocolState ProtocolState {
            get { return this.Protocol != null ? this.Protocol.State : null; }
        }

        /// <summary>
        /// Text command controller to pipe all text chats through for analysis of text commands.
        /// </summary>
        public ICoreController TextCommands { get; protected set; }

        /// <summary>
        /// The instance of procon that owns this connection.
        /// </summary>
        public InstanceController Instance { get; set; }

        public SharedReferences Shared { get; private set; }

        /// <summary>
        /// Initializes the connection controller with default values, setting up command dispatches
        /// </summary>
        public ConnectionController() : base() {
            this.Shared = new SharedReferences();

            this.ConnectionModel = new ConnectionModel();

            this.CommandDispatchers.AddRange(new List<ICommandDispatch>() {
                new CommandDispatch() {
                    CommandType = CommandType.ConnectionQuery,
                    Handler = this.ConnectionQuery
                },
                new CommandDispatch() {
                    CommandType = CommandType.NetworkProtocolQueryPlayers,
                    Handler = this.NetworkProtocolQueryPlayers
                },
                new CommandDispatch() {
                    CommandType = CommandType.NetworkProtocolQuerySettings,
                    Handler = this.NetworkProtocolQuerySettings
                },
                new CommandDispatch() {
                    CommandType = CommandType.NetworkProtocolQueryBans,
                    Handler = this.NetworkProtocolQueryBans
                },
                new CommandDispatch() {
                    CommandType = CommandType.NetworkProtocolQueryMaps,
                    Handler = this.NetworkProtocolQueryMaps
                },
                new CommandDispatch() {
                    CommandType = CommandType.NetworkProtocolQueryMapPool,
                    Handler = this.NetworkProtocolQueryMapPool
                }
            });

            // Add all network actions, dispatching them to NetworkProtocolAction
            this.CommandDispatchers.AddRange(Enum.GetValues(typeof(NetworkActionType)).Cast<NetworkActionType>().Select(actionType => new CommandDispatch() {
                Name = actionType.ToString(),
                ParameterTypes = new List<CommandParameterType>() {
                    new CommandParameterType() {
                        Name = "action",
                        Type = typeof(INetworkAction)
                    }
                },
                Handler = this.NetworkProtocolAction
            }));

            this.CommandDispatchers.AddRange(Enum.GetValues(typeof(NetworkActionType)).Cast<NetworkActionType>().Select(actionType => new CommandDispatch() {
                Name = actionType.ToString(),
                ParameterTypes = new List<CommandParameterType>() {
                    new CommandParameterType() {
                        Name = "action",
                        Type = typeof(INetworkAction),
                        IsList = true
                    }
                },
                Handler = this.NetworkProtocolActions
            }));
        }

        public override void WriteConfig(IConfig config, string password) {
            if (this.Plugins != null) {
                this.Plugins.WriteConfig(config, password);
            }
        }

        /// <summary>
        /// Create the app domain setup options required to create the app domain.
        /// </summary>
        /// <returns></returns>
        public AppDomainSetup CreateAppDomainSetup() {
            AppDomainSetup setup = new AppDomainSetup {
                LoaderOptimization = LoaderOptimization.MultiDomain,
                ApplicationBase = AppDomain.CurrentDomain.BaseDirectory,
                PrivateBinPath = String.Join(";", new[] {
                    AppDomain.CurrentDomain.BaseDirectory,
                    Defines.PackageMyrconProconCoreLibNet40.FullName,
                    Defines.PackageMyrconProconSharedLibNet40.FullName
                })
            };

            return setup;
        }

        /// <summary>
        /// Creates the permissions set to apply to the app domain.
        /// </summary>
        /// <returns></returns>
        public PermissionSet CreatePermissionSet(IProtocolAssemblyMetadata meta) {
            PermissionSet permissions = new PermissionSet(PermissionState.None);

            permissions.AddPermission(new SecurityPermission(SecurityPermissionFlag.Execution));

            permissions.AddPermission(new DnsPermission(PermissionState.Unrestricted));
            permissions.AddPermission(new WebPermission(PermissionState.Unrestricted));
            permissions.AddPermission(new SocketPermission(NetworkAccess.Connect, TransportType.All, "*.*.*.*", SocketPermission.AllPorts));

            permissions.AddPermission(new FileIOPermission(FileIOPermissionAccess.PathDiscovery, AppDomain.CurrentDomain.BaseDirectory));
            permissions.AddPermission(new FileIOPermission(FileIOPermissionAccess.AllAccess, meta.Directory.FullName));

            DirectoryInfo coreSharedPackageDirectory = Defines.PackageContainingPath(Defines.PackageMyrconProconSharedLibNet40.FullName);

            if (coreSharedPackageDirectory != null) {
                permissions.AddPermission(new FileIOPermission(FileIOPermissionAccess.Read | FileIOPermissionAccess.PathDiscovery, coreSharedPackageDirectory.FullName));
            }

            return permissions;
        }

        /// <summary>
        /// Sets everything up to load the plugins, creating the seperate appdomin and permission requirements
        /// </summary>
        public void SetupProtocolFactory(IProtocolAssemblyMetadata meta) {
            AppDomainSetup setup = this.CreateAppDomainSetup();

            PermissionSet permissions = this.CreatePermissionSet(meta);

            // Create the app domain and the plugin factory in the new domain.
            this.AppDomainSandbox = AppDomain.CreateDomain(String.Format("Procon.Protocols.{0}", this.ConnectionModel != null ? this.ConnectionModel.ConnectionGuid.ToString() : String.Empty), null, setup, permissions);

            this.ProtocolFactory = (ISandboxProtocolController)this.AppDomainSandbox.CreateInstanceAndUnwrap(typeof(ISandboxProtocolController).Assembly.FullName, typeof(SandboxProtocolController).FullName);

            this.AssignProtocolEvents();
        }

        /// <summary>
        /// Assign all current event handlers.
        /// </summary>
        protected void AssignProtocolEvents() {
            this.UnassignProtocolEvents();

            if (this.ProtocolFactory != null) {
                this.ProtocolFactory.Bubble = new SandboxProtocolCallbackProxy() {
                    ProtocolEvent = Protocol_ProtocolEvent,
                    ClientEvent = Protocol_ClientEvent
                };
            }
        }

        /// <summary>
        /// Removes all current event handlers.
        /// </summary>
        protected void UnassignProtocolEvents() {
            if (this.ProtocolFactory != null) {
                this.ProtocolFactory.Bubble = null;
            }
        }

        /// <summary>
        /// Sets up the factory, then attempts to load a specific type into the protocol appdomain.
        /// </summary>
        public bool SetupProtocol(IProtocolAssemblyMetadata meta, IProtocolType type, ProtocolSetup setup) {
            if (this.ProtocolFactory == null) {
                this.SetupProtocolFactory(meta);
            }

            if (this.ProtocolFactory.Create(meta.Assembly.FullName, type) == true) {
                this.Protocol = this.ProtocolFactory;

                this.Protocol.Setup(setup);
            }

            return this.Protocol != null;
        }

        public override ICoreController Execute() {
            if (this.Protocol != null) {
                this.ConnectionModel.ProtocolType = this.Protocol.ProtocolType as ProtocolType;
                this.ConnectionModel.Hostname = this.Protocol.Options.Hostname;
                this.ConnectionModel.Port = this.Protocol.Options.Port;
                this.ConnectionModel.Password = this.Protocol.Options.Password;
                this.ConnectionModel.Arguments = this.Protocol.Options.ArgumentsString();
            }

            this.ConnectionModel.ConnectionGuid = MD5.Guid(String.Format("{0}:{1}:{2}", this.ConnectionModel.ProtocolType, this.ConnectionModel.Hostname, this.ConnectionModel.Port));

            this.TextCommands = new TextCommandController() {
                Connection = this
            };

            this.Plugins = new CorePluginController() {
                Connection = this
            };

            // Go up to the the instance that owns this connection
            this.BubbleObjects.Add(this.Instance);

            // Go down to the text commands or plugins owned by this connection.
            this.TunnelObjects.Add(this.TextCommands);
            this.TunnelObjects.Add(this.Plugins);

            // Registered bubble/tunnel objects, now go.
            this.TextCommands.Execute();
            this.Plugins.Execute();
            
            // Set the default ignore list.
            this.Shared.Variables.Set(new Command() {
                Origin = CommandOrigin.Local
            }, CommonVariableNames.ProtocolEventsIgnoreList, new List<String>() {
                ProtocolEventType.ProtocolBanlistUpdated.ToString(), 
                //GameEventType.GamePlayerlistUpdated.ToString(), 
                //GameEventType.GameSettingsUpdated.ToString(), 
                ProtocolEventType.ProtocolMaplistUpdated.ToString(), 
            });

            return base.Execute();
        }

        public override void Poke() {
            base.Poke();

            if (this.Protocol != null) {
                if (this.Protocol.State != null && this.Protocol.State.Settings.Current.ConnectionState == ConnectionState.ConnectionDisconnected) {
                    this.AttemptConnection();
                }
                else {
                    this.Protocol.Synchronize();
                }
            }

            if (this.Plugins != null) {
                this.Plugins.Poke();
            }
        }

        public override ICommandResult PropogatePreview(ICommand command, CommandDirection direction) {
            if (direction == CommandDirection.Bubble && command.ScopeModel != null && command.ScopeModel.ConnectionGuid == this.ConnectionModel.ConnectionGuid) {
                // We've bubbled up far enough, time to tunnel down this connection to find our result.
                return base.PropogatePreview(command, CommandDirection.Tunnel);
            }

            return base.PropogatePreview(command, direction);
        }

        public override ICommandResult PropogateHandler(ICommand command, CommandDirection direction) {
            if (direction == CommandDirection.Bubble && command.ScopeModel != null && command.ScopeModel.ConnectionGuid == this.ConnectionModel.ConnectionGuid) {
                // We've bubbled up far enough, time to tunnel down this connection to find our result.
                return base.PropogateHandler(command, CommandDirection.Tunnel);
            }

            return base.PropogateHandler(command, direction);
        }

        public override ICommandResult PropogateExecuted(ICommand command, CommandDirection direction) {
            if (direction == CommandDirection.Bubble && command.ScopeModel != null && command.ScopeModel.ConnectionGuid == this.ConnectionModel.ConnectionGuid) {
                // We've bubbled up far enough, time to tunnel down this connection to find our result.
                return base.PropogateExecuted(command, CommandDirection.Tunnel);
            }

            return base.PropogateExecuted(command, direction);
        }

        /// <summary>
        /// Attempts communication with the game server.
        /// </summary>
        public void AttemptConnection() {
            if (this.Protocol != null) {
                this.Protocol.AttemptConnection();
            }
        }

        /// <summary>
        /// Queries for information about the current connection
        /// </summary>
        public ICommandResult ConnectionQuery(ICommand command, Dictionary<String, ICommandParameter> parameters) {
            ICommandResult result = null;

            if (this.Shared.Security.DispatchPermissionsCheck(command, command.Name).Success == true) {
                ICommandResult players = this.Tunnel(new Command(command) {
                    CommandType = CommandType.NetworkProtocolQueryPlayers
                });

                ICommandResult settings = this.Tunnel(new Command(command) {
                    CommandType = CommandType.NetworkProtocolQuerySettings
                });

                ICommandResult maps = this.Tunnel(new Command(command) {
                    CommandType = CommandType.NetworkProtocolQueryMaps
                });

                result = new CommandResult() {
                    Success = true,
                    CommandResultType = CommandResultType.Success,
                    Scope = new CommandData() {
                        Connections = new List<ConnectionModel>() {
                            this.ConnectionModel
                        }
                    },
                    Now = new CommandData() {
                        // I didn't want plugins to be able to hide themselves.
                        Plugins = new List<PluginModel>(this.Plugins.LoadedPlugins),
                        Players = players.Now.Players,
                        Settings = settings.Now.Settings,
                        Maps = maps.Now.Maps
                    }
                };
            }
            else {
                result = CommandResult.InsufficientPermissions;
            }

            return result;
        }

        /// <summary>
        /// Queries this connection for an up to date list of players
        /// </summary>
        public ICommandResult NetworkProtocolQueryPlayers(ICommand command, Dictionary<String, ICommandParameter> parameters) {
            ICommandResult result = null;

            if (this.Shared.Security.DispatchPermissionsCheck(command, command.Name).Success == true) {
                result = new CommandResult() {
                    Success = true,
                    CommandResultType = CommandResultType.Success,
                    Scope = new CommandData() {
                        Connections = new List<ConnectionModel>() {
                            this.ConnectionModel
                        }
                    },
                    Now = new CommandData() {
                        Players = new List<PlayerModel>(this.Protocol.State.Players)
                    }
                };
            }
            else {
                result = CommandResult.InsufficientPermissions;
            }

            return result;
        }

        /// <summary>
        /// Queries this connection for the current protocol settings
        /// </summary>
        public ICommandResult NetworkProtocolQuerySettings(ICommand command, Dictionary<String, ICommandParameter> parameters) {
            ICommandResult result = null;

            if (this.Shared.Security.DispatchPermissionsCheck(command, command.Name).Success == true) {
                result = new CommandResult() {
                    Success = true,
                    CommandResultType = CommandResultType.Success,
                    Scope = new CommandData() {
                        Connections = new List<ConnectionModel>() {
                            this.ConnectionModel
                        }
                    },
                    Now = new CommandData() {
                        Settings = new List<Settings>() {
                            this.Protocol.State.Settings
                        }
                    }
                };
            }
            else {
                result = CommandResult.InsufficientPermissions;
            }

            return result;
        }

        /// <summary>
        /// Queries this connection for a complete list of active bans
        /// </summary>
        public ICommandResult NetworkProtocolQueryBans(ICommand command, Dictionary<String, ICommandParameter> parameters) {
            ICommandResult result = null;

            if (this.Shared.Security.DispatchPermissionsCheck(command, command.Name).Success == true) {
                result = new CommandResult() {
                    Success = true,
                    CommandResultType = CommandResultType.Success,
                    Scope = new CommandData() {
                        Connections = new List<ConnectionModel>() {
                            this.ConnectionModel
                        }
                    },
                    Now = new CommandData() {
                        Bans = new List<BanModel>(this.Protocol.State.Bans)
                    }
                };
            }
            else {
                result = CommandResult.InsufficientPermissions;
            }

            return result;
        }

        /// <summary>
        /// Queries this connection for a list of maps currently running
        /// </summary>
        public ICommandResult NetworkProtocolQueryMaps(ICommand command, Dictionary<String, ICommandParameter> parameters) {
            ICommandResult result = null;

            if (this.Shared.Security.DispatchPermissionsCheck(command, command.Name).Success == true) {
                result = new CommandResult() {
                    Success = true,
                    CommandResultType = CommandResultType.Success,
                    Scope = new CommandData() {
                        Connections = new List<ConnectionModel>() {
                            this.ConnectionModel
                        }
                    },
                    Now = new CommandData() {
                        Maps = new List<MapModel>(this.Protocol.State.Maps)
                    }
                };
            }
            else {
                result = CommandResult.InsufficientPermissions;
            }

            return result;
        }

        /// <summary>
        /// Queries this connection for a list of maps available
        /// </summary>
        public ICommandResult NetworkProtocolQueryMapPool(ICommand command, Dictionary<String, ICommandParameter> parameters) {
            ICommandResult result = null;

            if (this.Shared.Security.DispatchPermissionsCheck(command, command.Name).Success == true) {
                result = new CommandResult() {
                    Success = true,
                    CommandResultType = CommandResultType.Success,
                    Scope = new CommandData() {
                        Connections = new List<ConnectionModel>() {
                            this.ConnectionModel
                        }
                    },
                    Now = new CommandData() {
                        Maps = new List<MapModel>(this.Protocol.State.MapPool)
                    }
                };
            }
            else {
                result = CommandResult.InsufficientPermissions;
            }

            return result;
        }

        /// <summary>
        /// Executes a single action on the protocol
        /// </summary>
        public ICommandResult NetworkProtocolAction(ICommand command, Dictionary<String, ICommandParameter> parameters) {
            ICommandResult result = null;

            INetworkAction action = parameters["action"].First<INetworkAction>();

            if (this.Shared.Security.DispatchPermissionsCheck(command, command.Name).Success == true) {
                action.Name = command.Name;

                result = new CommandResult() {
                    Success = true,
                    CommandResultType = CommandResultType.Success,
                    Now = new CommandData() {
                        Packets = this.Protocol.Action(action)
                    }
                };
            }
            else {
                result = CommandResult.InsufficientPermissions;
            }

            return result;
        }

        /// <summary>
        /// Executes a list of actions on the protocol, ensuring each action matches the authenticated command
        /// </summary>
        /// <param name="command"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public ICommandResult NetworkProtocolActions(ICommand command, Dictionary<String, ICommandParameter> parameters) {
            ICommandResult result = null;

            List<INetworkAction> actions = parameters["actions"].All<INetworkAction>();

            if (this.Shared.Security.DispatchPermissionsCheck(command, command.Name).Success == true) {
                List<IPacket> packets = new List<IPacket>();

                foreach (INetworkAction action in actions) {
                    action.Name = command.Name;

                    packets.AddRange(this.Protocol.Action(action));
                }

                result = new CommandResult() {
                    Success = true,
                    CommandResultType = CommandResultType.Success,
                    Now = new CommandData() {
                        Packets = packets
                    }
                };
            }
            else {
                result = CommandResult.InsufficientPermissions;
            }

            return result;
        }

        private void Protocol_ClientEvent(IClientEventArgs e) {
            if (e.EventType == ClientEventType.ClientConnectionStateChange) {
                if (this.Protocol != null) {
                    var options = this.Protocol.Options;

                    if (options != null) {
                        this.ConnectionModel.ProtocolType = this.Protocol.ProtocolType as ProtocolType;
                        this.ConnectionModel.Hostname = options.Hostname;
                        this.ConnectionModel.Port = options.Port;

                        // Once connected, sync the connection.
                        if (e.ConnectionState == ConnectionState.ConnectionLoggedIn) {
                            this.Protocol.Synchronize();
                        }
                    }
                }

                this.Shared.Events.Log(new GenericEvent() {
                    Name = e.ConnectionState.ToString(),
                    Scope = new CommandData() {
                        Connections = new List<ConnectionModel>() {
                            this.ConnectionModel
                        }
                    },
                    Stamp = e.Stamp
                });
            }
            else if (e.EventType == ClientEventType.ClientConnectionFailure || e.EventType == ClientEventType.ClientSocketException) {
                this.Shared.Events.Log(new GenericEvent() {
                    Name = e.EventType.ToString(),
                    Message = e.Now.Exceptions.FirstOrDefault(),
                    Scope = new CommandData() {
                        Connections = new List<ConnectionModel>() {
                            this.ConnectionModel
                        }
                    },
                    Stamp = e.Stamp
                });
            }

            if (this.ClientEvent != null) {
                this.ClientEvent(e);
            }
        }

        /// <summary>
        /// This function is pretty much a plugin on it's own that only dispatches events from the game.
        /// 
        /// I might put this into it's own class at some point to seperate the GameConnection from an action Procon is taking.
        /// </summary>
        /// <param name="e"></param>
        private void Protocol_ProtocolEvent(IProtocolEventArgs e) {
            if (this.Shared.Variables.Get<List<String>>(CommonVariableNames.ProtocolEventsIgnoreList).Contains(e.ProtocolEventType.ToString()) == false) {
                this.Shared.Events.Log(new GenericEvent() {
                    Name = e.ProtocolEventType.ToString(),
                    Then = GenericEventData.Parse(e.Then),
                    Now = GenericEventData.Parse(e.Now),
                    Scope = new CommandData() {
                        Connections = new List<ConnectionModel>() {
                            this.ConnectionModel
                        }
                    },
                    Stamp = e.Stamp
                });
            }

            if (e.ProtocolEventType == ProtocolEventType.ProtocolChat) {
                ChatModel chat = e.Now.Chats.First();

                // At least has the first prefix character 
                // and a little something-something to pass to
                // the parser.
                if (chat.Now.Content != null && chat.Now.Content.Count > 0 && chat.Now.Content.First().Length >= 2) {
                    String prefix = chat.Now.Content.First().First().ToString(CultureInfo.InvariantCulture);
                    String text = chat.Now.Content.First().Remove(0, 1);

                    bool execute = prefix == this.Shared.Variables.Get<String>(CommonVariableNames.TextCommandPublicPrefix) || prefix == this.Shared.Variables.Get<String>(CommonVariableNames.TextCommandProtectedPrefix) || prefix == this.Shared.Variables.Get<String>(CommonVariableNames.TextCommandPrivatePrefix);

                    if (execute == true) {
                        this.Tunnel(new Command() {
                            Origin = CommandOrigin.Plugin,
                            Authentication = {
                                GameType = this.ConnectionModel.ProtocolType.Type,
                                Uid = chat.Now.Players.First().Uid
                            },
                            CommandType = CommandType.TextCommandsExecute,
                            Parameters = new List<ICommandParameter>() {
                                new CommandParameter() {
                                    Data = {
                                        Content = new List<String>() {
                                            text
                                        }
                                    }
                                }
                            }
                        });
                    }
                }
            }

            if (this.ProtocolEvent != null) {
                this.ProtocolEvent(e);
            }
        }

        public override void Dispose() {

            this.ConnectionModel.ProtocolType = null;
            this.ConnectionModel.Hostname = null;
            this.ConnectionModel.Port = 0;

            // Now shutdown and null out the game. Note that we want to capture and report
            // events during the shutdown, but then we want to unassign events to the game
            // object before we null it out. We only null it so we dont suppress errors.
            if (this.Protocol != null) {
                this.Protocol.Shutdown();
            }

            if (this.AppDomainSandbox != null) {
                AppDomain.Unload(this.AppDomainSandbox);
                this.AppDomainSandbox = null;
            }

            this.ProtocolFactory = null;

            this.UnassignProtocolEvents();

            // this.Game.Dispose();
            this.Protocol = null;

            if (this.TextCommands != null) {
                this.TextCommands.Dispose();
            }

            if (this.Plugins != null) {
                this.Plugins.Dispose();
            }

            base.Dispose();
        }
    }
}
