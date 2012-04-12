// Copyright 2011 Cameron 'Imisnew2' Gunnin
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
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Media.Imaging;
using System.Windows.Threading;

using Procon.Core;
using Procon.Core.Interfaces;
using Procon.Core.Interfaces.Connections;
using Procon.Core.Interfaces.Layer;
using Procon.Net;
using Procon.UI.API.Utils;

namespace Procon.UI.API.ViewModels
{
    /// <summary>Wraps an Interface of Procon so that it can be used in the UI.</summary>
    public class InterfaceViewModel : ViewModel<Interface>
    {
        // Standard Model Properties
        public String Hostname
        {
            get { return Model.Layer.Hostname; }
        }
        public UInt16 Port
        {
            get { return Model.Layer.Port; }
        }
        public String Username
        {
            get { return (Model.Layer is LayerGame) ? (Model.Layer as LayerGame).Username : String.Empty; }
        }
        public String Password
        {
            get { return (Model.Layer is LayerGame) ? (Model.Layer as LayerGame).Password : String.Empty; }
        }
        public Boolean IsCompressed
        {
            get { return Model.Layer.IsCompressed; }
        }
        public Boolean IsEncrypted
        {
            get { return Model.Layer.IsEncrypted; }
        }
        public ConnectionState ConnectionState
        {
            get { return Model.Layer.ConnectionState; }
        }

        // Custom Properties
        public BitmapImage ConnectionStateIcon
        {
            get
            {
                switch (ConnectionState)
                {
                    case ConnectionState.LoggedIn:
                        return InstanceViewModel.PublicProperties["Images"]["Connection"]["Good"].Value as BitmapImage;
                    case ConnectionState.Connecting:
                    case ConnectionState.Connected:
                    case ConnectionState.Ready:
                        return InstanceViewModel.PublicProperties["Images"]["Connection"]["Flux"].Value as BitmapImage;
                    case ConnectionState.Disconnecting:
                    case ConnectionState.Disconnected:
                    default:
                        return InstanceViewModel.PublicProperties["Images"]["Connection"]["Bad"].Value as BitmapImage;
                }
            }
        }
        public Boolean IsLocal
        {
            get { return Model is LocalInterface; }
        }

        // View Model Properties
        public  ObservableCollection<ConnectionViewModel> Connections
        {
            get { return mConnections; }
            protected set {
                if (mConnections != value) {
                    mConnections = value;
                    OnPropertyChanged(this, "Connections");
                }
            }
        }
        private ObservableCollection<ConnectionViewModel> mConnections;

        /// <summary>Creates an instance of InterfaceViewModel and initalizes its properties.</summary>
        /// <param name="model">A reference to an instance of an interface in procon.</param>
        public InterfaceViewModel(Interface model) : base(model)
        {
            // Listen for changes within the model:
            Model.ConnectionAdded       += Connections_Added;
            Model.ConnectionRemoved     += Connections_Removed;
            Model.PropertyChanged       += Interface_PropertyChanged;
            Model.Layer.PropertyChanged += Interface_PropertyChanged;

            // Expose collections within the model:
            Connections = new ObservableCollection<ConnectionViewModel>(Model.Connections.Select(x => new ConnectionViewModel(x)));
        }



        /// <summary>
        /// Attempts to add a connection to the interface.
        /// </summary>
        public void AddConnection(String gameType, String hostname, UInt16 port, String password, String additional)
        {
            Model.AddConnection(
                CommandInitiator.Local,
                gameType,
                hostname,
                port,
                password,
                additional);
        }
        /// <summary>
        /// Attempts to remove a connection from the interface.
        /// </summary>
        public void RemoveConnection(String gameType, String hostname, UInt16 port)
        {
            Model.RemoveConnection(
                CommandInitiator.Local,
                gameType,
                hostname,
                port);
        }



        /// <summary>
        /// Lets the UI's view model know we added a connection.
        /// </summary>
        private void Connections_Added(Interface parent, Connection item)
        {
            // Force the UI thread to execute this method.
            if (Dispatcher.CurrentDispatcher != MainQueue) {
                MainQueue.Invoke(new System.Action(() => Connections_Added(parent, item)));
                return;
            }

            // Add the new connection.
            Connections.Add(new ConnectionViewModel(item));
        }
        /// <summary>
        /// Lets the UI's view model know we removed a connection.
        /// </summary>
        private void Connections_Removed(Interface parent, Connection item)
        {
            // Force the UI thread to execute this method.
            if (Dispatcher.CurrentDispatcher != MainQueue) {
                MainQueue.Invoke(new System.Action(() => Connections_Removed(parent, item)));
                return;
            }

            // Remove the old connection.
            for (int i = 0; i < Connections.Count; i++)
                if (Connections[i].ModelEquals(item)) {
                    Connections.RemoveAt(i);
                    break;
                }
        }
        /// <summary>
        /// Lets the UI's view model know a property in the model changed.
        /// </summary>
        private void Interface_PropertyChanged(Object sender, PropertyChangedEventArgs e)
        {
            // Force the UI thread to execute this method.
            if (Dispatcher.CurrentDispatcher != MainQueue) {
                MainQueue.Invoke(new System.Action(() => Interface_PropertyChanged(sender, e)));
                return;
            }

            // Connections collection was re-set?
            if (e.PropertyName == "Connections") {
                Boolean exists;
                // Removes models that no longer exist.
                for (int i = 0; i < Connections.Count; i++) {
                    exists = false;
                    for (int j = 0; j < Model.Connections.Count; j++)
                        if (exists = Connections[i].ModelEquals(Model.Connections[j]))
                            break;
                    if (!exists) Connections.RemoveAt(i--);
                }
                // Adds models that are new.
                for (int i = 0; i < Model.Connections.Count; i++) {
                    exists = false;
                    for (int j = 0; j < Connections.Count; j++)
                        if (exists = Connections[j].ModelEquals(Model.Connections[i]))
                            break;
                    if (exists) Connections.Add(new ConnectionViewModel(Model.Connections[i]));
                }
            }
            // Connection State was updated.
            else if (e.PropertyName == "ConnectionState") {
                OnPropertyChanged(this, e.PropertyName);
                OnPropertyChanged(this, e.PropertyName + "Icon");
            }
            // Other.
            else OnPropertyChanged(this, e.PropertyName);
        }
    }
}
