// Copyright 2011 Geoffrey 'Phogue' Green
// Modified by Cameron 'Imisnew2' Gunnin
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
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace Procon.Core.Interfaces {
    using Procon.Core.Interfaces.Connections;
    using Procon.Core.Interfaces.Layer;
    using Procon.Core.Interfaces.Layer.Objects;
    using Procon.Core.Interfaces.Packages;
    using Procon.Core.Interfaces.Security;
    using Procon.Core.Interfaces.Variables;

    public abstract class Interface : Executable<Interface>
    {
        // Public Properties
        public  List<Connection> Connections
        {
            get { return mConnections; }
            protected set {
                if (mConnections != value) {
                    mConnections = value;
                    OnPropertyChanged(this, "Connection");
                }
            }
        }
        private List<Connection> mConnections;

        public  SecurityController Security
        {
            get { return mSecurity; }
            set {
                if (mSecurity != value) {
                    mSecurity = value;
                    OnPropertyChanged(this, "Security");
                }
            }
        }
        private SecurityController mSecurity;

        public  PackageController Packages
        {
            get { return mPackages; }
            set {
                if (mPackages != value) {
                    mPackages = value;
                    OnPropertyChanged(this, "Packages");
                }
            }
        }
        private PackageController mPackages;

        public  VariableController Variables
        {
            get { return mVariables; }
            set {
                if (mVariables != value) {
                    mVariables = value;
                    OnPropertyChanged(this, "Variables");
                }
            }
        }
        private VariableController mVariables;

        [JsonIgnore]
        public  ILayer Layer
        {
            get { return mLayer; }
            set {
                if (mLayer != value) {
                    mLayer = value;
                    OnPropertyChanged(this, "Layer");
                }
            }
        }
        private ILayer mLayer;

        // Default Initialization
        public Interface() : base() {
            Connections = new List<Connection>();
        }



        #region Executable

        /// <summary>
        /// Executes the commands specified in the config file and returns a reference itself.
        /// </summary>
        public override Interface Execute()
        {
            return base.Execute();
        }
        
        /// <summary>
        /// Disposes of all the connections before calling the base dispose.
        /// </summary>
        public override void Dispose()
        {
            foreach (Connection c in Connections)
                c.Dispose();
            base.Dispose();
        }

        /// <summary>
        /// Relies on children classes to implement this.
        /// </summary>
        protected override void WriteConfig(XElement config, ref FileInfo xFile) { }

        #endregion
        #region Events

        // -- Adding an Connection --
        public delegate void ConnectionAddedHandler(Interface parent, Connection item);
        public event         ConnectionAddedHandler ConnectionAdded;
        protected void OnConnectionAdded(Interface parent, Connection item)
        {
            if (ConnectionAdded != null)
                ConnectionAdded(parent, item);
        }

        // -- Removing an Connection --
        public delegate void ConnectionRemovedHandler(Interface parent, Connection item);
        public event         ConnectionRemovedHandler ConnectionRemoved;
        protected void OnConnectionRemoved(Interface parent, Connection item)
        {
            if (ConnectionRemoved != null)
                ConnectionRemoved(parent, item);
        }

        #endregion



        /// <summary>
        /// Assigns events to be handled by this class.
        /// </summary>
        protected virtual void AssignEvents()
        {
            Layer.PropertyChanged   += Layer_ConnectionStateChanged;
            Layer.ProcessLayerEvent += Layer_ProcessLayerEvent;
        }

        /// <summary>
        /// Handle events related to the connection state of the interface.
        /// </summary>
        private void Layer_ConnectionStateChanged(Object sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == "ConnectionState") {
                // TODO: Is this really necessary?
                // newState = Layer.ConnectionState
            }
        }

        /// <summary>
        /// Handle events related to when the layer receives a command to be processed.
        /// </summary>
        private void Layer_ProcessLayerEvent(String username, Context context, CommandName command, EventName @event, Object[] parameters) {
            // Execute a command on the interface if something came in over the layer.
            if (context.ContextType == ContextType.All) {
                Execute(
                    new CommandInitiator() {
                        CommandOrigin = CommandOrigin.Remote,
                        Username      = username
                    },
                    new CommandAttribute() {
                        Command = command,
                        Event   = @event
                    },
                    parameters
                );
            }
        }



        /// <summary>
        /// Adds a connection to the interface.
        /// Forces children classes to override this so that either a local connection can be made
        /// or the command to add the connection can be sent to the layer.
        /// </summary>
        public abstract void AddConnection(CommandInitiator initiator, String gametype, String hostname, UInt16 port, String password, String additional = "");

        /// <summary>
        /// Removes a connection from the interface.
        /// Forces children classes to override this so that either a local connection can be removed
        /// or the command to remove the connection can be sent to the layer.
        /// </summary>
        public abstract void RemoveConnection(CommandInitiator initiator, String gametype, String hostname, UInt16 port);
    }
}
