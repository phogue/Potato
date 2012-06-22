using System;
using System.ComponentModel;
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
        // Constructor.
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

        
        // Execute:
        // -- Assigns event handlers to deal with changes within the class.
        // -- Starts the execution of this object's security, packages, and variables.
        // -- Loads the configuration file.
        public override Interface Execute()
        {
            AssignEvents();
            
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
        // WriteConfig:
        // -- Saves all the connections to the config file.
        internal override void WriteConfig(Config config)
        {
            foreach (Connection tConnection in this.Connections) {
                Config tConfig = new Config().Generate(tConnection.GetType());
                config.Root.Add(new XElement("command",
                    new XAttribute("name", CommandName.ConnectionsAddConnection), // gametype, hostname, port, password, additional
                    new XElement("gametype",   tConnection.GameType),
                    new XElement("hostname",   tConnection.Hostname),
                    new XElement("port",       tConnection.Port),
                    new XElement("password",   tConnection.Password),
                    new XElement("additional", tConnection.Additional)
                ));
                tConnection.WriteConfig(tConfig);
                config.Add(tConfig);
            }
        }

        
        // Manage the connections.
        [Command(Command = CommandName.ConnectionsAddConnection)]
        public override void AddConnection(CommandInitiator initiator, String gametype, String hostname, UInt16 port, String password, String additional = "")
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
                                connection.GameType   = game;
                                connection.Hostname   = hostname;
                                connection.Port       = port;
                                connection.Password   = password;
                                connection.Additional = additional;
                                connection.Layer      = Layer;
                                connection.Security   = Security;
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
        [Command(Command = CommandName.ConnectionsRemoveConnection)]
        public override void RemoveConnection(CommandInitiator initiator, String gametype, String hostname, UInt16 port)
        {
            // As long as the current account is allowed to execute this command...
            if (initiator.CommandOrigin == CommandOrigin.Local && Security.Can(Security.Account(initiator.Username), initiator.Command))
            {
                Connection local = Connections
                                       .Where(x => x.GameType.ToString() == gametype &&
                                                   x.Hostname            == hostname &&
                                                   x.Port                == port)
                                       .FirstOrDefault();
                // As long as the connection for that specific game, hostname, and port exists...
                if (local != null) {
                    Connections.Remove(local);
                    OnConnectionRemoved(this, local);
                    local.Dispose();
                }
            }
        }
        

        // Assigns events to be handled by this class.
        protected override void AssignEvents()
        {
            base.AssignEvents();
            ((LayerListener)Layer).ClientAdded   += Layer_ClientAdded;
            ((LayerListener)Layer).ClientRemoved += Layer_ClientRemoved;
            ConnectionAdded                      += Connections_ConnectionAdded;
            ConnectionRemoved                    += Connections_ConnectionRemoved;
            Packages.CoreUpdateAvailable         += Packages_CoreUpdateAvailable;
        }
        private void Layer_ClientAdded(LayerListener parent, LayerGame item) {
            item.PropertyChanged += Client_ConnectionStateChanged;
        }
        private void Layer_ClientRemoved(LayerListener parent, LayerGame item) {
            item.PropertyChanged -= Client_ConnectionStateChanged;
        }
        private void Client_ConnectionStateChanged(Object sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == "ConnectionState") {
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
        private void Connections_ConnectionAdded(Interface parent, Connection item) {
            Layer.Request(
                new Layer.Objects.Context() { ContextType = ContextType.All },
                CommandName.None,
                EventName.ConnectionsConnectionAdded,
                item
            );
        }
        private void Connections_ConnectionRemoved(Interface parent, Connection item) {
            Layer.Request(
                new Context() { ContextType = ContextType.All },
                CommandName.None,
                EventName.ConnectionsConnectionRemoved,
                item
            );
        }
        private void Packages_CoreUpdateAvailable(PackageController sender, Package package) {
            // TODO: Add option to control "automatic updating"
            ((LocalPackage)package).Install();
        }
    }
}
