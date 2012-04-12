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
using System.Linq;

namespace Procon.Core.Interfaces.Connections.Plugins {
    using Procon.Core.Interfaces.Connections.Plugins.Variables;
    using Procon.Core.Interfaces.Layer.Objects;

    public class RemotePluginController : PluginController
    {
        /// <summary>
        /// Assigns events to be handled by this class.
        /// </summary>
        protected override void AssignEvents()
        {
            Layer.ProcessLayerEvent += Layer_ProcessLayerEvent;
        }

        /// <summary>
        /// Executes a command based on events sent across the layer.
        /// </summary>
        private void Layer_ProcessLayerEvent(String username, Context context, CommandName command, EventName @event, Object[] parameters)
        {
            if (context.ContextType == ContextType.Connection && Layer.ServerContext(Connection.Hostname, Connection.Port).CompareTo(context) == 0) {
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
        /// Synchronizes the plugin controller across the layer.
        /// </summary>
        public RemotePluginController Synchronize(PluginController pluginController)
        {
            Plugins.Clear();
            foreach (Plugin plugin in pluginController.Plugins)
                Plugins.Add(
                    new RemotePlugin() {
                        Uid           = plugin.Uid,
                        PluginDetails = plugin.PluginDetails
                    }
                );
            return this;
        }



        /// <summary>
        /// Callback event from the server.  The layer has let us know that a plugin was added.
        /// </summary>
        [Command(Event = EventName.PluginAdded)]
        protected void PluginAdded(CommandInitiator initiator, Plugin item)
        {
            if (Plugins.Find(x => x.Uid == item.Uid) == null) {
                Plugins.Add(
                    new RemotePlugin() {
                        Uid           = item.Uid,
                        PluginDetails = item.PluginDetails
                    }
                );
                OnPluginAdded(this, Plugins[Plugins.Count - 1]);
            }
        }

        /// <summary>
        /// Callback event from the server.  The layer has let us know that a plugin was removed.
        /// </summary>
        [Command(Event = EventName.PluginRemoved)]
        protected void PluginRemoved(CommandInitiator initiator, Plugin item)
        {
            Plugin plugin = Plugins.Where(x => x.Uid == item.Uid).FirstOrDefault();
            if (plugin != null) {
                Plugins.Remove(plugin);
                OnPluginRemoved(this, plugin);
            }
        }

        /// <summary>
        /// Callback event from the server.  The layer has let us know that a variable was set.
        /// </summary>
        [Command(Event = EventName.PluginVariableSet)]
        protected void GroupAdded(CommandInitiator initiator, String uid, String jsonVariable)
        {
            Plugin plugin = Plugins.Where(x => x.Uid == uid).FirstOrDefault();
            if (plugin != null)
                plugin.SetPluginVariable(initiator, Variable.FromJson(jsonVariable));
        }

        /// <summary>
        /// Sets variable(s) inside of the plugin using JSON as the bridge.
        /// </summary>
        public override void SetPluginVariable(CommandInitiator initiator, String uid, String jsonVariable)
        {
            Layer.Request(
                new Layer.Objects.Context() { ContextType = ContextType.All },
                CommandName.PluginSetVariable,
                EventName.None,
                uid,
                jsonVariable
            );
        }
    }
}
