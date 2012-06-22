using System;
using System.Linq;

namespace Procon.Core.Interfaces {
    using Procon.Core.Interfaces.Connections;
    using Procon.Core.Interfaces.Layer;
    using Procon.Core.Interfaces.Layer.Objects;
    using Procon.Core.Interfaces.Packages;
    using Procon.Core.Interfaces.Security;
    using Procon.Core.Interfaces.Variables;

    public class RemoteInterface : Interface
    {
        // Constructor.
        public RemoteInterface(String hostname, UInt16 port, String username, String password) {
            Layer = new LayerGame(hostname, port) {
                Username = username,
                Password = password
            };
            Security = new RemoteSecurityController() {
                Layer = Layer
            };
            Packages = new RemotePackageController() {
                Layer = Layer
            };
            Variables = new RemoteVariableController() {
                Layer = Layer
            };
        }

        
        // Execute:
        // -- Begins synchronization of the layer.
        // -- Starts the execution of this object's security, packages, and variables.
        // -- Loads the configuration file.
        public override Interface Execute()
        {
            Layer.Begin();

            Security.Execute();
            Packages.Execute();
            Variables.Execute();

            return base.Execute();
        }
        // Dispose:
        // -- Shuts down the layer.
        // -- Disposes of this object's security, packages, and variables.
        // -- Calls the base dispose.
        public override void Dispose()
        {
            Layer.Shutdown();

            Security.Dispose();
            Packages.Dispose();
            Variables.Dispose();

            base.Dispose();
        }


        // Manage the connections.
        public override void AddConnection(CommandInitiator initiator, String gametype, String hostname, UInt16 port, String password, String additional = "") {
            Layer.Request(
                new Context() { ContextType = ContextType.All },
                CommandName.ConnectionsAddConnection,
                EventName.None,
                gametype,
                hostname,
                port,
                password,
                additional
            );
        }
        public override void RemoveConnection(CommandInitiator initiator, String gametype, String hostname, UInt16 port) {
            Layer.Request(
                new Context() { ContextType = ContextType.All },
                CommandName.ConnectionsRemoveConnection,
                EventName.None,
                gametype,
                hostname,
                port
            );
        }


        // Synchronizing connections with the layer.
        [Command(Event = EventName.InterfaceSynchronization)]
        protected void InterfaceSynchronization(CommandInitiator initiator, Interface @interface)
        {
            (Security as RemoteSecurityController).Synchronize(@interface.Security);
            (Packages as RemotePackageController).Synchronize(@interface.Packages);
            (Variables as RemoteVariableController).Synchronize(@interface.Variables);

            while (Connections.Count > 0) {
                Connections[0].Dispose();
                Connections.RemoveAt(0);
            }
            OnPropertyChanged(this, "Connections");

            foreach (Connection connection in @interface.Connections)
                LayerConnectionAdded(initiator, connection);
        }
        [Command(Event = EventName.ConnectionsConnectionAdded)]
        protected void LayerConnectionAdded(CommandInitiator initiator, Connection connection)
        {
            RemoteConnection rConnection = new RemoteConnection() {
                GameType  = connection.GameType,
                Hostname  = connection.Hostname,
                Port      = connection.Port,
                Layer     = Layer,
                Security  = Security
            };
            rConnection.Execute();
            rConnection.Synchronize(connection);

            Connections.Add(rConnection);
            OnConnectionAdded(this, rConnection);
        }
        [Command(Event = EventName.ConnectionsConnectionRemoved)]
        protected void LayerConnectionRemoved(CommandInitiator initiator, Connection connection)
        {
            Connection remote = Connections
                                   .Where(x => x.GameType == connection.GameType &&
                                               x.Hostname == connection.Hostname &&
                                               x.Port     == connection.Port)
                                   .FirstOrDefault();
            if (remote != null) {
                Connections.Remove(remote);
                OnConnectionRemoved(this, remote);
                remote.Dispose();
            }
        }
    }
}
