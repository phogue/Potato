﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Procon.Core.Events;
using Procon.Core.Variables;

namespace Procon.Core.Connections {
    using Procon.Core.Connections.TextCommands;
    using Procon.Core.Connections.Plugins;
    using Procon.Net;
    using Procon.Net.Utils;
    using Procon.Net.Protocols.Objects;

    [Serializable]
    public class Connection : Executable {

        /// <summary>
        /// The unique hash for this connection. This simplifies identifying a connection to a single string that can be compared.
        /// </summary>
        public Guid ConnectionGuid { get; set; }

        public String ProtocolProvider {
            get { return this.Game != null ? this.Game.ProtocolProvider : String.Empty; }
            // ReSharper disable ValueParameterNotUsed
            set { }
            // ReSharper restore ValueParameterNotUsed
        }

        public String GameType {
            get { return this.Game != null ? this.Game.GameType : String.Empty; }
            // ReSharper disable ValueParameterNotUsed
            set { }
            // ReSharper restore ValueParameterNotUsed
        }

        public String GameName {
            get { return this.Game != null ? this.Game.GameName : String.Empty; }
            // ReSharper disable ValueParameterNotUsed
            set { }
            // ReSharper restore ValueParameterNotUsed
        }

        public String Hostname {
            get { return this.Game != null ? this.Game.Hostname : String.Empty; }
            // ReSharper disable ValueParameterNotUsed
            set { }
            // ReSharper restore ValueParameterNotUsed
        }

        public ushort Port {
            get { return this.Game != null ? this.Game.Port : (ushort)0; }
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
        public Game Game { get; set; }

        /// <summary>
        /// The instance of procon that owns this connection.
        /// </summary>
        [XmlIgnore, JsonIgnore]
        public Instance Instance { get; set; }

        [XmlIgnore, JsonIgnore]
        public GameState GameState {
            get { return this.Game != null ? this.Game.State : null; }
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

        protected override IList<IExecutableBase> BubbleExecutableObjects(Command command) {
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

        [CommandAttribute(CommandType = CommandType.ConnectionQuery)]
        public CommandResultArgs ConnectionQuery(Command command) {
            CommandResultArgs result = null;

            if (this.Security.DispatchPermissionsCheck(command, command.Name).Success == true) {
                CommandResultArgs players = this.Execute(new Command(command) {
                    CommandType = CommandType.NetworkProtocolQueryPlayers
                });

                CommandResultArgs settings = this.Execute(new Command(command) {
                    CommandType = CommandType.NetworkProtocolQuerySettings
                });

                CommandResultArgs bans = this.Execute(new Command(command) {
                    CommandType = CommandType.NetworkProtocolQueryBans
                });

                CommandResultArgs maps = this.Execute(new Command(command) {
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
                        Plugins = new List<Plugin>(this.Plugins.Plugins),
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

        [CommandAttribute(CommandType = CommandType.NetworkProtocolQueryPlayers)]
        public CommandResultArgs NetworkProtocolQueryPlayers(Command command) {
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
                        Players = new List<Player>(this.Game.State.PlayerList)
                    }
                };
            }
            else {
                result = CommandResultArgs.InsufficientPermissions;
            }

            return result;
        }

        [CommandAttribute(CommandType = CommandType.NetworkProtocolQuerySettings)]
        public CommandResultArgs NetworkProtocolQuerySettings(Command command) {
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

        [CommandAttribute(CommandType = CommandType.NetworkProtocolQueryBans)]
        public CommandResultArgs NetworkProtocolQueryBans(Command command) {
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
                        Bans = new List<Ban>(this.Game.State.BanList)
                    }
                };
            }
            else {
                result = CommandResultArgs.InsufficientPermissions;
            }

            return result;
        }

        [CommandAttribute(CommandType = CommandType.NetworkProtocolQueryMaps)]
        public CommandResultArgs NetworkProtocolQueryMaps(Command command) {
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
                        Maps = new List<Map>(this.Game.State.MapList)
                    }
                };
            }
            else {
                result = CommandResultArgs.InsufficientPermissions;
            }

            return result;
        }

        [CommandAttribute(CommandType = CommandType.NetworkProtocolQueryMapPool)]
        public CommandResultArgs NetworkProtocolQueryMapPool(Command command) {
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

        [CommandAttribute(CommandType = CommandType.NetworkProtocolActionChat)]
        public CommandResultArgs NetworkProtocolAction(Command command, Chat chat) {
            CommandResultArgs result = null;

            if (this.Security.DispatchPermissionsCheck(command, command.Name).Success == true) {
                this.Game.Action(chat);

                result = new CommandResultArgs() {
                    Success = true,
                    Status = CommandResultType.Success,
                    Now = new CommandData() {
                        Chats = new List<Chat>() {
                            chat
                        }
                    }
                };
            }
            else {
                result = CommandResultArgs.InsufficientPermissions;
            }

            return result;
        }

        [CommandAttribute(CommandType = CommandType.NetworkProtocolActionKill)]
        public CommandResultArgs NetworkProtocolAction(Command command, Kill kill) {
            CommandResultArgs result = null;
            
            if (this.Security.DispatchPermissionsCheck(command, command.Name).Success == true) {
                this.Game.Action(kill);

                result = new CommandResultArgs() {
                    Success = true,
                    Status = CommandResultType.Success,
                    Now = new CommandData() {
                        Kills = new List<Kill>() {
                            kill
                        }
                    }
                };
            }
            else {
                result = CommandResultArgs.InsufficientPermissions;
            }

            return result;
        }

        [CommandAttribute(CommandType = CommandType.NetworkProtocolActionMove)]
        public CommandResultArgs NetworkProtocolAction(Command command, Move move) {
            CommandResultArgs result = null;

            if (this.Security.DispatchPermissionsCheck(command, command.Name).Success == true) {
                this.Game.Action(move);

                result = new CommandResultArgs() {
                    Success = true,
                    Status = CommandResultType.Success,
                    Now = new CommandData() {
                        Moves = new List<Move>() {
                            move
                        }
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
                this.Events.Log(new GenericEventArgs() {
                    Name = e.EventType.ToString(),
                    Message = e.ConnectionError != null ? e.ConnectionError.Message : String.Empty,
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
                        this.Execute(new Command() {
                            GameType = this.GameType,
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