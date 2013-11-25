using System;
using System.Linq;
using Procon.Net.Protocols;

namespace Procon.Net {
    using Procon.Net.Attributes;
    using Procon.Net.Protocols.Objects;

    public abstract class Game : IGame {

        /// <summary>
        /// The client to handle all communications with the game server
        /// </summary>
        public IClient Client { get; protected set; }

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
        /// Who is providing the protocol implementation being used
        /// </summary>
        public String ProtocolProvider {
            get {
                if (String.IsNullOrEmpty(this._mProtocolProvider) == true) {
                    GameTypeAttribute[] attributes = this.GetType().GetCustomAttributes(typeof(GameTypeAttribute), false) as GameTypeAttribute[];

                    if (attributes != null && attributes.Any() == true) {
                        this._mProtocolProvider = attributes.First().Provider;
                    }
                }

                return this._mProtocolProvider;
            }
        }
        private String _mProtocolProvider = String.Empty;

        /// <summary>
        /// The game type of this implementation, used for serialization and such to identify the current game.
        /// </summary>
        public String GameType {
            get {
                if (this._mGameType == Protocols.CommonGameType.None) {
                    GameTypeAttribute[] attributes = this.GetType().GetCustomAttributes(typeof(GameTypeAttribute), false) as GameTypeAttribute[];

                    if (attributes != null && attributes.Any() == true) {
                        this._mGameType = attributes.First().Type;
                    }
                }

                return this._mGameType;
            }
        }
        private String _mGameType = CommonGameType.None;

        /// <summary>
        /// The game name of this implementation, used for serialization and such to identify the current game.
        /// </summary>
        public String GameName {
            get {
                if (String.IsNullOrEmpty(this._mGameName) == true) {
                    GameTypeAttribute[] attributes = this.GetType().GetCustomAttributes(typeof(GameTypeAttribute), false) as GameTypeAttribute[];

                    if (attributes != null && attributes.Any() == true) {
                        this._mGameName = attributes.First().Name;
                    }
                }

                return this._mGameName;
            }
        }
        private String _mGameName = String.Empty;

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
            this.Client.PacketReceived += (sender, packet) => {
                this.PacketDispatcher.Dispatch(packet);

                this.OnClientEvent(ClientEventType.ClientPacketReceived, packet);
            };

            this.Client.ConnectionStateChanged += (sender, state) => {
                this.OnClientEvent(ClientEventType.ClientConnectionStateChange);

                this.State.Settings.ConnectionState = state;

                if (state == ConnectionState.ConnectionReady) {
                    this.Login(this.Password);
                }
            };

            this.Client.PacketSent += (sender, packet) => this.OnClientEvent(ClientEventType.ClientPacketSent, packet);
            this.Client.SocketException += (sender, se) => this.OnClientEvent(ClientEventType.ClientSocketException, null, se);
            this.Client.ConnectionFailure += (sender, exception) => this.OnClientEvent(ClientEventType.ClientConnectionFailure, null, exception);
        }

        /// <summary>
        /// Fired when ever a dispatched game event occurs.
        /// </summary>
        public virtual event GameEventHandler GameEvent;
        public delegate void GameEventHandler(Game sender, GameEventArgs e);

        protected void OnGameEvent(GameEventType eventType, GameEventData after = null, GameEventData before = null) {
            var handler = this.GameEvent;
            if (handler != null) {
                handler(
                    this,
                    new GameEventArgs() {
                        GameEventType = eventType,
                        GameType = this.GameType,
                        GameState = this.State,
                        Then = before ?? new GameEventData(),
                        Now = after ?? new GameEventData(),
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

        protected void OnClientEvent(ClientEventType eventType, IPacketWrapper wrapper = null, Exception exception = null) {
            var handler = this.ClientEvent;
            if (handler != null) {
                handler(this, new ClientEventArgs() {
                    EventType = eventType,
                    ConnectionState = this.Client.ConnectionState,
                    ConnectionError = exception,
                    Packet = wrapper != null ? wrapper.Packet as Packet : null
                });
            }
        }

        /// <summary>
        /// Initiates the login to the game server.
        /// </summary>
        /// <param name="password"></param>
        protected virtual void Login(string password) {

        }

        /// <summary>
        /// Process a generic network action
        /// </summary>
        /// <param name="action"></param>
        public virtual void Action(NetworkAction action) {
            if (action is Chat) {
                this.Action(action as Chat);
            }
            else if (action is Kick) {
                this.Action(action as Kick);
            }
            else if (action is Ban) {
                this.Action(action as Ban);
            }
            else if (action is Map) {
                this.Action(action as Map);
            }
            else if (action is Kill) {
                this.Action(action as Kill);
            }
            else if (action is Move) {
                this.Action(action as Move);
            }
            else if (action is Raw) {
                this.Action(action as Raw);
            }
        }

        protected virtual void Action(Chat chat) {

        }

        protected virtual void Action(Kick kick) {

        }

        protected virtual void Action(Ban ban) {

        }

        protected virtual void Action(Map map) {

        }

        protected virtual void Action(Kill kill) {

        }

        protected virtual void Action(Move move) {

        }

        protected virtual void Action(Raw raw) {
            if (raw.ActionType == NetworkActionType.NetworkSend) {
                this.Send(this.CreatePacket(raw.PacketText));
            }
        }

        /// <summary>
        /// Sends a packet to the server, provided a client exists and the connection is open and ready or logged in.
        /// This allows for the login command to be sent to a ready connection, otherwise no login packets could be sent.
        /// </summary>
        /// <param name="wrapper"></param>
        public virtual void Send(IPacketWrapper wrapper) {
            if (wrapper != null) {
                if (this.Client != null && (this.Client.ConnectionState == ConnectionState.ConnectionReady || this.Client.ConnectionState == ConnectionState.ConnectionLoggedIn)) {
                    this.Client.Send(wrapper);
                }
            }
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
    }
}
