using System;
using Newtonsoft.Json;

namespace Procon.Core.Interfaces.Connections {
    using Procon.Core.Interfaces.Connections.Text;
    using Procon.Core.Interfaces.Connections.Plugins;
    using Procon.Core.Interfaces.Layer;
    using Procon.Core.Interfaces.Security;
    using Procon.Core.Interfaces.Variables;
    using Procon.Net;
    using Procon.Net.Protocols;
    using Procon.Net.Protocols.Objects;

    public abstract class Connection : Executable<Connection> {

        // Public Accessors/Mutators.
        public GameType GameType {
            get { return mGameType; }
            set {
                if (mGameType != value) {
                    mGameType = value;
                    OnPropertyChanged(this, "GameType");
                }
            }
        }
        private GameType mGameType;
        
        public String Hostname {
            get { return mHostname; }
            set {
                if (mHostname != value) {
                    mHostname = value;
                    OnPropertyChanged(this, "Hostname");
                }
            }
        }
        private String mHostname;

        public UInt16 Port {
            get { return mPort; }
            set {
                if (mPort != value) {
                    mPort = value;
                    OnPropertyChanged(this, "Port");
                }
            }
        }
        private UInt16 mPort;

        public String Additional {
            get { return mAdditional; }
            set {
                if (mAdditional != value) {
                    mAdditional = value;
                    OnPropertyChanged(this, "Additional");
                }
            }
        }
        private String mAdditional;

        public SecurityController Security {
            get { return mSecurity; }
            set {
                if (mSecurity != value) {
                    mSecurity = value;
                    OnPropertyChanged(this, "Security");
                }
            }
        }
        private SecurityController mSecurity;

        public VariableController Variables {
            get { return mVariables; }
            set {
                if (mVariables != value) {
                    mVariables = value;
                    OnPropertyChanged(this, "Variables");
                }
            }
        }
        private VariableController mVariables;

        public PluginController Plugins {
            get { return mPlugins; }
            set {
                if (mPlugins != value) {
                    mPlugins = value;
                    OnPropertyChanged(this, "Plugins");
                }
            }
        }
        private PluginController mPlugins;

        [JsonIgnore]
        public String Password {
            get { return mPassword; }
            set {
                if (mPassword != value) {
                    mPassword = value;
                    OnPropertyChanged(this, "Password");
                }
            }
        }
        private String mPassword;

        [JsonIgnore]
        public ILayer Layer {
            get { return mLayer; }
            set {
                if (mLayer != value) {
                    mLayer = value;
                    OnPropertyChanged(this, "Layer");
                }
            }
        }
        private ILayer mLayer;

        [JsonIgnore]
        public ICoreNLP StateNLP {
            get { return mStateNLP; }
            protected set {
                if (mStateNLP != value) {
                    mStateNLP = value;
                    OnPropertyChanged(this, "StateNLP");
                }
            }
        }
        private ICoreNLP mStateNLP;

        public abstract GameState GameState {
            get;
            protected set;
        }

        #region Events

        public event Game.GameEventHandler GameEvent;
        public event Game.ClientEventHandler ClientEvent;

        #endregion

        // Constructor.
        public Connection()
            : base() {
        }

        // Execute:
        // -- Loads the configuration file.
        public override Connection Execute() {
            return base.Execute();
        }

        // Assigns events to be handled by this class.
        protected abstract void AssignEvents();

        // Attempts to begin communication with the game server.
        public abstract void AttemptConnection();

        // Performs a detailed action specified in the protocol object.
        public abstract void Action(ProtocolObject action);

        protected void OnGameEvent(Game sender, GameEventArgs e) {
            if (GameEvent != null) {
                GameEvent(sender, e);
            }
        }

        protected void OnClientEvent(Game sender, ClientEventArgs e) {
            if (ClientEvent != null) {
                ClientEvent(sender, e);
            }
        }
    }
}
