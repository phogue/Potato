// Copyright 2011 Geoffrey 'Phogue' Green
// 
// http://www.phogue.net
//  
// This file is part of Procon 2.
// 
// Procon 2 is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// Procon 2 is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Procon 2.  If not, see <http://www.gnu.org/licenses/>.

using System;
using System.IO;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace Procon.Core.Interfaces.Connections {
    using Procon.Core.Interfaces.Connections.NLP;
    using Procon.Core.Interfaces.Connections.Plugins;
    using Procon.Core.Interfaces.Layer;
    using Procon.Core.Interfaces.Security;
    using Procon.Core.Interfaces.Variables;
    using Procon.Net;
    using Procon.Net.Protocols;
    using Procon.Net.Protocols.Objects;

    public abstract class Connection : Executable<Connection> {

        // Public Objects
        public  GameType GameType {
            get { return mGameType; }
            set {
                if (mGameType != value) {
                    mGameType = value;
                    OnPropertyChanged(this, "GameType");
                }
            }
        }
        private GameType mGameType;

        public  String Hostname {
            get { return mHostname; }
            set {
                if (mHostname != value) {
                    mHostname = value;
                    OnPropertyChanged(this, "Hostname");
                }
            }
        }
        private String mHostname;

        public  UInt16 Port {
            get { return mPort; }
            set {
                if (mPort != value) {
                    mPort = value;
                    OnPropertyChanged(this, "Port");
                }
            }
        }
        private UInt16 mPort;

        [JsonIgnore]
        public  String Password {
            get { return mPassword; }
            set {
                if (mPassword != value) {
                    mPassword = value;
                    OnPropertyChanged(this, "Password");
                }
            }
        }
        private String mPassword;
        
        public  String Additional {
            get { return mAdditional; }
            set {
                if (mAdditional != value) {
                    mAdditional = value;
                    OnPropertyChanged(this, "Additional");
                }
            }
        }
        private String mAdditional;

        public abstract GameState GameState
        {
            get;
            protected set;
        }

        public  SecurityController Security {
            get { return mSecurity; }
            set {
                if (mSecurity != value) {
                    mSecurity = value;
                    OnPropertyChanged(this, "Security");
                }
            }
        }
        private SecurityController mSecurity;
        
        public  VariableController Variables {
            get { return mVariables; }
            set {
                if (mVariables != value) {
                    mVariables = value;
                    OnPropertyChanged(this, "Variables");
                }
            }
        }
        private VariableController mVariables;

        public PluginController  Plugins {
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
        public  ILayer Layer {
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
        public  ICoreNLP StateNLP {
            get { return mStateNLP; }
            protected set {
                if (mStateNLP != value) {
                    mStateNLP = value;
                    OnPropertyChanged(this, "StateNLP");
                }
            }
        }
        private ICoreNLP mStateNLP;

        // Default Initialization
        public Connection() : base() {
        }



        #region Executable

        /// <summary>
        /// Executes the commands specified in the config file and returns a reference itself.
        /// </summary>
        public override Connection Execute()
        {
            return base.Execute();
        }

        /// <summary>
        /// Relies on children classes to implement this.
        /// </summary>
        public override void Dispose() { }

        /// <summary>
        /// Relies on children classes to implement this.
        /// </summary>
        protected override void WriteConfig(XElement config, ref FileInfo xFile) { }

        #endregion
        #region Events

        // -- Game Event --
        public event Game.GameEventHandler GameEvent;
        protected void OnGameEvent(Game sender, GameEventArgs e)
        {
            if (GameEvent != null)
                GameEvent(sender, e);
        }

        // -- Game Event --
        public event Game.ClientEventHandler ClientEvent;
        protected void OnClientEvent(Game sender, ClientEventArgs e)
        {
            if (ClientEvent != null)
                ClientEvent(sender, e);
        }

        #endregion



        /// <summary>
        /// Assigns events to be handled by this class.
        /// </summary>
        protected abstract void AssignEvents();



        /// <summary>
        /// Attempts to establish a connection to the specified server and port.
        /// Begins communications with the server based on the children of this class.
        /// </summary>
        public abstract void AttemptConnection();

        /// <summary>
        /// Performs an action detailed in the protocol object.
        /// </summary>
        public abstract void Action(ProtocolObject action);
    }
}
