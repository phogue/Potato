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
using System.Linq;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace Procon.Core.Interfaces.Connections {
    using Procon.Core.Interfaces.Connections.NLP;
    using Procon.Core.Interfaces.Connections.Plugins;
    using Procon.Core.Interfaces.Variables;
    using Procon.Core.Scheduler;
    using Procon.Core.Utils;
    using Procon.Net;
    using Procon.Net.Protocols.Objects;

    public class LocalConnection<G> : Connection
        where G : Game {

        // Public Objects
        public override GameState GameState {
            get           { return this.Game != null ? this.Game.State : null; }
            protected set { }
        }

        [JsonIgnore]
        public  TaskController Tasks {
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
        private   Game mGame;

        // Default Initialization
        public LocalConnection() : base() {
            Tasks = new TaskController().Start();
        }



        #region Executable

        /// <summary>
        /// Executes the commands specified in the config file and returns a reference itself.
        /// </summary>
        public override Connection Execute()
        {
            Game = (Game)Activator.CreateInstance(typeof(G), Hostname, Port);
            Game.Password       = Password;
            Game.Additional     = Additional;
            Game.GameConfigPath = Defines.CONFIGS_GAMES_DIRECTORY;
            AssignBubbledEvents();

            AssignEvents();

            StateNLP = new StateNLP() {
                Languages = MasterLanguages
            }.Execute();

            Plugins = new LocalPluginController() {
                Connection = this,
                Security   = Security,
                Variables  = Variables,
                Layer      = Layer
            }.Execute();

            Tasks.Add(
                new Task() {
                    Conditions = new Temporal() {
                        x => x.Second % 10 == 0
                    }
                }
            ).Tick += new Task.TickHandler(LocalConnection_Tick);

            return base.Execute();
        }

        /// <summary>
        /// Stops the plugins and tasks for this connection.
        /// </summary>
        public override void Dispose()
        {
            Plugins.Dispose();
            Tasks.Stop();
        }

        /// <summary>
        /// Relies on children classes to implement this.
        /// </summary>
        protected override void WriteConfig(XElement config) { }

        #endregion



        /// <summary>
        /// Assigns events to be handled by this class.
        /// </summary>
        protected override void AssignEvents()
        {
            ClientEvent += new Game.ClientEventHandler(LocalConnection_ClientEvent);
            GameEvent   += new Game.GameEventHandler(LocalConnection_GameEvent);
        }

        /// <summary>
        /// Assigns events in the game to be handled by this class.
        /// </summary>
        protected void AssignBubbledEvents()
        {
            Game.ClientEvent += new Game.ClientEventHandler(Game_ClientEvent);
            Game.GameEvent   += new Game.GameEventHandler(Game_GameEvent);
        }

        /// <summary>
        /// Sends commands across the layer.
        /// </summary>
        protected void BubbleRequest(CommandName command, EventName @event, params object[] parameters)
        {
            Layer.Request(Layer.ServerContext(Hostname, Port), command, @event, parameters);
        }

        /// <summary>
        /// Either attempt to reconnect or synchronize the game at each tick.
        /// </summary>
        private void LocalConnection_Tick(Task sender, DateTime dt)
        {
            if (Game != null && Game.State != null && Game.State.Variables.ConnectionState == ConnectionState.Disconnected)
                this.AttemptConnection();
            else
                this.Game.Synchronize();
        }

        /// <summary>
        /// Routes all the connection state change events to... not used?
        /// </summary>
        private void LocalConnection_ClientEvent(Game sender, ClientEventArgs e)
        {
            if (e.EventType == ClientEventType.ConnectionStateChange) ;
        }

        /// <summary>
        /// Routes all the chat events to the NLP checker.
        /// Routes all the server info events to... not used?
        /// </summary>
        private void LocalConnection_GameEvent(Game sender, GameEventArgs e)
        {
            if (e.EventType == GameEventType.Chat)
            {
                if (e.Chat.Text.Length > 0)
                {
                    string prefix = e.Chat.Text.First().ToString();
                    string text   = e.Chat.Text.Remove(0, 1);
                    if ((prefix = GetValidPrefix(prefix)) != null)
                        StateNLP.Execute(Game.State,
                                         e.Chat.Author,
                                         Security.Account(GameType, e.Chat.Author.UID),
                                         prefix,
                                         text);
                }
            }
            else if (e.EventType == GameEventType.ServerInfoUpdated) ;
        }

        /// <summary>
        /// Bubbles all game events up the connection and across the layer.
        /// </summary>
        private void Game_GameEvent(Game sender, GameEventArgs e)
        {
            OnGameEvent(sender, e);
            BubbleRequest(CommandName.None, EventName.GameEvent, null, e);
        }

        /// <summary>
        /// Bubbles all client events up the connection and across the layer.
        /// </summary>
        private void Game_ClientEvent(Game sender, ClientEventArgs e)
        {
            OnClientEvent(sender, e);
            if (e.EventType != ClientEventType.PacketReceived && e.EventType != ClientEventType.PacketSent)
                BubbleRequest(CommandName.None, EventName.ClientEvent, null, e);
        }



        #region This may be moved to a NLP Controller at some point

        /// <summary>
        /// Checks if a prefix is an allowed prefix
        /// </summary>
        /// <param name="prefix">The prefix to check (e.g !, @ etc.)</param>
        /// <returns>The parameter prefix, or null if the prefix is invalid</returns>
        private string GetValidPrefix(string prefix) {

            string result = null;

            if (prefix == Variables.Get<String>(CommandInitiator.Local, CommonVariableNames.TextCommandPublicPrefix) ||
                prefix == Variables.Get<String>(CommandInitiator.Local, CommonVariableNames.TextCommandProtectedPrefix) ||
                prefix == Variables.Get<String>(CommandInitiator.Local, CommonVariableNames.TextCommandPrivatePrefix)) {
                    result = prefix;
            }

            return result;
        }

        #endregion



        /// <summary>
        /// Attempts to establish a connection to the specified server and port.
        /// Begins communications with the server based on the children of this class.
        /// </summary>
        public override void AttemptConnection()
        {
            if (Game != null)
                Game.AttemptConnection();
        }

        /// <summary>
        /// Performs an action detailed in the protocol object.
        /// </summary>
        public override void Action(ProtocolObject action)
        {
            if (Game != null)
                Game.Action(action);
        }
    }
}
