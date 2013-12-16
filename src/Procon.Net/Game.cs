using System;
using System.Collections.Generic;
using System.Linq;
using Procon.Net.Actions;
using Procon.Net.Actions.Deferred;
using Procon.Net.Models;
using Procon.Net.Protocols;

namespace Procon.Net {
    public abstract class Game : IGame {

        /// <summary>
        /// The client to handle all communications with the game server
        /// </summary>
        public IClient Client { get; protected set; }

        /// <summary>
        /// Manages actions and potential events to fire once an action has completed or expired.
        /// </summary>
        public IWaitingActions WaitingActions { get; set; }

        /// <summary>
        /// Everything the connection currently knows about the game. This is updated
        /// with all of the information we receive from the server.
        /// </summary>
        public GameState State { get; protected set; }

        /// <summary>
        /// The password used to authenticate with the server.
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// Describing attribute of this game.
        /// </summary>
        public virtual IGameType GameType {
            get {
                if (this._gameType == null) {
                    IGameType attribute = this.GetType().GetCustomAttributes(typeof(IGameType), false).Cast<IGameType>().FirstOrDefault();

                    if (attribute != null) {
                        this._gameType = new GameType(attribute);
                    }
                }

                return _gameType; 
            }
        }
        private IGameType _gameType;

        /// <summary>
        /// The base path to look for game configs.
        /// </summary>
        public String GameConfigPath { get; set; }

        [Obsolete]
        public string Additional { get; set; }
        
        /// <summary>
        /// Handles all packet dispatching.
        /// </summary>
        public IPacketDispatcher PacketDispatcher { get; set; }

        protected Game(string hostName, ushort port) : base() {
            this.Execute(hostName, port);
        }

        /// <summary>
        /// This should be moved from here
        /// </summary>
        [Obsolete]
        static Game() {
            // Load all supported game assemblies.
            SupportedGameTypes.GetSupportedGames();
        }

        protected virtual void Execute(string hostName, ushort port) {
            this.State = new GameState();
            this.Client = this.CreateClient(hostName, port);
            this.PacketDispatcher = this.CreatePacketDispatcher();

            // Handle client events, most of which are proxies to events we fire.
            this.Client.PacketReceived += (sender, wrapper) => {
                this.PacketDispatcher.Dispatch(wrapper);

                // Alert the deferrer that we have a new packet that's been dispatched
                this.WaitingActions.Mark(wrapper.Packet);

                this.OnClientEvent(ClientEventType.ClientPacketReceived, new ClientEventData() {
                    Packets = new List<IPacket>() {
                        wrapper.Packet
                    }
                });
            };

            this.Client.ConnectionStateChanged += (sender, state) => {
                this.OnClientEvent(ClientEventType.ClientConnectionStateChange);

                this.State.Settings.Current.ConnectionState = state;

                if (state == ConnectionState.ConnectionReady) {
                    this.Login(this.Password);
                }
            };

            this.Client.PacketSent += (sender, wrapper) => this.OnClientEvent(ClientEventType.ClientPacketSent, new ClientEventData() {
                Packets = new List<IPacket>() {
                    wrapper.Packet
                }
            });

            this.Client.SocketException += (sender, se) => this.OnClientEvent(ClientEventType.ClientSocketException, new ClientEventData() {
                Exceptions = new List<Exception>() {
                    se
                }
            });

            this.Client.ConnectionFailure += (sender, exception) => this.OnClientEvent(ClientEventType.ClientConnectionFailure, new ClientEventData() {
                Exceptions = new List<Exception>() {
                    exception
                }
            });

            this.WaitingActions = new WaitingActions() {
                Done = (action, requests, responses) => this.OnClientEvent(
                    ClientEventType.ClientActionDone,
                    new ClientEventData() {
                        Packets = responses
                    },
                    new ClientEventData() {
                        Actions = new List<NetworkAction>() {
                            action
                        },
                        Packets = requests
                    }
                ),
                Expired = (action, requests, responses) => this.OnClientEvent(
                    ClientEventType.ClientActionExpired,
                    new ClientEventData() {
                        Packets = responses
                    },
                    new ClientEventData() {
                        Actions = new List<NetworkAction>() {
                            action
                        },
                        Packets = requests
                    }
                ),
            };
        }

        /// <summary>
        /// Fired when ever a dispatched game event occurs.
        /// </summary>
        public virtual event GameEventHandler GameEvent;
        public delegate void GameEventHandler(Game sender, GameEventArgs e);

        protected void OnGameEvent(GameEventType eventType, GameEventData now = null, GameEventData then = null) {
            var handler = this.GameEvent;
            if (handler != null) {
                handler(
                    this,
                    new GameEventArgs() {
                        GameEventType = eventType,
                        GameType = this.GameType as GameType, // Required for serialization. How to get around?
                        GameState = this.State,
                        Now = now ?? new GameEventData(),
                        Then = then ?? new GameEventData()
                    }
                );
            }
        }

        /// <summary>
        /// Fired when something occurs with the underlying client. This can
        /// be connections, disconnections, logins or raw packets being recieved.
        /// </summary>
        public virtual event ClientEventHandler ClientEvent;
        public delegate void ClientEventHandler(Game sender, ClientEventArgs e);

        protected void OnClientEvent(ClientEventType eventType, ClientEventData now = null, ClientEventData then = null) {
            var handler = this.ClientEvent;
            if (handler != null) {
                handler(this, new ClientEventArgs() {
                    EventType = eventType,
                    ConnectionState = this.Client.ConnectionState,
                    Now = now ?? new ClientEventData(),
                    Then = then ?? new ClientEventData()
                });
            }
        }

        /// <summary>
        /// Initiates the login to the game server.
        /// </summary>
        /// <param name="password"></param>
        protected abstract void Login(string password);

        /// <summary>
        /// Process a generic network action
        /// </summary>
        /// <param name="action"></param>
        public virtual List<IPacket> Action(NetworkAction action) {
            List<IPacket> packets = null;
            List<IPacketWrapper> wrappers = null;

            if (action is Chat) {
                wrappers = this.Action(action as Chat);
            }
            else if (action is Kick) {
                wrappers = this.Action(action as Kick);
            }
            else if (action is Ban) {
                wrappers = this.Action(action as Ban);
            }
            else if (action is Map) {
                wrappers = this.Action(action as Map);
            }
            else if (action is Kill) {
                wrappers = this.Action(action as Kill);
            }
            else if (action is Move) {
                wrappers = this.Action(action as Move);
            }
            else if (action is Raw) {
                wrappers = this.Action(action as Raw);
            }

            if (wrappers != null) {
                // Fetch all of the packets that are not null
                packets = wrappers.Where(wrapper => wrapper != null).Select(wrapper => wrapper.Packet).ToList();

                // Defer this completed action for now.
                this.WaitingActions.Wait(action, packets);

                // Now send the packets
                foreach (IPacketWrapper wrapper in wrappers) {
                    this.Send(wrapper);
                }
            }

            return packets;
        }

        protected abstract List<IPacketWrapper> Action(Chat chat);

        protected abstract List<IPacketWrapper> Action(Kick kick);

        protected abstract List<IPacketWrapper> Action(Ban ban);

        protected abstract List<IPacketWrapper> Action(Map map);

        protected abstract List<IPacketWrapper> Action(Kill kill);

        protected abstract List<IPacketWrapper> Action(Move move);

        /// <summary>
        /// Send a raw packet to the sever, creating or wrapping it first.
        /// </summary>
        /// <param name="raw"></param>
        /// <returns></returns>
        protected virtual List<IPacketWrapper> Action(Raw raw) {
            List<IPacketWrapper> wrappers = new List<IPacketWrapper>();

            if (raw.ActionType == NetworkActionType.NetworkSend) {
                wrappers.AddRange(raw.Now.Content.Select(text => this.CreatePacket(text)));

                wrappers.AddRange(raw.Now.Packets.Select(this.WrapPacket));
            }

            return wrappers;
        }

        /// <summary>
        /// Sends a packet to the server, provided a client exists and the connection is open and ready or logged in.
        /// This allows for the login command to be sent to a ready connection, otherwise no login packets could be sent.
        /// </summary>
        /// <param name="wrapper"></param>
        public virtual IPacket Send(IPacketWrapper wrapper) {
            IPacket sent = null;

            if (wrapper != null) {
                if (this.Client != null && (this.Client.ConnectionState == ConnectionState.ConnectionReady || this.Client.ConnectionState == ConnectionState.ConnectionLoggedIn)) {
                    sent = this.Client.Send(wrapper);
                }
            }

            return sent;
        }

        /// <summary>
        /// Attempts a connection to the server.
        /// </summary>
        public void AttemptConnection() {
            if (this.Client != null && this.Client.ConnectionState == ConnectionState.ConnectionDisconnected) {
                this.Client.Connect();
            }
        }

        /// <summary>
        /// Shutsdown this connection
        /// </summary>
        public virtual void Shutdown() {
            if (this.Client != null) {
                this.Client.Shutdown();
            }
        }

        /// <summary>
        /// General timed event to synch everything on the server with what is known locally.
        /// </summary>
        public virtual void Synchronize() {
            if (this.Client != null && this.Client.ConnectionState != ConnectionState.ConnectionLoggedIn) {
                this.Client.Poke();
            }
        }

        /// <summary>
        /// Create the dispatcher to use.
        /// </summary>
        /// <returns></returns>
        protected virtual IPacketDispatcher CreatePacketDispatcher() {
            return new PacketDispatcher();
        }

        /// <summary>
        /// Creates an appropriate client for the game type
        /// </summary>
        /// <param name="hostName">The hostname of the server to connect to</param>
        /// <param name="port">The port on the server to connect to</param>
        /// <returns>A client capable of communicating with this game server</returns>
        protected abstract IClient CreateClient(string hostName, ushort port);

        /// <summary>
        /// Create a packet from a string
        /// </summary>
        /// <param name="format"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        protected abstract IPacketWrapper CreatePacket(String format, params object[] args);

        /// <summary>
        /// Wraps a completed packet in a packet wrapper, allowing it to be sent to the server.
        /// </summary>
        /// <param name="packet"></param>
        /// <returns></returns>
        protected abstract IPacketWrapper WrapPacket(IPacket packet);
    }
}
