using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Procon.Core.Connections.Plugins;
using Procon.Core.Connections.TextCommands;
using Procon.Core.Shared;
using Procon.Core.Shared.Events;
using Procon.Core.Shared.Models;
using Procon.Net.Shared;
using Procon.Net.Shared.Actions;
using Procon.Net.Shared.Models;
using Procon.Net.Shared.Utils;

namespace Procon.Core.Connections {
    /// <summary>
    /// Handles connections, plugins and text commands for a single game server.
    /// </summary>
    [Serializable]
    public class ConnectionController : CoreController, ISharedReferenceAccess {
        /// <summary>
        /// Data about the protocol connection
        /// </summary>
        public ConnectionModel ConnectionModel { get; set; }

        /// <summary>
        /// The controller to load up and manage plugins
        /// </summary>
        public CorePluginController Plugins { get; set; }

        /// <summary>
        /// Text command controller to pipe all text chats through for analysis of text commands.
        /// </summary>
        public TextCommandController TextCommands { get; protected set; }

        /// <summary>
        ///  The actual game object
        /// </summary>
        public IProtocol Protocol { get; set; }

        /// <summary>
        /// The instance of procon that owns this connection.
        /// </summary>
        public InstanceController Instance { get; set; }

        /// <summary>
        /// Proxy to the active protocol state
        /// </summary>
        public IProtocolState ProtocolState {
            get { return this.Protocol != null ? this.Protocol.State : null; }
        }

        public SharedReferences Shared { get; private set; }

        /// <summary>
        /// Initializes the connection controller with default values, setting up command dispatches
        /// </summary>
        public ConnectionController() : base() {
            this.Shared = new SharedReferences();

            this.ConnectionModel = new ConnectionModel();

            this.CommandDispatchers.AddRange(new List<CommandDispatch>() {
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

        public override void WriteConfig(IConfig config) {
            this.Plugins.WriteConfig(config);
        }

        public override ICoreController Execute() {
            if (this.Protocol != null) {
                this.ConnectionModel.ProtocolType = this.Protocol.ProtocolType as ProtocolType;
                this.ConnectionModel.Hostname = this.Protocol.Client.Hostname;
                this.ConnectionModel.Port = this.Protocol.Client.Port;
                this.ConnectionModel.Password = this.Protocol.Password;
                this.ConnectionModel.Additional = this.Protocol.Additional;
            }

            this.ConnectionModel.ConnectionGuid = MD5.Guid(String.Format("{0}:{1}:{2}", this.ConnectionModel.ProtocolType, this.ConnectionModel.Hostname, this.ConnectionModel.Port));

            this.AssignEvents();

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

        /// <summary>
        /// Assign all current event handlers.
        /// </summary>
        protected void AssignEvents() {
            this.UnassignEvents();

            if (this.Protocol != null) {
                this.Protocol.ClientEvent += Protocol_ClientEvent;
                this.Protocol.ProtocolEvent += Protocol_ProtocolEvent;
            }
        }

        /// <summary>
        /// Removes all current event handlers.
        /// </summary>
        protected void UnassignEvents() {
            if (this.Protocol != null) {
                this.Protocol.ClientEvent -= Protocol_ClientEvent;
                this.Protocol.ProtocolEvent -= Protocol_ProtocolEvent;
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

                ICommandResult bans = this.Tunnel(new Command(command) {
                    CommandType = CommandType.NetworkProtocolQueryBans
                });

                ICommandResult maps = this.Tunnel(new Command(command) {
                    CommandType = CommandType.NetworkProtocolQueryMaps
                });

                result = new CommandResult() {
                    Success = true,
                    Status = CommandResultType.Success,
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
                        Bans = bans.Now.Bans,
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
                    Status = CommandResultType.Success,
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
                    Status = CommandResultType.Success,
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
                    Status = CommandResultType.Success,
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
                    Status = CommandResultType.Success,
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
                    Status = CommandResultType.Success,
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
                    Status = CommandResultType.Success,
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
                    Status = CommandResultType.Success,
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

        private void Protocol_ClientEvent(IProtocol sender, IClientEventArgs e) {
            if (e.EventType == ClientEventType.ClientConnectionStateChange) {
                if (this.Protocol != null) {
                    this.ConnectionModel.ProtocolType = this.Protocol.ProtocolType as ProtocolType;
                    this.ConnectionModel.Hostname = this.Protocol.Client.Hostname;
                    this.ConnectionModel.Port = this.Protocol.Client.Port;

                    // Once connected, sync the connection.
                    if (e.ConnectionState == ConnectionState.ConnectionLoggedIn) {
                        this.Protocol.Synchronize();
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
        }

        /// <summary>
        /// This function is pretty much a plugin on it's own that only dispatches events from the game.
        /// 
        /// I might put this into it's own class at some point to seperate the GameConnection from an action Procon is taking.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Protocol_ProtocolEvent(IProtocol sender, IProtocolEventArgs e) {
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

                    if ((prefix = this.TextCommands.GetValidTextCommandPrefix(prefix)) != null) {
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
        }

        public override void Dispose() {

            this.ConnectionModel.ProtocolType = null;
            this.ConnectionModel.Hostname = null;
            this.ConnectionModel.Port = 0;

            // Now shutdown and null out the game. Note that we want to capture and report
            // events during the shutdown, but then we want to unassign events to the game
            // object before we null it out. We only null it so we dont suppress errors.
            this.Protocol.Shutdown();

            this.UnassignEvents();

            // this.Game.Dispose();
            this.Protocol = null;

            this.TextCommands.Dispose();

            this.Plugins.Dispose();

            base.Dispose();
        }
    }
}
