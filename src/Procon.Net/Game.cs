using System;
using System.Linq;
using System.Reflection;
using Procon.Net.Protocols;

namespace Procon.Net {
    using Procon.Net.Attributes;
    using Procon.Net.Protocols.Objects;

    public abstract class Game {

        /// <summary>
        /// Everything the connection currently knows about the game. This is updated
        /// with all of the information we receive from the server.
        /// </summary>
        public GameState State { get; protected set; }

        /// <summary>
        /// The end point hostname.
        /// </summary>
        public abstract string Hostname { get; }

        /// <summary>
        /// The endpoint port.
        /// </summary>
        public abstract ushort Port { get; }

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

        /// <summary>
        /// Helper to fetch the connection state of the underlying client.
        /// </summary>
        public abstract ConnectionState ConnectionState { get; }

        [Obsolete]
        private string _mAdditional;
        [Obsolete]
        public string Additional {
            get {
                return this._mAdditional;
            }
            set {
                this._mAdditional = value;

                // This way does not require System.Web reference.
                foreach (string item in this._mAdditional.Split('&')) {
                    string[] kvp = item.Split('=');

                    if (kvp.Length == 2) {
                        PropertyInfo property = this.GetType().GetProperty(kvp[0], BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);

                        if (property != null) {

                            property.SetValue(this, System.Convert.ChangeType(Uri.UnescapeDataString(kvp[1]), property.PropertyType), null);
                        }
                    }
                }
            }
        }
        
        /// <summary>
        /// Fired when ever a dispatched game event occurs.
        /// </summary>
        public virtual event GameEventHandler GameEvent;
        public delegate void GameEventHandler(Game sender, GameEventArgs e);

        /// <summary>
        /// Fired when something occurs with the underlying client. This can
        /// be connections, disconnections, logins or raw packets being recieved.
        /// </summary>
        public virtual event ClientEventHandler ClientEvent;
        public delegate void ClientEventHandler(Game sender, ClientEventArgs e);

        protected void OnClientEvent(ClientEventType eventType, Packet packet = null, Exception exception = null) {
            var handler = this.ClientEvent;
            if (handler != null) {
                handler(this, new ClientEventArgs() {
                    EventType = eventType,
                    ConnectionState = this.ConnectionState,
                    ConnectionError = exception,
                    Packet = packet
                });
            }
        }

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

        static Game() {
            // Load all supported game assemblies.
            SupportedGameTypes.GetSupportedGames();
        }

        // These may get transfered to a Interface used by Game and PacketFactory
        public abstract void Login(string password);

        /// <summary>
        /// Process a generic network action
        /// </summary>
        /// <param name="action"></param>
        public abstract void Action(NetworkAction action);

        /// <summary>
        /// Sends a raw packet to the connected server.
        /// </summary>
        /// <param name="packet"></param>
        public abstract void Send(Packet packet);

        /// <summary>
        /// Attempts a connection to the server.
        /// </summary>
        public abstract void AttemptConnection();

        /// <summary>
        /// Shutsdown this connection
        /// </summary>
        public abstract void Shutdown();

        /// <summary>
        /// General timed event to synch everything on the server with what is known locally.
        /// </summary>
        public abstract void Synchronize();
    }
}
