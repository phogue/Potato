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
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Lifetime;
using System.Security;
using System.Security.Permissions;
using Potato.Core.Connections.Plugins;
using Potato.Core.Connections.TextCommands;
using Potato.Core.Shared;
using Potato.Core.Shared.Events;
using Potato.Core.Shared.Models;
using Potato.Net.Shared;
using Potato.Net.Shared.Actions;
using Potato.Net.Shared.Models;
using Potato.Net.Shared.Sandbox;
using Potato.Net.Shared.Utils;
using Potato.Service.Shared;

namespace Potato.Core.Connections {
    /// <summary>
    /// Handles connections, plugins and text commands for a single game server.
    /// </summary>
    [Serializable]
    public class ConnectionController : AsynchronousCoreController, ISharedReferenceAccess, IConnectionController {
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
        public IProtocolState ProtocolState { get; set; }

        /// <summary>
        /// Text command controller to pipe all text chats through for analysis of text commands.
        /// </summary>
        public ICoreController TextCommands { get; protected set; }

        /// <summary>
        /// The instance of Potato that owns this connection.
        /// </summary>
        public PotatoController Potato { get; set; }

        public SharedReferences Shared { get; private set; }

        /// <summary>
        /// Initializes the connection controller with default values, setting up command dispatches
        /// </summary>
        public ConnectionController() : base() {
            Shared = new SharedReferences();

            ConnectionModel = new ConnectionModel();

            ProtocolState = new ProtocolState();

            CommandDispatchers.AddRange(new List<ICommandDispatch>() {
                new CommandDispatch() {
                    CommandType = CommandType.ConnectionQuery,
                    Handler = ConnectionQuery
                },
                new CommandDispatch() {
                    CommandType = CommandType.NetworkProtocolQueryPlayers,
                    Handler = NetworkProtocolQueryPlayers
                },
                new CommandDispatch() {
                    CommandType = CommandType.NetworkProtocolQuerySettings,
                    Handler = NetworkProtocolQuerySettings
                },
                new CommandDispatch() {
                    CommandType = CommandType.NetworkProtocolQueryBans,
                    Handler = NetworkProtocolQueryBans
                },
                new CommandDispatch() {
                    CommandType = CommandType.NetworkProtocolQueryMaps,
                    Handler = NetworkProtocolQueryMaps
                },
                new CommandDispatch() {
                    CommandType = CommandType.NetworkProtocolQueryMapPool,
                    Handler = NetworkProtocolQueryMapPool
                }
            });

            // Add all network actions, dispatching them to NetworkProtocolAction
            CommandDispatchers.AddRange(Enum.GetValues(typeof(NetworkActionType)).Cast<NetworkActionType>().Select(actionType => new CommandDispatch() {
                Name = actionType.ToString(),
                ParameterTypes = new List<CommandParameterType>() {
                    new CommandParameterType() {
                        Name = "action",
                        Type = typeof(INetworkAction)
                    }
                },
                Handler = NetworkProtocolAction
            }));

            CommandDispatchers.AddRange(Enum.GetValues(typeof(NetworkActionType)).Cast<NetworkActionType>().Select(actionType => new CommandDispatch() {
                Name = actionType.ToString(),
                ParameterTypes = new List<CommandParameterType>() {
                    new CommandParameterType() {
                        Name = "action",
                        Type = typeof(INetworkAction),
                        IsList = true
                    }
                },
                Handler = NetworkProtocolActions
            }));
        }

        public override void WriteConfig(IConfig config, string password = null) {
            if (Plugins != null) {
                Plugins.WriteConfig(config, password);
            }
        }

        /// <summary>
        /// Create the app domain setup options required to create the app domain.
        /// </summary>
        /// <returns></returns>
        public AppDomainSetup CreateAppDomainSetup() {
            var setup = new AppDomainSetup {
                LoaderOptimization = LoaderOptimization.MultiDomain,
                ApplicationBase = AppDomain.CurrentDomain.BaseDirectory,
                PrivateBinPath = string.Join(";", new[] {
                    AppDomain.CurrentDomain.BaseDirectory,
                    Defines.PackageMyrconPotatoCoreLibNet40.FullName,
                    Defines.PackageMyrconPotatoSharedLibNet40.FullName
                })
            };

            return setup;
        }

        /// <summary>
        /// Creates the permissions set to apply to the app domain.
        /// </summary>
        /// <returns></returns>
        public PermissionSet CreatePermissionSet(IProtocolAssemblyMetadata meta) {
            var permissions = new PermissionSet(PermissionState.None);

            permissions.AddPermission(new SecurityPermission(SecurityPermissionFlag.Execution));

            permissions.AddPermission(new DnsPermission(PermissionState.Unrestricted));
            permissions.AddPermission(new WebPermission(PermissionState.Unrestricted));
            permissions.AddPermission(new SocketPermission(NetworkAccess.Connect, TransportType.All, "*.*.*.*", SocketPermission.AllPorts));

            permissions.AddPermission(new FileIOPermission(FileIOPermissionAccess.PathDiscovery, AppDomain.CurrentDomain.BaseDirectory));
            permissions.AddPermission(new FileIOPermission(FileIOPermissionAccess.AllAccess, meta.Directory.FullName));

            var coreSharedPackageDirectory = Defines.PackageContainingPath(Defines.PackageMyrconPotatoSharedLibNet40.FullName);

            if (coreSharedPackageDirectory != null) {
                permissions.AddPermission(new FileIOPermission(FileIOPermissionAccess.Read | FileIOPermissionAccess.PathDiscovery, coreSharedPackageDirectory.FullName));
            }

            return permissions;
        }

        /// <summary>
        /// Sets everything up to load the plugins, creating the seperate appdomin and permission requirements
        /// </summary>
        public void SetupProtocolFactory(IProtocolAssemblyMetadata meta) {
            var setup = CreateAppDomainSetup();

            var permissions = CreatePermissionSet(meta);

            // Create the app domain and the plugin factory in the new domain.
            AppDomainSandbox = AppDomain.CreateDomain(string.Format("Potato.Protocols.{0}", ConnectionModel != null ? ConnectionModel.ConnectionGuid.ToString() : string.Empty), null, setup, permissions);

            ProtocolFactory = (ISandboxProtocolController)AppDomainSandbox.CreateInstanceAndUnwrap(typeof(ISandboxProtocolController).Assembly.FullName, typeof(SandboxProtocolController).FullName);

            AssignProtocolEvents();
        }

        /// <summary>
        /// Assign all current event handlers.
        /// </summary>
        protected void AssignProtocolEvents() {
            UnassignProtocolEvents();

            if (ProtocolFactory != null) {
                ProtocolFactory.Bubble = new SandboxProtocolCallbackProxy() {
                    ProtocolEvent = Protocol_ProtocolEvent,
                    ClientEvent = Protocol_ClientEvent
                };
            }
        }

        /// <summary>
        /// Removes all current event handlers.
        /// </summary>
        protected void UnassignProtocolEvents() {
            if (ProtocolFactory != null) {
                ProtocolFactory.Bubble = null;
            }
        }

        /// <summary>
        /// Sets up the factory, then attempts to load a specific type into the protocol appdomain.
        /// </summary>
        public bool SetupProtocol(IProtocolAssemblyMetadata meta, IProtocolType type, ProtocolSetup setup) {
            if (ProtocolFactory == null) {
                SetupProtocolFactory(meta);
            }

            if (ProtocolFactory.Create(meta.Assembly.FullName, type) == true) {
                Protocol = ProtocolFactory;

                Protocol.Setup(setup);
            }

            return Protocol != null;
        }

        public override ICoreController Execute() {
            if (Protocol != null) {
                ConnectionModel.ProtocolType = Protocol.ProtocolType as ProtocolType;
                ConnectionModel.Hostname = Protocol.Options.Hostname;
                ConnectionModel.Port = Protocol.Options.Port;
                ConnectionModel.Password = Protocol.Options.Password;
                ConnectionModel.Arguments = Protocol.Options.ArgumentsString();
            }

            ConnectionModel.ConnectionGuid = MD5.Guid(string.Format("{0}:{1}:{2}", ConnectionModel.ProtocolType, ConnectionModel.Hostname, ConnectionModel.Port));

            TextCommands = new TextCommandController() {
                Connection = this
            };

            Plugins = new CorePluginController() {
                Connection = this
            };

            // Go up to the the instance that owns this connection
            BubbleObjects.Add(Potato);

            // Go down to the text commands or plugins owned by this connection.
            TunnelObjects.Add(TextCommands);
            TunnelObjects.Add(Plugins);

            // Registered bubble/tunnel objects, now go.
            TextCommands.Execute();
            Plugins.Execute();
            
            // Set the default ignore list.
            Shared.Variables.Set(new Command() {
                Origin = CommandOrigin.Local
            }, CommonVariableNames.ProtocolEventsIgnoreList, new List<string>() {
                ProtocolEventType.ProtocolBanlistUpdated.ToString(), 
                //GameEventType.GamePlayerlistUpdated.ToString(), 
                //GameEventType.GameSettingsUpdated.ToString(), 
                ProtocolEventType.ProtocolMaplistUpdated.ToString(), 
            });

            return base.Execute();
        }

        public override void Poke() {
            base.Poke();

            if (Protocol != null) {
                if (ProtocolState != null && ProtocolState.Settings.Current.ConnectionState == ConnectionState.ConnectionDisconnected) {
                    AttemptConnection();
                }
                else {
                    Protocol.Synchronize();
                }
            }

            if (Plugins != null) {
                Plugins.Poke();
            }

            // Update the lease on the protocol AppDomain
            if (ProtocolFactory != null) {
                var lease = ((MarshalByRefObject)ProtocolFactory).GetLifetimeService() as ILease;

                if (lease != null) {
                    lease.Renew(lease.InitialLeaseTime);
                }
            }
        }

        public override ICommandResult PropogatePreview(ICommand command, CommandDirection direction) {
            if (direction == CommandDirection.Bubble && command.Scope != null && command.Scope.ConnectionGuid == ConnectionModel.ConnectionGuid) {
                // We've bubbled up far enough, time to tunnel down this connection to find our result.
                return base.PropogatePreview(command, CommandDirection.Tunnel);
            }

            return base.PropogatePreview(command, direction);
        }

        public override ICommandResult PropogateHandler(ICommand command, CommandDirection direction) {
            if (direction == CommandDirection.Bubble && command.Scope != null && command.Scope.ConnectionGuid == ConnectionModel.ConnectionGuid) {
                // We've bubbled up far enough, time to tunnel down this connection to find our result.
                return base.PropogateHandler(command, CommandDirection.Tunnel);
            }

            return base.PropogateHandler(command, direction);
        }

        public override ICommandResult PropogateExecuted(ICommand command, CommandDirection direction) {
            if (direction == CommandDirection.Bubble && command.Scope != null && command.Scope.ConnectionGuid == ConnectionModel.ConnectionGuid) {
                // We've bubbled up far enough, time to tunnel down this connection to find our result.
                return base.PropogateExecuted(command, CommandDirection.Tunnel);
            }

            return base.PropogateExecuted(command, direction);
        }

        /// <summary>
        /// Attempts communication with the game server.
        /// </summary>
        public void AttemptConnection() {
            if (Protocol != null) {
                Protocol.AttemptConnection();
            }
        }

        /// <summary>
        /// Queries for information about the current connection
        /// </summary>
        public ICommandResult ConnectionQuery(ICommand command, Dictionary<string, ICommandParameter> parameters) {
            ICommandResult result = null;

            if (Shared.Security.DispatchPermissionsCheck(command, command.Name).Success == true) {
                var players = Tunnel(new Command(command) {
                    CommandType = CommandType.NetworkProtocolQueryPlayers
                });

                var settings = Tunnel(new Command(command) {
                    CommandType = CommandType.NetworkProtocolQuerySettings
                });

                var maps = Tunnel(new Command(command) {
                    CommandType = CommandType.NetworkProtocolQueryMaps
                });

                result = new CommandResult() {
                    Success = true,
                    CommandResultType = CommandResultType.Success,
                    Scope = new CommandData() {
                        Connections = new List<ConnectionModel>() {
                            ConnectionModel
                        }
                    },
                    Now = new CommandData() {
                        // I didn't want plugins to be able to hide themselves.
                        Plugins = new List<PluginModel>(Plugins.LoadedPlugins),
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
        public ICommandResult NetworkProtocolQueryPlayers(ICommand command, Dictionary<string, ICommandParameter> parameters) {
            ICommandResult result = null;

            if (Shared.Security.DispatchPermissionsCheck(command, command.Name).Success == true) {
                result = new CommandResult() {
                    Success = true,
                    CommandResultType = CommandResultType.Success,
                    Scope = new CommandData() {
                        Connections = new List<ConnectionModel>() {
                            ConnectionModel
                        }
                    },
                    Now = new CommandData() {
                        Players = new List<PlayerModel>(ProtocolState.Players.Values)
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
        public ICommandResult NetworkProtocolQuerySettings(ICommand command, Dictionary<string, ICommandParameter> parameters) {
            ICommandResult result = null;

            if (Shared.Security.DispatchPermissionsCheck(command, command.Name).Success == true) {
                result = new CommandResult() {
                    Success = true,
                    CommandResultType = CommandResultType.Success,
                    Scope = new CommandData() {
                        Connections = new List<ConnectionModel>() {
                            ConnectionModel
                        }
                    },
                    Now = new CommandData() {
                        Settings = new List<Settings>() {
                            ProtocolState.Settings
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
        public ICommandResult NetworkProtocolQueryBans(ICommand command, Dictionary<string, ICommandParameter> parameters) {
            ICommandResult result = null;

            if (Shared.Security.DispatchPermissionsCheck(command, command.Name).Success == true) {
                result = new CommandResult() {
                    Success = true,
                    CommandResultType = CommandResultType.Success,
                    Scope = new CommandData() {
                        Connections = new List<ConnectionModel>() {
                            ConnectionModel
                        }
                    },
                    Now = new CommandData() {
                        Bans = new List<BanModel>(ProtocolState.Bans.Values)
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
        public ICommandResult NetworkProtocolQueryMaps(ICommand command, Dictionary<string, ICommandParameter> parameters) {
            ICommandResult result = null;

            if (Shared.Security.DispatchPermissionsCheck(command, command.Name).Success == true) {
                result = new CommandResult() {
                    Success = true,
                    CommandResultType = CommandResultType.Success,
                    Scope = new CommandData() {
                        Connections = new List<ConnectionModel>() {
                            ConnectionModel
                        }
                    },
                    Now = new CommandData() {
                        Maps = new List<MapModel>(ProtocolState.Maps.Values)
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
        public ICommandResult NetworkProtocolQueryMapPool(ICommand command, Dictionary<string, ICommandParameter> parameters) {
            ICommandResult result = null;

            if (Shared.Security.DispatchPermissionsCheck(command, command.Name).Success == true) {
                result = new CommandResult() {
                    Success = true,
                    CommandResultType = CommandResultType.Success,
                    Scope = new CommandData() {
                        Connections = new List<ConnectionModel>() {
                            ConnectionModel
                        }
                    },
                    Now = new CommandData() {
                        Maps = new List<MapModel>(ProtocolState.MapPool.Values)
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
        public ICommandResult NetworkProtocolAction(ICommand command, Dictionary<string, ICommandParameter> parameters) {
            ICommandResult result = null;

            var action = parameters["action"].First<INetworkAction>();

            if (Shared.Security.DispatchPermissionsCheck(command, command.Name).Success == true) {
                action.Name = command.Name;

                result = new CommandResult() {
                    Success = true,
                    CommandResultType = CommandResultType.Success,
                    Now = new CommandData() {
                        Packets = Protocol.Action(action)
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
        public ICommandResult NetworkProtocolActions(ICommand command, Dictionary<string, ICommandParameter> parameters) {
            ICommandResult result = null;

            var actions = parameters["actions"].All<INetworkAction>();

            if (Shared.Security.DispatchPermissionsCheck(command, command.Name).Success == true) {
                var packets = new List<IPacket>();

                foreach (var action in actions) {
                    action.Name = command.Name;

                    packets.AddRange(Protocol.Action(action));
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
                if (Protocol != null) {
                    var options = Protocol.Options;

                    if (options != null) {
                        ConnectionModel.ProtocolType = Protocol.ProtocolType as ProtocolType;
                        ConnectionModel.Hostname = options.Hostname;
                        ConnectionModel.Port = options.Port;

                        // Once connected, sync the connection.
                        if (e.ConnectionState == ConnectionState.ConnectionLoggedIn) {
                            Protocol.Synchronize();
                        }
                    }
                }

                Shared.Events.Log(new GenericEvent() {
                    Name = e.ConnectionState.ToString(),
                    Scope = new CommandData() {
                        Connections = new List<ConnectionModel>() {
                            ConnectionModel
                        }
                    },
                    Stamp = e.Stamp
                });
            }
            else if (e.EventType == ClientEventType.ClientConnectionFailure || e.EventType == ClientEventType.ClientSocketException) {
                Shared.Events.Log(new GenericEvent() {
                    Name = e.EventType.ToString(),
                    Message = e.Now.Exceptions.FirstOrDefault(),
                    Scope = new CommandData() {
                        Connections = new List<ConnectionModel>() {
                            ConnectionModel
                        }
                    },
                    Stamp = e.Stamp
                });
            }

            if (ClientEvent != null) {
                ClientEvent(e);
            }
        }

        /// <summary>
        /// This function is pretty much a plugin on it's own that only dispatches events from the game.
        /// 
        /// I might put this into it's own class at some point to seperate the GameConnection from an action Potato is taking.
        /// </summary>
        /// <param name="e"></param>
        private void Protocol_ProtocolEvent(IProtocolEventArgs e) {
            if (e.StateDifference != null) {
                ProtocolState.Apply(e.StateDifference);
            }

            if (Shared.Variables.Get<List<string>>(CommonVariableNames.ProtocolEventsIgnoreList).Contains(e.ProtocolEventType.ToString()) == false) {
                Shared.Events.Log(new GenericEvent() {
                    Name = e.ProtocolEventType.ToString(),
                    Then = GenericEventData.Parse(e.Then),
                    Now = GenericEventData.Parse(e.Now),
                    Scope = new CommandData() {
                        Connections = new List<ConnectionModel>() {
                            ConnectionModel
                        }
                    },
                    Stamp = e.Stamp
                });
            }

            if (e.ProtocolEventType == ProtocolEventType.ProtocolChat) {
                var chat = e.Now.Chats.First();

                // At least has the first prefix character 
                // and a little something-something to pass to
                // the parser.
                if (chat.Now.Content != null && chat.Now.Content.Count > 0 && chat.Now.Content.First().Length >= 2) {
                    var prefix = chat.Now.Content.First().First().ToString(CultureInfo.InvariantCulture);
                    var text = chat.Now.Content.First().Remove(0, 1);

                    var execute = prefix == Shared.Variables.Get<string>(CommonVariableNames.TextCommandPublicPrefix) || prefix == Shared.Variables.Get<string>(CommonVariableNames.TextCommandProtectedPrefix) || prefix == Shared.Variables.Get<string>(CommonVariableNames.TextCommandPrivatePrefix);

                    if (execute == true) {
                        Tunnel(new Command() {
                            Origin = CommandOrigin.Plugin,
                            Authentication = {
                                GameType = ConnectionModel.ProtocolType.Type,
                                Uid = chat.Now.Players.First().Uid
                            },
                            CommandType = CommandType.TextCommandsExecute,
                            Parameters = new List<ICommandParameter>() {
                                new CommandParameter() {
                                    Data = {
                                        Content = new List<string>() {
                                            text
                                        }
                                    }
                                }
                            }
                        });
                    }
                }
            }

            if (ProtocolEvent != null) {
                ProtocolEvent(e);
            }
        }

        public override void Dispose() {

            ConnectionModel.ProtocolType = null;
            ConnectionModel.Hostname = null;
            ConnectionModel.Port = 0;

            // Now shutdown and null out the game. Note that we want to capture and report
            // events during the shutdown, but then we want to unassign events to the game
            // object before we null it out. We only null it so we dont suppress errors.
            if (Protocol != null) {
                Protocol.Shutdown();
            }

            if (AppDomainSandbox != null) {
                AppDomain.Unload(AppDomainSandbox);
                AppDomainSandbox = null;
            }

            ProtocolFactory = null;

            UnassignProtocolEvents();

            // this.Game.Dispose();
            Protocol = null;

            if (TextCommands != null) {
                TextCommands.Dispose();
            }

            if (Plugins != null) {
                Plugins.Dispose();
            }

            base.Dispose();
        }
    }
}
