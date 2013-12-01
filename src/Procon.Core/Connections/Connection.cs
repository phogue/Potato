using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Procon.Core.Events;
using Procon.Core.Variables;
using Procon.Net.Actions;
using Procon.Net.Data;

namespace Procon.Core.Connections {
    using Procon.Core.Connections.TextCommands;
    using Procon.Core.Connections.Plugins;
    using Procon.Net;
    using Procon.Net.Utils;

    [Serializable]
    public class Connection : Executable {

        /// <summary>
        /// The unique hash for this connection. This simplifies identifying a connection to a single string that can be compared.
        /// </summary>
        public Guid ConnectionGuid { get; set; }

        public GameType GameType {
            get { return this.Game != null ? this.Game.GameType as GameType : null; }
            // ReSharper disable ValueParameterNotUsed
            set { }
            // ReSharper restore ValueParameterNotUsed
        }

        public String Hostname {
            get { return this.Game != null ? this.Game.Client.Hostname : String.Empty; }
            // ReSharper disable ValueParameterNotUsed
            set { }
            // ReSharper restore ValueParameterNotUsed
        }

        public ushort Port {
            get { return this.Game != null ? this.Game.Client.Port : (ushort)0; }
            // ReSharper disable ValueParameterNotUsed
            set { }
            // ReSharper restore ValueParameterNotUsed
        }

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
        public PluginController Plugins { get; set; }

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

        public Connection() : base() {

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
            Config pluginConfig = new Config().Create(typeof(PluginController));
            this.Plugins.WriteConfig(pluginConfig);
            config.Combine(pluginConfig);
        }

        public override ExecutableBase Execute() {
            this.ConnectionGuid = MD5.Guid(String.Format("{0}:{1}:{2}", this.GameType, this.Hostname, this.Port));

            this.AssignEvents();

            this.TextCommands = new TextCommandController() {
                Connection = this
            };

            this.Plugins = new PluginController() {
                Connection = this
            };

            this.TextCommands.Execute();
            this.Plugins.Execute();
            
            // Set the default ignore list.
            this.Variables.Set(new Command() {
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
                this.Game.ClientEvent += new Net.Game.ClientEventHandler(Game_ClientEvent);
                this.Game.GameEvent += new Net.Game.GameEventHandler(Game_GameEvent);
            }
        }

        /// <summary>
        /// Removes all current event handlers.
        /// </summary>
        protected void UnassignEvents() {
            if (this.Game != null) {
                this.Game.ClientEvent -= new Net.Game.ClientEventHandler(Game_ClientEvent);
                this.Game.GameEvent -= new Net.Game.GameEventHandler(Game_GameEvent);
            }
        }

        public override CommandResultArgs PropogatePreview(Command command, bool tunnel = true) {
            if (tunnel == false && command.Scope != null && command.Scope.ConnectionGuid == this.ConnectionGuid) {
                // We've bubbled up far enough, time to tunnel down this connection to find our result.
                return this.Tunnel(command);
            }

            return base.PropogatePreview(command, tunnel);
        }

        public override CommandResultArgs PropogateHandler(Command command, bool tunnel = true) {
            if (tunnel == false && command.Scope != null && command.Scope.ConnectionGuid == this.ConnectionGuid) {
                // We've bubbled up far enough, time to tunnel down this connection to find our result.
                return this.Tunnel(command);
            }

            return base.PropogateHandler(command, tunnel);
        }

        public override CommandResultArgs PropogateExecuted(Command command, bool tunnel = true) {
            if (tunnel == false && command.Scope != null && command.Scope.ConnectionGuid == this.ConnectionGuid) {
                // We've bubbled up far enough, time to tunnel down this connection to find our result.
                return this.Tunnel(command);
            }

            return base.PropogateExecuted(command, tunnel);
        }

        protected override IList<IExecutableBase> BubbleExecutableObjects(Command command) {
            return this.Instance != null ? new List<IExecutableBase>() {
                this.Instance
            } : new List<IExecutableBase>();
        }

        protected override IList<IExecutableBase> TunnelExecutableObjects(Command command) {
            return new List<IExecutableBase> {
                this.TextCommands,
                this.Plugins
            };
        }

        // Attempts to begin communication with the game server.
        public void AttemptConnection() {
            if (this.Game != null) {
                this.Game.AttemptConnection();
            }
        }

        public CommandResultArgs ConnectionQuery(Command command, Dictionary<String, CommandParameter> parameters) {
            CommandResultArgs result = null;

            if (this.Security.DispatchPermissionsCheck(command, command.Name).Success == true) {
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
                        Connections = new List<Connection>() {
                            this
                        }
                    },
                    Now = new CommandData() {
                        // I didn't want plugins to be able to hide themselves.
                        Plugins = new List<HostPlugin>(this.Plugins.Plugins),
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

            if (this.Security.DispatchPermissionsCheck(command, command.Name).Success == true) {
                result = new CommandResultArgs() {
                    Success = true,
                    Status = CommandResultType.Success,
                    Scope = new CommandData() {
                        Connections = new List<Connection>() {
                            this
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

            if (this.Security.DispatchPermissionsCheck(command, command.Name).Success == true) {
                result = new CommandResultArgs() {
                    Success = true,
                    Status = CommandResultType.Success,
                    Scope = new CommandData() {
                        Connections = new List<Connection>() {
                            this
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

            if (this.Security.DispatchPermissionsCheck(command, command.Name).Success == true) {
                result = new CommandResultArgs() {
                    Success = true,
                    Status = CommandResultType.Success,
                    Scope = new CommandData() {
                        Connections = new List<Connection>() {
                            this
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

            if (this.Security.DispatchPermissionsCheck(command, command.Name).Success == true) {
                result = new CommandResultArgs() {
                    Success = true,
                    Status = CommandResultType.Success,
                    Scope = new CommandData() {
                        Connections = new List<Connection>() {
                            this
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

            if (this.Security.DispatchPermissionsCheck(command, command.Name).Success == true) {
                result = new CommandResultArgs() {
                    Success = true,
                    Status = CommandResultType.Success,
                    Scope = new CommandData() {
                        Connections = new List<Connection>() {
                            this
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

            if (this.Security.DispatchPermissionsCheck(command, command.Name).Success == true) {
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

            if (this.Security.DispatchPermissionsCheck(command, command.Name).Success == true) {
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
            
            if (this.Security.DispatchPermissionsCheck(command, command.Name).Success == true) {
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

            if (this.Security.DispatchPermissionsCheck(command, command.Name).Success == true) {
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

            if (this.Security.DispatchPermissionsCheck(command, command.Name).Success == true) {
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

        private void Game_ClientEvent(Game sender, ClientEventArgs e) {
            if (e.EventType == ClientEventType.ClientConnectionStateChange) {

                // Once connected, sync the connection.
                if (e.ConnectionState == ConnectionState.ConnectionLoggedIn) {
                    this.Game.Synchronize();
                }

                this.Events.Log(new GenericEventArgs() {
                    Name = e.ConnectionState.ToString(),
                    Scope = new CommandData() {
                        Connections = new List<Connection>() {
                            this
                        }
                    },
                    Stamp = e.Stamp
                });
            }
            else if (e.EventType == ClientEventType.ClientConnectionFailure || e.EventType == ClientEventType.ClientSocketException) {
                Exception exception = e.Now.Exceptions.FirstOrDefault();

                this.Events.Log(new GenericEventArgs() {
                    Name = e.EventType.ToString(),
                    Message = exception != null ? exception.Message : String.Empty,
                    Scope = new CommandData() {
                        Connections = new List<Connection>() {
                            this
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
        private void Game_GameEvent(Game sender, GameEventArgs e) {
            if (this.Variables.Get<List<String>>(CommonVariableNames.GameEventsIgnoreList).Contains(e.GameEventType.ToString()) == false) {
                this.Events.Log(new GenericEventArgs() {
                    Name = e.GameEventType.ToString(),
                    Then = GenericEventData.Parse(e.Then),
                    Now = GenericEventData.Parse(e.Now),
                    Scope = new CommandData() {
                        Connections = new List<Connection>() {
                            this
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
                            GameType = this.GameType.Type,
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

            this.GameType = null;
            this.Hostname = null;
            this.Port = 0;

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
