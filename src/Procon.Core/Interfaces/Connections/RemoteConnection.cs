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

namespace Procon.Core.Interfaces.Connections {
    using Procon.Core.Interfaces.Connections.Plugins;
    using Procon.Core.Interfaces.Layer;
    using Procon.Core.Interfaces.Layer.Objects;
    using Procon.Net;
    using Procon.Net.Protocols.Objects;

    public class RemoteConnection : Connection {

        // Public Objects
        public override GameState GameState {
            get;
            protected set;
        }

        // Default Initialization
        public RemoteConnection() : base() {
            GameState = new GameState();
        }



        #region Executable

        /// <summary>
        /// Executes the commands specified in the config file and returns a reference itself.
        /// </summary>
        public override Connection Execute()
        {
            Plugins = new RemotePluginController() {
                Connection = this,
                Layer      = Layer
            }.Execute();

            return base.Execute();
        }

        /// <summary>
        /// Disposes nothing, as other classes clean up this object.
        /// </summary>
        public override void Dispose() { }

        /// <summary>
        /// Saves nothing, as remote classes are saved by their local counterparts and synchronized
        /// when Procon is ran again.
        /// </summary>
        protected override void WriteConfig(XElement config) { }

        #endregion



        /// <summary>
        /// Assigns events to be handled by this class.
        /// </summary>
        protected override void AssignEvents() {
            Layer.ProcessLayerEvent += new LayerGame.ProcessLayerEventHandler(Layer_ProcessLayerEvent);
        }

        /// <summary>
        /// Execute commands as the come across the layer.
        /// </summary>
        private void Layer_ProcessLayerEvent(String username, Context context, CommandName command, EventName @event, object[] parameters) {

            if (Layer.ServerContext(Hostname, Port).CompareTo(context) == 0)
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



        /// <summary>
        /// Synchronizes the plugins across the layer.
        /// </summary>
        public Connection Synchronize(Connection connection)
        {
            (Plugins as RemotePluginController).Synchronize(connection.Plugins);
            return this;
        }

        /// <summary>
        /// Sends an action across the layer.
        /// </summary>
        public override void Action(ProtocolObject action)
        {
            Layer.Request(Layer.ServerContext(Hostname, Port), CommandName.Action, EventName.None, action);
        }

        /// <summary>
        /// Not needed for remote connections?
        /// </summary>
        public override void AttemptConnection() { }



        /// <summary>
        /// Updates the game state across the layer.
        /// Fires game events as they come across the layer.
        /// </summary>
        [Command(Event = EventName.GameEvent)]
        protected void OnGameEvent(CommandInitiator initiator, Game sender, GameEventArgs e)
        {
            GameState = e.GameState;
            OnGameEvent(sender, e);

            if (e.EventType == GameEventType.ServerInfoUpdated) ;
        }

        /// <summary>
        /// Fires client events as they come across the layer.
        /// </summary>
        [Command(Event = EventName.ClientEvent)]
        protected void OnClientEvent(CommandInitiator initiator, Game sender, ClientEventArgs e)
        {
            OnClientEvent(sender, e);

            if (e.EventType == ClientEventType.ConnectionStateChange) ;
        }
    }
}
