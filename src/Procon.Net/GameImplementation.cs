using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;

namespace Procon.Net {
    using Procon.Net.Attributes;
    using Procon.Net.Protocols.Objects;

    public abstract class GameImplementation<P> : Game where P : Packet {

        /// <summary>
        /// The client to handle all communications with the game server
        /// </summary>
        public Client<P> Client { get; protected set; }

        /// <summary>
        /// Array of dispatch handlers used to locate an appropriate method to call
        /// once we receieve a packet.
        /// </summary>
        protected Dictionary<DispatchPacketAttribute, MethodInfo> DispatchHandlers;

        /// <summary>
        /// Fetches the hostname of the connection. Proxy for Client.Hostname.
        /// </summary>
        [Obsolete]
        public override string Hostname {
            get { return this.Client != null ? this.Client.Hostname : String.Empty; }
        }

        [Obsolete]
        public override ushort Port {
            get { return this.Client != null ? this.Client.Port : (ushort)0; }
        }

        [Obsolete]
        public override ConnectionState ConnectionState {
            get { return this.Client != null ? this.Client.ConnectionState : ConnectionState.ConnectionDisconnected; }
        }

        protected GameImplementation(string hostName, ushort port) : base() {
            this.Execute(this.CreateClient(hostName, port));
        }
  
        protected void Execute(Client<P> client) {
            this.State = new GameState();
            this.Client = client;
            this.AssignEvents();

            this.DispatchHandlers = new Dictionary<DispatchPacketAttribute, MethodInfo>();

            foreach (MethodInfo method in this.GetType().GetMethods()) {

                ParameterInfo[] parameters = method.GetParameters();

                if (parameters.Length == 2 && typeof(P).IsAssignableFrom(parameters[0].ParameterType) && typeof(P).IsAssignableFrom(parameters[1].ParameterType)) {

                    object[] attributes = method.GetCustomAttributes(typeof(DispatchPacketAttribute), false);

                    foreach (DispatchPacketAttribute attribute in attributes) {
                        if (this.DispatchHandlers.ContainsKey(attribute) == false) {
                            this.DispatchHandlers.Add(attribute, method);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Creates an appropriate client for the game type
        /// </summary>
        /// <param name="hostName">The hostname of the server to connect to</param>
        /// <param name="port">The port on the server to connect to</param>
        /// <returns>A client capable of communicating with this game server</returns>
        protected abstract Client<P> CreateClient(string hostName, ushort port);

        /// <summary>
        /// Dispatches a recieved packet. Each game implementation needs to supply its own dispatch
        /// method as the protocol may be very different and have additional requirements beyond a 
        /// simple text match.
        /// </summary>
        /// <param name="packet">The packet recieved from the game server.</param>
        protected abstract void Dispatch(P packet);

        /// <summary>
        /// Sends a packet to the server, provided a client exists and the connection is open and ready or logged in.
        /// This allows for the login command to be sent to a ready connection, otherwise no login packets could be sent.
        /// </summary>
        /// <param name="packet"></param>
        public override void Send(Packet packet) {
            if (this.Client != null && (this.Client.ConnectionState == ConnectionState.ConnectionReady || this.Client.ConnectionState == ConnectionState.ConnectionLoggedIn)) {
                this.Client.Send((P)packet);
            }
        }

        public override void Synchronize() {
            if (this.Client != null && this.Client.ConnectionState != ConnectionState.ConnectionLoggedIn) {
                this.Client.Poke();
            }
        }

        protected virtual void AssignEvents() {
            this.Client.PacketReceived += new Net.Client<P>.PacketDispatchHandler(Client_PacketReceived);
            this.Client.PacketSent += new Client<P>.PacketDispatchHandler(Client_PacketSent);
            this.Client.SocketException += new Client<P>.SocketExceptionHandler(Client_SocketException);
            this.Client.ConnectionStateChanged += new Client<P>.ConnectionStateChangedHandler(Client_ConnectionStateChanged);
            this.Client.ConnectionFailure += new Client<P>.FailureHandler(Client_ConnectionFailure);
        }
        
        private void Client_ConnectionFailure(Client<P> sender, Exception exception) {
            this.OnClientEvent(ClientEventType.ClientConnectionFailure, null, exception);
        }

        private void Client_ConnectionStateChanged(Client<P> sender, ConnectionState newState) {
            this.OnClientEvent(ClientEventType.ClientConnectionStateChange);

            this.State.Settings.ConnectionState = newState;

            if (newState == ConnectionState.ConnectionReady) {
                this.Login(this.Password);
            }
        }

        private void Client_SocketException(Client<P> sender, System.Net.Sockets.SocketException se) {
            this.OnClientEvent(ClientEventType.ClientSocketException, null, se);
        }

        private void Client_PacketSent(Client<P> sender, P packet) {
            this.OnClientEvent(ClientEventType.ClientPacketSent, packet);
        }

        private void Client_PacketReceived(Client<P> sender, P packet) {
            this.OnClientEvent(ClientEventType.ClientPacketReceived, packet);

            this.Dispatch(packet);
        }

        protected virtual void Dispatch(DispatchPacketAttribute identifer, P request, P response) {

            var dispatchMethods = this.DispatchHandlers.Where(dispatcher => dispatcher.Key.MatchText == identifer.MatchText)
                .Where(dispatcher => dispatcher.Key.PacketOrigin == PacketOrigin.None || dispatcher.Key.PacketOrigin == identifer.PacketOrigin)
                .Select(dispatcher => dispatcher.Value)
                .ToList();

            if (dispatchMethods.Any()) {
                foreach (MethodInfo dispatchMethod in dispatchMethods) {
                    dispatchMethod.Invoke(this, new object[] { request, response });
                }
            }
            else {
                this.DispatchFailed(identifer, request, response);
            }
        }

        protected virtual void DispatchFailed(DispatchPacketAttribute identifer, P request, P response) {

        }

        public override void AttemptConnection() {
            if (this.Client != null && this.Client.ConnectionState == ConnectionState.ConnectionDisconnected) {
                this.Client.Connect();
            }
        }

        #region Helper Methods

        protected abstract P CreatePacket(String format, params object[] args);

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

        #endregion

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
