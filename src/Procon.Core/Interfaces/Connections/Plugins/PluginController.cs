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
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace Procon.Core.Interfaces.Connections.Plugins {
    using Procon.Core.Interfaces.Layer;

    public abstract class PluginController : Executable<PluginController> {

        // Public Properties
        public  List<Plugin> Plugins
        {
            get { return mPlugins; }
            protected set {
                if (mPlugins != value) {
                    mPlugins = value;
                    OnPropertyChanged(this, "Plugins");
                }
            }
        }
        private List<Plugin> mPlugins;

        [JsonIgnore]
        public  Connection Connection
        {
            get { return mConnection; }
            set {
                if (mConnection != value) {
                    mConnection = value;
                    OnPropertyChanged(this, "Connection");
                }
            }
        }
        private Connection mConnection;

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

        // Default Initialization
        public PluginController() : base() {
            Plugins = new List<Plugin>();
        }



        #region Executable

        /// <summary>
        /// Executes the commands specified in the config file and returns a reference itself.
        /// </summary>
        public override PluginController Execute()
        {
            return base.Execute();
        }

        /// <summary>
        /// Disposes of all the plugins before calling the base dispose.
        /// </summary>
        public override void Dispose()
        {
            foreach (Plugin p in Plugins)
                p.Dispose();
            base.Dispose();
        }

        /// <summary>
        /// Relies on children classes to implement this.
        /// </summary>
        protected override void WriteConfig(XElement config, ref FileInfo xFile) { }

        #endregion
        #region Events

        // -- Adding an Plugin --
        public delegate void PluginAddedHandler(PluginController parent, Plugin item);
        public event         PluginAddedHandler PluginAdded;
        protected void OnPluginAdded(PluginController parent, Plugin item)
        {
            if (PluginAdded != null)
                PluginAdded(parent, item);
        }

        // -- Removing an Plugin --
        public delegate void PluginRemovedHandler(PluginController parent, Plugin item);
        public event         PluginRemovedHandler PluginRemoved;
        protected void OnPluginRemoved(PluginController parent, Plugin item)
        {
            if (PluginRemoved != null)
                PluginRemoved(parent, item);
        }

        #endregion



        /// <summary>
        /// Assigns events to be handled by this class.
        /// </summary>
        protected abstract void AssignEvents();



        /// <summary>
        /// Sets variable(s) inside of the plugin using JSON as the bridge.
        /// </summary>
        public abstract void SetPluginVariable(CommandInitiator initiator, String uid, String jsonVariable);
    }
}
