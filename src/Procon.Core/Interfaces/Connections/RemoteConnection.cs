using System;

namespace Procon.Core.Interfaces.Connections {
    using Procon.Core.Interfaces.Connections.Plugins;
    using Procon.Core.Interfaces.Layer;
    using Procon.Core.Interfaces.Layer.Objects;
    using Procon.Net;
    using Procon.Net.Protocols.Objects;

    public class RemoteConnection : Connection {

        // Public Accessors/Mutators.
        public override GameState GameState {
            get { return mGameState; }
            protected set {
                if (mGameState != value) {
                    mGameState = value;
                    OnPropertyChanged(this, "GameState");
                }
            }
        }

        // Internal Variables.
        private GameState mGameState;


        // Constructor.
        public RemoteConnection()
            : base() {
            GameState = new GameState();
        }


        // Execute:
        // -- Starts the execution of this object's plugins.
        // -- Loads the configuration file.
        public override Connection Execute() {
            Plugins = new RemotePluginController() {
                Connection = this,
                Layer = Layer
            }.Execute();

            return base.Execute();
        }

        // Dispose:
        // -- Disposes of this object's plugins.
        public override void Dispose() {
            Plugins.Dispose();
        }

        // Assigns events to be handled by this class.
        protected override void AssignEvents() {
            Layer.ProcessLayerEvent += new LayerGame.ProcessLayerEventHandler(Layer_ProcessLayerEvent);
        }

        private void Layer_ProcessLayerEvent(String username, Context context, CommandName command, EventName @event, object[] parameters) {
            if (Layer.ServerContext(Hostname, Port).CompareTo(context) == 0) {
                Execute(
                    new CommandInitiator() {
                        CommandOrigin = CommandOrigin.Remote,
                        Username = username
                    },
                    new CommandAttribute() {
                        Command = command,
                        Event = @event
                    },
                    parameters
                );
            }
        }

        // TODO: Not needed for a remote connection?
        public override void AttemptConnection() { }

        // Performs a detailed action specified in the protocol object.
        public override void Action(ProtocolObject action) {
            Layer.Request(Layer.ServerContext(Hostname, Port), CommandName.Action, EventName.None, action);
        }

        // Synchronizing the game state and plugins with the layer.
        public Connection Synchronize(Connection connection) {
            ((RemotePluginController)Plugins).Synchronize(connection.Plugins);
            return this;
        }

        [Command(Event = EventName.GameEvent)]
        protected void OnGameEvent(CommandInitiator initiator, Game sender, GameEventArgs e) {
            GameState = e.GameState;
            OnGameEvent(sender, e);

            if (e.EventType == GameEventType.ServerInfoUpdated) {
                // TODO: Why is this here?
            }
        }

        [Command(Event = EventName.ClientEvent)]
        protected void OnClientEvent(CommandInitiator initiator, Game sender, ClientEventArgs e) {
            OnClientEvent(sender, e);

            if (e.EventType == ClientEventType.ConnectionStateChange) {
                // TODO: Why is this here?
            }
        }
    }
}
