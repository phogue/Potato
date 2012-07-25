using System;
using System.Linq;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace Procon.Core.Interfaces.Connections {
    using Procon.Core.Interfaces.Connections.TextCommands;
    using Procon.Core.Interfaces.Connections.Plugins;
    using Procon.Core.Interfaces.Variables;
    using Procon.Core.Scheduler;
    using Procon.Core.Utils;
    using Procon.Net;
    using Procon.Net.Protocols.Objects;

    public class LocalConnection<G> : Connection where G : Game {

        // Public Accessors/Mutators.
        public override GameState GameState {
            get { return Game != null ? Game.State : null; }
            protected set { }
        }

        [JsonIgnore]
        public TaskController Tasks {
            get { return mTasks; }
            set {
                if (mTasks != value) {
                    mTasks = value;
                    OnPropertyChanged(this, "Tasks");
                }
            }
        }
        private TaskController mTasks;

        [JsonIgnore]
        protected Game Game {
            get { return mGame; }
            set {
                if (mGame != value) {
                    mGame = value;
                    OnPropertyChanged(this, "Game");
                }
            }
        }
        private Game mGame;

        // Constructor.
        public LocalConnection()
            : base() {
            Tasks = new TaskController();
            Tasks.Add(
                new Task() {
                    Conditions = new Temporal() {
                        x => x.Second % 10 == 0
                    }
                }
            ).Tick += new Task.TickHandler(LocalConnection_Tick);

            TextCommand = new LocalTextCommandController() {
                Connection = this,
                Languages = MasterLanguages
            }.Execute();
        }


        // Execute:
        // -- Creates the connection to the game server.
        // -- Assigns event handlers to deal with changes within the game and class.
        // -- Starts the execution of this object's plugins and tasks.
        // -- Adds a new task to execute every 10 seconds.
        // -- Loads the configuration file.
        public override Connection Execute() {
            Game = (Game)Activator.CreateInstance(typeof(G), Hostname, Port);
            Game.Password = Password;
            Game.Additional = Additional;
            Game.GameConfigPath = Defines.CONFIGS_GAMES_DIRECTORY;
            AssignBubbledEvents();

            AssignEvents();

            Plugins = new LocalPluginController() {
                Connection = this,
                Security = Security,
                Variables = Variables,
                Layer = Layer
            }.Execute();
            Tasks.Start();

            return base.Execute();
        }

        // Dispose:
        // -- Shuts down the connection to the game server.
        // -- Disposes of this object's plugins and tasks.
        public override void Dispose() {
            Game.Shutdown();

            Plugins.Dispose();
            Tasks.Stop();
        }

        // WriteConfig:
        // -- TODO: Saves all the plugins to the config file.
        internal override void WriteConfig(Config config) { }


        // Assigns events to be handled by this class.
        protected override void AssignEvents() {
            ClientEvent += new Game.ClientEventHandler(LocalConnection_ClientEvent);
        }

        private void LocalConnection_ClientEvent(Game sender, ClientEventArgs e) {
            if (e.EventType == ClientEventType.ConnectionStateChange) ;
        }

        private void LocalConnection_Tick(Task sender, DateTime dt) {
            if (Game != null && Game.State != null && Game.State.Variables.ConnectionState == ConnectionState.Disconnected) {
                AttemptConnection();
            }
            else {
                Game.Synchronize();
            }
                
        }

        // Bubbles events fired by the game.
        private void AssignBubbledEvents() {
            Game.ClientEvent += new Game.ClientEventHandler(Game_ClientEvent);
            Game.GameEvent += new Game.GameEventHandler(Game_GameEvent);
        }

        private void Game_GameEvent(Game sender, GameEventArgs e) {
            OnGameEvent(sender, e);
            BubbleRequest(CommandName.None, EventName.GameEvent, null, e);
        }

        private void Game_ClientEvent(Game sender, ClientEventArgs e) {
            OnClientEvent(sender, e);
            if (e.EventType != ClientEventType.PacketReceived && e.EventType != ClientEventType.PacketSent) {
                BubbleRequest(CommandName.None, EventName.ClientEvent, null, e);
            }
        }

        private void BubbleRequest(CommandName command, EventName @event, params Object[] parameters) {
            Layer.Request(Layer.ServerContext(Hostname, Port), command, @event, parameters);
        }


        // Attempts to begin communication with the game server.
        public override void AttemptConnection() {
            if (Game != null) {
                Game.AttemptConnection();
            }
        }

        // Performs a detailed action specified in the protocol object.
        public override void Action(ProtocolObject action) {
            if (Game != null) {
                Game.Action(action);
            } 
        }
    }
}
