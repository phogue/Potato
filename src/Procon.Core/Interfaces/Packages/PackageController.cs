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
using System.ComponentModel;
using System.IO;
using System.Xml.Linq;
using Newtonsoft.Json;

namespace Procon.Core.Interfaces.Packages {
    using Procon.Core.Interfaces.Layer;
    using Procon.Core.Interfaces.Layer.Objects;

    public abstract class PackageController : Executable<PackageController>
    {    
        // Public Objects
        public  List<Package> Packages
        {
            get { return mPackages; }
            protected set {
                if (mPackages != value) {
                    mPackages = value;
                    OnPropertyChanged(this, "Packages");
                }
            }
        }
        private List<Package> mPackages;

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
        public PackageController() : base() {
            Packages = new List<Package>();
        }



        #region Executable

        /// <summary>
        /// Executes the commands specified in the config file and returns a reference itself.
        /// </summary>
        public override PackageController Execute()
        {
            return base.Execute();
        }

        /// <summary>
        /// Relies on children classes to implement this.
        /// </summary>
        public override void Dispose() { }

        /// <summary>
        /// Relies on children classes to implement this.
        /// </summary>
        protected override void WriteConfig(XElement config, ref FileInfo xFile) { }

        #endregion
        #region Events

        // -- Whenever an update to the core is available --
        public delegate void CoreUpdateAvailableHandler(PackageController parent, Package item);
        public event         CoreUpdateAvailableHandler CoreUpdateAvailable;
        protected void OnCoreUpdateAvailable(PackageController parent, Package item)
        {
            if (CoreUpdateAvailable != null)
                CoreUpdateAvailable(parent, item);
        }

        // -- Adding an Package --
        public delegate void PackageAddedHandler(PackageController parent, Package item);
        public event         PackageAddedHandler PackageAdded;
        protected void OnPackageAdded(PackageController parent, Package item)
        {
            if (PackageAdded != null)
                PackageAdded(parent, item);
        }

        // -- Removing an Package --
        public delegate void PackageRemovedHandler(PackageController parent, Package item);
        public event         PackageRemovedHandler PackageRemoved;
        protected void OnPackageRemoved(PackageController parent, Package item)
        {
            if (PackageRemoved != null)
                PackageRemoved(parent, item);
        }

        #endregion



        /// <summary>
        /// Listens for changes to do with the layer.
        /// Only changes to either the connection state or processing of an event are captured.
        /// </summary>
        protected virtual void AssignEvents() {
            Layer.PropertyChanged   += Layer_ConnectionStateChanged;
            Layer.ProcessLayerEvent += Layer_ProcessLayerEvent;
        }

        /// <summary>
        /// Handle events related to the connection state of the interface.
        /// </summary>
        private void Layer_ConnectionStateChanged(Object sender, PropertyChangedEventArgs e) {
            if (e.PropertyName == "ConnectionState") {
                // TODO: Is this really necessary?
                // newState = Layer.ConnectionState
            }
        }

        /// <summary>
        /// Handle events related to when the layer receives a command to be processed.
        /// </summary>
        private void Layer_ProcessLayerEvent(String username, Context context, CommandName command, EventName @event, Object[] parameters) {
            // Execute a command on the interface if something came in over the layer.
            if (context.ContextType == ContextType.All) {
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
        /// Attempts to install the package on the given interface that is running the local version.
        /// </summary>
        public abstract void InstallPackage(CommandInitiator initiator, String uid);
    }
}
