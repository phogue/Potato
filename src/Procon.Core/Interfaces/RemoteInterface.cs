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
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Procon.Core.Interfaces {
    using Procon.Core.Interfaces.Connections;
    using Procon.Core.Interfaces.Layer;
    using Procon.Core.Interfaces.Layer.Objects;
    using Procon.Core.Interfaces.Packages;
    using Procon.Core.Interfaces.Security;
    using Procon.Core.Interfaces.Variables;

    public class RemoteInterface : Interface
    {
        // Default Initialization.
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

        

        #region Executable

        /// <summary>
        /// Starts the execution of this object's security, packages, and variables.
        /// Executes the commands specified in the config file and returns a reference itself.
        /// </summary>
        public override Interface Execute()
        {
            Layer.Begin();

            Security.Execute();
            Packages.Execute();
            Variables.Execute();

            return base.Execute();
        }

        /// <summary>
        /// Shutsdown the layer and disposes of security, packages, and variables.
        /// </summary>
        public override void Dispose()
        {
            Layer.Shutdown();

            Security.Dispose();
            Packages.Dispose();
            Variables.Dispose();

            base.Dispose();
        }

        /// <summary>
        /// Saves nothing, as remote classes are saved by their local counterparts and synchronized
        /// when Procon is ran again.
        /// </summary>
        protected override void WriteConfig(XElement xNamespace) { }

        #endregion



        /// <summary>
        /// Sends a request to the layer to create a connection using the specified details.  The layer then
        /// attempts to create and establish a connection.
        /// </summary>
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

        /// <summary>
        /// Sends a request to the layer to remove all connections with the specified details.  The layer then
        /// attempts to find and remove the connections.
        /// </summary>
        public override void RemoveConnection(CommandInitiator initiator, string gametype, string hostname, ushort port) {
            Layer.Request(
                new Context() { ContextType = ContextType.All },
                CommandName.ConnectionsRemoveConnection,
                EventName.None,
                gametype,
                hostname,
                port
            );
        }



        /// <summary>
        /// Synchronizes this remote interface with the layers information by setting this objects information
        /// to the same as the layers.
        /// </summary>
        [Command(Event = EventName.InterfaceSynchronization)]
        protected void InterfaceSynchronization(CommandInitiator initiator, Interface @interface)
        {
            (Security as RemoteSecurityController).Synchronize(@interface.Security);
            (Packages as RemotePackageController).Synchronize(@interface.Packages);
            (Variables as RemoteVariableController).Synchronize(@interface.Variables);

            while (Connections.Count > 0)
            {
                Connections[0].Dispose();
                Connections.RemoveAt(0);
            }
            OnPropertyChanged(this, "Connections");

            foreach (Connection connection in @interface.Connections)
                LayerConnectionAdded(initiator, connection);
        }

        /// <summary>
        /// Callback event from the server.  The layer has let us know that a connection was created.
        /// </summary>
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

        /// <summary>
        /// Callback event from the server.  The server has let us know that a connection was deleted.
        /// </summary>
        [Command(Event = EventName.ConnectionsConnectionRemoved)]
        protected void LayerConnectionRemoved(CommandInitiator initiator, Connection connection)
        {
            Connection remote = Connections
                                   .Where(x => x.GameType == connection.GameType &&
                                               x.Hostname == connection.Hostname &&
                                               x.Port     == connection.Port)
                                   .FirstOrDefault();
            if (remote != null)
            {
                remote.Dispose();
                Connections.Remove(remote);
                OnConnectionRemoved(this, remote);
            }
        }
    }
}
