using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Serialization;
using Newtonsoft.Json;
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

        public ConnectionModel ConnectionModel { get; set; }

        [XmlIgnore, JsonIgnore]
        public String Password {
            get { return this.Game != null ? this.Game.Password : String.Empty; }
        }

        [Obsolete]
        public String Additional {
            get { return this.Game != null ? this.Game.Additional : String.Empty; }
        }

        /// <summary>
        /// The controller to load up and manage plugins
        /// </summary>
        [XmlIgnore, JsonIgnore]
        public CorePluginController Plugins { get; set; }

        /// <summary>
        /// Text command controller to pipe all text chats through for analysis of text commands.
        /// </summary>
        [XmlIgnore, JsonIgnore]
        public TextCommandController TextCommands { get; protected set; }

        /// <summary>
        ///  The actual game object
        /// </summary>
        [XmlIgnore, JsonIgnore]
        public IGame Game { get; set; }

        /// <summary>
        /// The instance of procon that owns this connection.
        /// </summary>
        [XmlIgnore, JsonIgnore]
        public Instance Instance { get; set; }

        [XmlIgnore, JsonIgnore]
        public GameState GameState {
            get { return this.Game != null ? this.Game.State : null; }
        }

        [XmlIgnore, JsonIgnore]
        public SharedReferences Shared { get; private set; }

        public ConnectionController() : base() {
            this.Shared = new SharedReferences();

            this.ConnectionModel = new ConnectionModel();

            this.AppendDispatchHandlers(new Dictionary<CommandAttribute, CommandDispatchHandler>() {
                {
                    new CommandAttribute() {
                        CommandType = CommandType.ConnectionQuery
                    },
                    new CommandDispatchHandler(this.ConnectionQuery)
                }, {
                    new CommandAttribute() {
                        CommandType = CommandType.NetworkProtocolQueryPlayers
                    },
                    new CommandDispatchHandler(this.NetworkProtocolQueryPlayers)
                }, {
                    new CommandAttribute() {
                        CommandType = CommandType.NetworkProtocolQuerySettings
                    },
                    new CommandDispatchHandler(this.NetworkProtocolQuerySettings)
                }, {
                    new CommandAttribute() {
                        CommandType = CommandType.NetworkProtocolQueryBans
                    },
                    new CommandDispatchHandler(this.NetworkProtocolQueryBans)
                }, {
                    new CommandAttribute() {
                        CommandType = CommandType.NetworkProtocolQueryMaps
                    },
                    new CommandDispatchHandler(this.NetworkProtocolQueryMaps)
                }, {
                    new CommandAttribute() {
                        CommandType = CommandType.NetworkProtocolQueryMapPool
                    },
                    new CommandDispatchHandler(this.NetworkProtocolQueryMapPool)
                }, {
                    new CommandAttribute() {
                        CommandType = CommandType.NetworkProtocolActionRaw,
                        ParameterTypes = new List<CommandParameterType>() {
                            new CommandParameterType() {
                                Name = "raw",
                                Type = typeof(Raw)
                            }
                        }
                    },
                    new CommandDispatchHandler(this.NetworkProtocolActionRaw)
                }, {
                    new CommandAttribute() {
                        CommandType = CommandType.NetworkProtocolActionChat,
                        ParameterTypes = new List<CommandParameterType>() {
                            new CommandParameterType() {
                                Name = "chat",
                                Type = typeof(Chat)
                            }
                        }
                    },
                    new CommandDispatchHandler(this.NetworkProtocolActionChat)
                }, {
                    new CommandAttribute() {
                        CommandType = CommandType.NetworkProtocolActionKill,
                        ParameterTypes = new List<CommandParameterType>() {
                            new CommandParameterType() {
                                Name = "kill",
                                Type = typeof(Kill)
                            }
                        }
                    },
                    new CommandDispatchHandler(this.NetworkProtocolActionKill)
                }, {
                    new CommandAttribute() {
                        CommandType = CommandType.NetworkProtocolActionMove,
                        ParameterTypes = new List<CommandParameterType>() {
                            new CommandParameterType() {
                                Name = "move",
                                Type = typeof(Move)
                            }
                        }
                    },
                    new CommandDispatchHandler(this.NetworkProtocolActionMove)
                }, {
                    new CommandAttribute() {
                        CommandType = CommandType.NetworkProtocolActionKick,
                        ParameterTypes = new List<CommandParameterType>() {
                            new CommandParameterType() {
                                Name = "kick",
                                Type = typeof(Kick)
                            }
                        }
                    },
                    new CommandDispatchHandler(this.NetworkProtocolActionKick)
                }
            });
        }

        public override void WriteConfig(Config config) {
            this.Plugins.WriteConfig(config);
        }

        public override ICoreController Execute() {
            if (this.Game != null) {
                this.ConnectionModel.GameType = this.Game.GameType as GameType;
                this.ConnectionModel.Hostname = this.Game.Client.Hostname;
                this.ConnectionModel.Port = this.Game.Client.Port;
            }

            this.ConnectionModel.ConnectionGuid = MD5.Guid(String.Format("{0}:{1}:{2}", this.ConnectionModel.GameType, this.ConnectionModel.Hostname, this.ConnectionModel.Port));

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
            }, CommonVariableNames.GameEventsIgnoreList, new List<String>() {
                GameEventType.GameBanlistUpdated.ToString(), 
                //GameEventType.GamePlayerlistUpdated.ToString(), 
                //GameEventType.GameSettingsUpdated.ToString(), 
                GameEventType.GameMaplistUpdated.ToString(), 
            });

            return base.Execute();
        }

        /// <summary>
        /// Assign all current event handlers.
        /// </summary>
        protected void AssignEvents() {
            this.UnassignEvents();

            if (this.Game != null) {
                this.Game.ClientEvent += Game_ClientEvent;
                this.Game.GameEvent += Game_GameEvent;
            }
        }

        /// <summary>
        /// Removes all current event handlers.
        /// </summary>
        protected void UnassignEvents() {
            if (this.Game != null) {
                this.Game.ClientEvent -= Game_ClientEvent;
                this.Game.GameEvent -= Game_GameEvent;
            }
        }

        public override CommandResultArgs PropogatePreview(Command command, CommandDirection direction) {
            if (direction == CommandDirection.Bubble && command.Scope != null && command.Scope.ConnectionGuid == this.ConnectionModel.ConnectionGuid) {
                // We've bubbled up far enough, time to tunnel down this connection to find our result.
                return base.PropogatePreview(command, CommandDirection.Tunnel);
            }

            return base.PropogatePreview(command, direction);
        }

        public override CommandResultArgs PropogateHandler(Command command, CommandDirection direction) {
            if (direction == CommandDirection.Bubble && command.Scope != null && command.Scope.ConnectionGuid == this.ConnectionModel.ConnectionGuid) {
                // We've bubbled up far enough, time to tunnel down this connection to find our result.
                return base.PropogateHandler(command, CommandDirection.Tunnel);
            }

            return base.PropogateHandler(command, direction);
        }

        public override CommandResultArgs PropogateExecuted(Command command, CommandDirection direction) {
            if (direction == CommandDirection.Bubble && command.Scope != null && command.Scope.ConnectionGuid == this.ConnectionModel.ConnectionGuid) {
                // We've bubbled up far enough, time to tunnel down this connection to find our result.
                return base.PropogateExecuted(command, CommandDirection.Tunnel);
            }

            return base.PropogateExecuted(command, direction);
        }

        /// <summary>
        /// Attempts communication with the game server.
        /// </summary>
        public void AttemptConnection() {
            if (this.Game != null) {
                this.Game.AttemptConnection();
            }
        }

        public CommandResultArgs ConnectionQuery(Command command, Dictionary<String, CommandParameter> parameters) {
            CommandResultArgs result = null;

            if (this.Shared.Security.DispatchPermissionsCheck(command, command.Name).Success == true) {
                CommandResultArgs players = this.Tunnel(new Command(command) {
                    CommandType = CommandType.NetworkProtocolQueryPlayers
                });

                CommandResultArgs settings = this.Tunnel(new Command(command) {
                    CommandType = CommandType.NetworkProtocolQuerySettings
                });

                CommandResultArgs bans = this.Tunnel(new Command(command) {
                    CommandType = CommandType.NetworkProtocolQueryBans
                });

                CommandResultArgs maps = this.Tunnel(new Command(command) {
                    CommandType = CommandType.NetworkProtocolQueryMaps
                });

                result = new CommandResultArgs() {
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
                result = CommandResultArgs.InsufficientPermissions;
            }

            return result;
        }

        public CommandResultArgs NetworkProtocolQueryPlayers(Command command, Dictionary<String, CommandParameter> parameters) {
            CommandResultArgs result = null;

            if (this.Shared.Security.DispatchPermissionsCheck(command, command.Name).Success == true) {
                result = new CommandResultArgs() {
                    Success = true,
                    Status = CommandResultType.Success,
                    Scope = new CommandData() {
                        Connections = new List<ConnectionModel>() {
                            this.ConnectionModel
                        }
                    },
                    Now = new CommandData() {
                        Players = new List<Player>(this.Game.State.Players)
                    }
                };
            }
            else {
                result = CommandResultArgs.InsufficientPermissions;
            }

            return result;
        }

        public CommandResultArgs NetworkProtocolQuerySettings(Command command, Dictionary<String, CommandParameter> parameters) {
            CommandResultArgs result = null;

            if (this.Shared.Security.DispatchPermissionsCheck(command, command.Name).Success == true) {
                result = new CommandResultArgs() {
                    Success = true,
                    Status = CommandResultType.Success,
                    Scope = new CommandData() {
                        Connections = new List<ConnectionModel>() {
                            this.ConnectionModel
                        }
                    },
                    Now = new CommandData() {
                        Settings = new List<Settings>() {
                            this.Game.State.Settings
                        }
                    }
                };
            }
            else {
                result = CommandResultArgs.InsufficientPermissions;
            }

            return result;
        }

        public CommandResultArgs NetworkProtocolQueryBans(Command command, Dictionary<String, CommandParameter> parameters) {
            CommandResultArgs result = null;

            if (this.Shared.Security.DispatchPermissionsCheck(command, command.Name).Success == true) {
                result = new CommandResultArgs() {
                    Success = true,
                    Status = CommandResultType.Success,
                    Scope = new CommandData() {
                        Connections = new List<ConnectionModel>() {
                            this.ConnectionModel
                        }
                    },
                    Now = new CommandData() {
                        Bans = new List<Ban>(this.Game.State.Bans)
                    }
                };
            }
            else {
                result = CommandResultArgs.InsufficientPermissions;
            }

            return result;
        }

        public CommandResultArgs NetworkProtocolQueryMaps(Command command, Dictionary<String, CommandParameter> parameters) {
            CommandResultArgs result = null;

            if (this.Shared.Security.DispatchPermissionsCheck(command, command.Name).Success == true) {
                result = new CommandResultArgs() {
                    Success = true,
                    Status = CommandResultType.Success,
                    Scope = new CommandData() {
                        Connections = new List<ConnectionModel>() {
                            this.ConnectionModel
                        }
                    },
                    Now = new CommandData() {
                        Maps = new List<Map>(this.Game.State.Maps)
                    }
                };
            }
            else {
                result = CommandResultArgs.InsufficientPermissions;
            }

            return result;
        }

        public CommandResultArgs NetworkProtocolQueryMapPool(Command command, Dictionary<String, CommandParameter> parameters) {
            CommandResultArgs result = null;

            if (this.Shared.Security.DispatchPermissionsCheck(command, command.Name).Success == true) {
                result = new CommandResultArgs() {
                    Success = true,
                    Status = CommandResultType.Success,
                    Scope = new CommandData() {
                        Connections = new List<ConnectionModel>() {
                            this.ConnectionModel
                        }
                    },
                    Now = new CommandData() {
                        Maps = new List<Map>(this.Game.State.MapPool)
                    }
                };
            }
            else {
                result = CommandResultArgs.InsufficientPermissions;
            }

            return result;
        }

        public CommandResultArgs NetworkProtocolActionRaw(Command command, Dictionary<String, CommandParameter> parameters) {
            CommandResultArgs result = null;

            Raw raw = parameters["raw"].First<Raw>();

            if (this.Shared.Security.DispatchPermissionsCheck(command, command.Name).Success == true) {
                result = new CommandResultArgs() {
                    Success = true,
                    Status = CommandResultType.Success,
                    Now = new CommandData() {
                        Packets = this.Game.Action(raw)
                    }
                };
            }
            else {
                result = CommandResultArgs.InsufficientPermissions;
            }

            return result;
        }

        public CommandResultArgs NetworkProtocolActionChat(Command command, Dictionary<String, CommandParameter> parameters) {
            CommandResultArgs result = null;

            Chat chat = parameters["chat"].First<Chat>();

            if (this.Shared.Security.DispatchPermissionsCheck(command, command.Name).Success == true) {
                result = new CommandResultArgs() {
                    Success = true,
                    Status = CommandResultType.Success,
                    Now = new CommandData() {
                        Chats = new List<Chat>() {
                            chat
                        },
                        Packets = this.Game.Action(chat)
                    }
                };
            }
            else {
                result = CommandResultArgs.InsufficientPermissions;
            }

            return result;
        }

        public CommandResultArgs NetworkProtocolActionKill(Command command, Dictionary<String, CommandParameter> parameters) {
            CommandResultArgs result = null;

            Kill kill = parameters["kill"].First<Kill>();
            
            if (this.Shared.Security.DispatchPermissionsCheck(command, command.Name).Success == true) {
                result = new CommandResultArgs() {
                    Success = true,
                    Status = CommandResultType.Success,
                    Now = new CommandData() {
                        Kills = new List<Kill>() {
                            kill
                        },
                        Packets = this.Game.Action(kill)
                    }
                };
            }
            else {
                result = CommandResultArgs.InsufficientPermissions;
            }

            return result;
        }

        public CommandResultArgs NetworkProtocolActionMove(Command command, Dictionary<String, CommandParameter> parameters) {
            CommandResultArgs result = null;

            Move move = parameters["move"].First<Move>();

            if (this.Shared.Security.DispatchPermissionsCheck(command, command.Name).Success == true) {
                result = new CommandResultArgs() {
                    Success = true,
                    Status = CommandResultType.Success,
                    Now = new CommandData() {
                        Moves = new List<Move>() {
                            move
                        },
                        Packets = this.Game.Action(move)
                    }
                };
            }
            else {
                result = CommandResultArgs.InsufficientPermissions;
            }

            return result;
        }

        public CommandResultArgs NetworkProtocolActionKick(Command command, Dictionary<String, CommandParameter> parameters) {
            CommandResultArgs result = null;

            Kick kick = parameters["kick"].First<Kick>();

            if (this.Shared.Security.DispatchPermissionsCheck(command, command.Name).Success == true) {
                result = new CommandResultArgs() {
                    Success = true,
                    Status = CommandResultType.Success,
                    Now = new CommandData() {
                        Kicks = new List<Kick>() {
                            kick
                        },
                        Packets = this.Game.Action(kick)
                    }
                };
            }
            else {
                result = CommandResultArgs.InsufficientPermissions;
            }

            return result;
        }

        private void Game_ClientEvent(IGame sender, ClientEventArgs e) {
            if (e.EventType == ClientEventType.ClientConnectionStateChange) {
                if (this.Game != null) {
                    this.ConnectionModel.GameType = this.Game.GameType as GameType;
                    this.ConnectionModel.Hostname = this.Game.Client.Hostname;
                    this.ConnectionModel.Port = this.Game.Client.Port;
                }

                // Once connected, sync the connection.
                if (e.ConnectionState == ConnectionState.ConnectionLoggedIn) {
                    this.Game.Synchronize();
                }

                this.Shared.Events.Log(new GenericEventArgs() {
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
                Exception exception = e.Now.Exceptions.FirstOrDefault();

                this.Shared.Events.Log(new GenericEventArgs() {
                    Name = e.EventType.ToString(),
                    Message = exception != null ? exception.Message : String.Empty,
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
        private void Game_GameEvent(IGame sender, GameEventArgs e) {
            if (this.Shared.Variables.Get<List<String>>(CommonVariableNames.GameEventsIgnoreList).Contains(e.GameEventType.ToString()) == false) {
                this.Shared.Events.Log(new GenericEventArgs() {
                    Name = e.GameEventType.ToString(),
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

            if (e.GameEventType == GameEventType.GameChat) {
                Chat chat = e.Now.Chats.First();

                // At least has the first prefix character 
                // and a little something-something to pass to
                // the parser.
                if (chat.Now.Content != null && chat.Now.Content.Count > 0 && chat.Now.Content.First().Length >= 2) {
                    String prefix = chat.Now.Content.First().First().ToString(CultureInfo.InvariantCulture);
                    String text = chat.Now.Content.First().Remove(0, 1);

                    if ((prefix = this.TextCommands.GetValidTextCommandPrefix(prefix)) != null) {
                        this.Tunnel(new Command() {
                            GameType = this.ConnectionModel.GameType.Type,
                            Origin = CommandOrigin.Plugin,
                            Uid = chat.Now.Players.First().Uid,
                            CommandType = CommandType.TextCommandsExecute,
                            Parameters = new List<CommandParameter>() {
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

            this.ConnectionModel.GameType = null;
            this.ConnectionModel.Hostname = null;
            this.ConnectionModel.Port = 0;

            // Now shutdown and null out the game. Note that we want to capture and report
            // events during the shutdown, but then we want to unassign events to the game
            // object before we null it out. We only null it so we dont suppress errors.
            this.Game.Shutdown();

            this.UnassignEvents();

            // this.Game.Dispose();
            this.Game = null;

            this.TextCommands.Dispose();

            this.Plugins.Dispose();

            base.Dispose();
        }
    }
}
