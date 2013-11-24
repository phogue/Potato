using System;

namespace Procon.Net {
    using Procon.Net.Protocols.Objects;

    public abstract class GameImplementation : Game {

        /// <summary>
        /// Handles all packet dispatching.
        /// </summary>
        public IPacketDispatcher PacketDispatcher { get; set; }

        protected GameImplementation(string hostName, ushort port) : base() {
            this.Execute(hostName, port);
        }

        protected void Execute(string hostName, ushort port) {
            this.State = new GameState();
            this.Client = this.CreateClient(hostName, port);
            this.PacketDispatcher = this.CreatePacketDispatcher();
            this.AssignEvents();
        }

        /// <summary>
        /// Creates an appropriate client for the game type
        /// </summary>
        /// <param name="hostName">The hostname of the server to connect to</param>
        /// <param name="port">The port on the server to connect to</param>
        /// <returns>A client capable of communicating with this game server</returns>
        protected abstract IClient CreateClient(string hostName, ushort port);

        /// <summary>
        /// Create the dispatcher to use.
        /// </summary>
        /// <returns></returns>
        protected virtual IPacketDispatcher CreatePacketDispatcher() {
            return new PacketDispatcher();
        }

        /// <summary>
        /// Sends a packet to the server, provided a client exists and the connection is open and ready or logged in.
        /// This allows for the login command to be sent to a ready connection, otherwise no login packets could be sent.
        /// </summary>
        /// <param name="packet"></param>
        public override void Send(Packet packet) {
            if (this.Client != null && (this.Client.ConnectionState == ConnectionState.ConnectionReady || this.Client.ConnectionState == ConnectionState.ConnectionLoggedIn)) {
                this.Client.Send(packet);
            }
        }

        public override void Synchronize() {
            if (this.Client != null && this.Client.ConnectionState != ConnectionState.ConnectionLoggedIn) {
                this.Client.Poke();
            }
        }

        protected virtual void AssignEvents() {
            this.Client.PacketReceived += new ClientBase.PacketDispatchHandler(Client_PacketReceived);
            this.Client.PacketSent += new ClientBase.PacketDispatchHandler(Client_PacketSent);
            this.Client.SocketException += new ClientBase.SocketExceptionHandler(Client_SocketException);
            this.Client.ConnectionStateChanged += new ClientBase.ConnectionStateChangedHandler(Client_ConnectionStateChanged);
            this.Client.ConnectionFailure += new ClientBase.FailureHandler(Client_ConnectionFailure);
        }
        
        private void Client_ConnectionFailure(IClient sender, Exception exception) {
            this.OnClientEvent(ClientEventType.ClientConnectionFailure, null, exception);
        }

        private void Client_ConnectionStateChanged(IClient sender, ConnectionState newState) {
            this.OnClientEvent(ClientEventType.ClientConnectionStateChange);

            this.State.Settings.ConnectionState = newState;

            if (newState == ConnectionState.ConnectionReady) {
                this.Login(this.Password);
            }
        }

        private void Client_SocketException(IClient sender, System.Net.Sockets.SocketException se) {
            this.OnClientEvent(ClientEventType.ClientSocketException, null, se);
        }

        private void Client_PacketSent(IClient sender, Packet packet) {
            this.OnClientEvent(ClientEventType.ClientPacketSent, packet);
        }

        private void Client_PacketReceived(IClient sender, Packet packet) {
            this.PacketDispatcher.Dispatch(packet);

            this.OnClientEvent(ClientEventType.ClientPacketReceived, packet);
        }

        public override void AttemptConnection() {
            if (this.Client != null && this.Client.ConnectionState == ConnectionState.ConnectionDisconnected) {
                this.Client.Connect();
            }
        }

        protected abstract Packet CreatePacket(String format, params object[] args);

        public override void Login(string password) {
            
        }

        public override void Action(NetworkAction action) {
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
        /// Executes a game config against this game implementation.
        /// </summary>
        /// <typeparam name="T">The type of config to execute.</typeparam>
        /// <param name="config"></param>
        protected virtual void ExecuteGameConfig<T>(T config) where T : GameConfig {
            if (config != null) {
                this.State.MapPool = config.MapPool;
                this.State.GameModePool = config.GameModePool;

                this.State.MapPool.ForEach(map => map.ActionType = NetworkActionType.NetworkMapPooled);

                this.OnGameEvent(GameEventType.GameConfigExecuted);
            }
        }

        /// <summary>
        /// Loads a game config
        /// </summary>
        /// <typeparam name="T">The type of config to load.</typeparam>
        /// <param name="protocolProvider"></param>
        /// <param name="protocolName"></param>
        protected virtual T LoadGameConfig<T>(String protocolProvider, String protocolName) where T : GameConfig {
            String configPath = GameConfig.BuildConfigPath(this.GameConfigPath, protocolProvider, protocolName);

            return GameConfig.LoadConfig<T>(configPath);
        }

        public override void Shutdown() {
            if (this.Client != null) {
                this.Client.Shutdown();
            }
        }
    }
}
