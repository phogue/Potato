using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        // Public Accessors/Mutators.
        public List<Connection>   Connections {
            get { return mConnections; }
            protected set {
                if (mConnections != value) {
                    mConnections = value;
                    OnPropertyChanged(this, "Connection");
        } } }
        public SecurityController Security {
            get { return mSecurity; }
            set {
                if (mSecurity != value) {
                    mSecurity = value;
                    OnPropertyChanged(this, "Security");
        } } }
        public PackageController  Packages {
            get { return mPackages; }
            set {
                if (mPackages != value) {
                    mPackages = value;
                    OnPropertyChanged(this, "Packages");
        } } }
        public VariableController Variables {
            get { return mVariables; }
            set {
                if (mVariables != value) {
                    mVariables = value;
                    OnPropertyChanged(this, "Variables");
        } } }
        // Internal Variables.
        private List<Connection>   mConnections;
        private SecurityController mSecurity;
        private PackageController  mPackages;
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


        // Constructor.
        public Interface() : base() {
            Connections = new List<Connection>();
        }

        
        // Execute:
        // -- Loads the configuration file.
        public override Interface Execute()
        {
            return base.Execute();
        }
        // Dispose:
        // -- Disposes of all connections.
        public override void Dispose()
        {
            foreach (Connection c in Connections)
                c.Dispose();
            Connections.Clear();
        }


        // Manage the connections.
        public abstract void AddConnection(CommandInitiator initiator, String gametype, String hostname, UInt16 port, String password, String additional = "");
        public abstract void RemoveConnection(CommandInitiator initiator, String gametype, String hostname, UInt16 port);

        
        // Assigns events to be handled by this class.
        protected virtual void AssignEvents()
        {
            Layer.PropertyChanged   += Layer_ConnectionStateChanged;
            Layer.ProcessLayerEvent += Layer_ProcessLayerEvent;
        }
        private void Layer_ConnectionStateChanged(Object sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == "ConnectionState") {
                // TODO: Is this really necessary?
                // newState = Layer.ConnectionState
            }
        }
        private void Layer_ProcessLayerEvent(String username, Context context, CommandName command, EventName @event, Object[] parameters) {
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


        // Events.
        public delegate void ConnectionHandler(Interface parent, Connection item);
        public event         ConnectionHandler ConnectionAdded;
        public event         ConnectionHandler ConnectionRemoved;
        protected void OnConnectionAdded(Interface parent, Connection item)
        {
            if (ConnectionAdded != null)
                ConnectionAdded(parent, item);
        }
        protected void OnConnectionRemoved(Interface parent, Connection item)
        {
            if (ConnectionRemoved != null)
                ConnectionRemoved(parent, item);
        }
    }
}
