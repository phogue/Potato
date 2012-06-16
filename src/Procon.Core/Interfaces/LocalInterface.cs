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
using System.ComponentModel;
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
    using Procon.Net;
    using Procon.Net.Protocols;

    public class LocalInterface : Interface
    {
        // Default Initialization
        public LocalInterface() : base() {
            Layer = new LayerListener();
            Security = new LocalSecurityController() {
                Layer = Layer
            };
            Packages = new LocalPackageController() {
                Layer    = Layer,
                Security = Security
            };
            Variables = new LocalVariableController() {
                Layer     = Layer,
                Security  = Security,
                Arguments = Arguments
            };
            ((LayerListener)Layer).Security = Security;
        }

        

        #region Executable

        /// <summary>
        /// Assigns event handlers to deal with changes within the class.
        /// Starts the execution of this object's security, packages, and variables.
        /// Executes the commands specified in the config file and returns a reference itself.
        /// </summary>
        public override Interface Execute()
        {
            AssignEvents();
            
            Security.Execute();
            Packages.Execute();
            Variables.Execute();

            return base.Execute();
        }

        /// <summary>
        /// Shutsdown the layer. Disposes of this object's security, packages, and variables before
        /// calling the base dispose.
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
        /// Saves information about each connection to the configuation file.
        /// </summary>
        protected override void WriteConfig(XElement xNamespace)
        {
            foreach (Connection connection in this.Connections)
                xNamespace.Add(new XElement("command",
                    new XAttribute("name", CommandName.ConnectionsAddConnection), // gametype, hostname, port, password, additional
                    new XElement("gametype",   connection.GameType),
                    new XElement("hostname",   connection.Hostname),
                    new XElement("port",       connection.Port),
                    new XElement("password",   connection.Password),
                    new XElement("additional", connection.Additional)
                ));
        }

        #endregion



        /// <summary>
        /// Assigns events to be handled by this class.
        /// </summary>
        protected virtual void AssignEvents()
        {
            base.AssignEvents();
            ((LayerListener)Layer).ClientAdded   += Layer_ClientAdded;
            ((LayerListener)Layer).ClientRemoved += Layer_ClientRemoved;
            ConnectionAdded                      += Connections_ConnectionAdded;
            ConnectionRemoved                    += Connections_ConnectionRemoved;
            Packages.CoreUpdateAvailable         += Packages_CoreUpdateAvailable;
        }

        /// <summary>
        /// Starts monitoring the client's connection state once it has been added to the layer.
        /// </summary>
        private void Layer_ClientAdded(LayerListener parent, LayerGame item)
        {
            item.PropertyChanged += Client_ConnectionStateChanged;
        }

        /// <summary>
        /// Stops monitoring the client's connection state once it has been removed from the layer.
        /// </summary>
        private void Layer_ClientRemoved(LayerListener parent, LayerGame item)
        {
            item.PropertyChanged -= Client_ConnectionStateChanged;
        }

        /// <summary>
        /// Synchronizes the layer and client once the client has finished connecting to the layer.
        /// </summary>
        private void Client_ConnectionStateChanged(Object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "ConnectionState")
            {
                LayerGame parent = (LayerGame)sender;
                if (parent.ConnectionState == ConnectionState.LoggedIn)
                    parent.Request(
                        new Context() { ContextType = ContextType.All },
                        CommandName.None,
                        EventName.InterfaceSynchronization,
                        this
                    );
            }
        }



        /// <summary>
        /// Lets the client's of the layer know we added a connection.
        /// </summary>
        private void Connections_ConnectionAdded(Interface parent, Connection item)
        {
            Layer.Request(
                new Layer.Objects.Context() { ContextType = ContextType.All },
                CommandName.None,
                EventName.ConnectionsConnectionAdded,
                item
            );
        }

        /// <summary>
        /// Lets the client's of the layer know we removed a connection.
        /// </summary>
        private void Connections_ConnectionRemoved(Interface parent, Connection item)
        {
            Layer.Request(
                new Context() { ContextType = ContextType.All },
                CommandName.None,
                EventName.ConnectionsConnectionRemoved,
                item
            );
        }

        /// <summary>
        /// Is called whenever there is an update for a specific package.
        /// </summary>
        private void Packages_CoreUpdateAvailable(PackageController sender, Package package) {
            // TODO: Add option to control "automatic updating"
            (package as LocalPackage).Install();
        }



        /// <summary>
        /// Creates a connection using the specified details and attempts to establish a connection.
        /// </summary>
        [Command(Command = CommandName.ConnectionsAddConnection)]
        public override void AddConnection(CommandInitiator initiator, string gametype, string hostname, ushort port, string password, string additional = "")
        {
            // As long as the current account is allowed to execute this command...
            if (initiator.CommandOrigin == CommandOrigin.Local || Security.Can(Security.Account(initiator.Username), initiator.Command))
            {
                // As long as we have less than the maximum amount of connections...
                if (Connections.Count < Variables.Get<Int32>(CommandInitiator.Local, CommonVariableNames.MaximumGameConnections, 9000))
                {
                    Type[] t = System.Reflection.Assembly.GetExecutingAssembly().GetTypes();

                    // As long as the connection for that specific game, hostname, and port does not exist...
                    if (Connections.Where(x => x.GameType.ToString() == gametype && x.Hostname == hostname && x.Port == port).Select(x => x).FirstOrDefault() == null)
                    {
                        // As long as the game type is defined...
                        if (Enum.IsDefined(typeof(GameType), gametype) == true)
                        {
                            GameType game     = (GameType)Enum.Parse(typeof(GameType), gametype);
                            Type     gameType = Game.GetSupportedGames().Where(x => x.Key == game).Select(x => x.Value).FirstOrDefault();

                            // As long as the game type selected is supported...
                            if (gameType != null)
                            {
                                Connection connection = (Connection)Activator.CreateInstance(Type.GetType("Procon.Core.Interfaces.Connections.LocalConnection`1").MakeGenericType(gameType));
                                connection.Hostname   = hostname;
                                connection.Port       = port;
                                connection.Password   = password;
                                connection.Additional = additional;
                                connection.Security   = Security;
                                connection.GameType   = game;
                                connection.Layer      = Layer;
                                connection.Variables  = Variables;

                                connection.Execute();
                                connection.AttemptConnection();
                                Connections.Add(connection);

                                OnConnectionAdded(this, connection);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Removes all connections whose fields match those specified.
        /// </summary>
        [Command(Command = CommandName.ConnectionsRemoveConnection)]
        public override void RemoveConnection(CommandInitiator initiator, String gametype, String hostname, UInt16 port)
        {
            if (initiator.CommandOrigin == CommandOrigin.Remote && Security.Can(Security.Account(initiator.Username), initiator.Command))
            {
                Connection local = Connections
                                       .Where(x => x.GameType.ToString() == gametype &&
                                                   x.Hostname            == hostname &&
                                                   x.Port                == port)
                                       .FirstOrDefault();
                if (local != null)
                {
                    local.Dispose();
                    Connections.Remove(local);
                    OnConnectionRemoved(this, local);
                }
            }
        }
    }
}
