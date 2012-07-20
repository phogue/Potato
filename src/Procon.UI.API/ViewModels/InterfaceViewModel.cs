using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

using Procon.Core;
using Procon.Core.Interfaces;
using Procon.Core.Interfaces.Connections;
using Procon.Core.Interfaces.Layer;
using Procon.Core.Interfaces.Repositories;
using Procon.Core.Interfaces.Repositories.Objects;
using Procon.Net;

namespace Procon.UI.API.ViewModels
{
    public class InterfaceViewModel : ViewModel<Interface>
    {
        // Properties.
        public String          Hostname
        {
            get { return nModel.Layer.Hostname; }
            set { nModel.Layer.Hostname = value; }
        }
        public UInt16          Port
        {
            get { return nModel.Layer.Port; }
            set { nModel.Layer.Port = value; }
        }
        public String          Username
        {
            get { return (nModel.Layer is LayerGame) ? (nModel.Layer as LayerGame).Username : null; }
            set { if (nModel.Layer is LayerGame) (nModel.Layer as LayerGame).Username = value; }
        }
        public String          Password
        {
            get { return (nModel.Layer is LayerGame) ? (nModel.Layer as LayerGame).Password : null; }
            set { if (nModel.Layer is LayerGame) (nModel.Layer as LayerGame).Password = value; }
        }
        public ConnectionState ConnectionState
        {
            get { return nModel.Layer.ConnectionState; }
        }

        // Custom Properties.
        public Boolean IsLocal
        {
            get { return nModel is LocalInterface; }
        }
        
        // Observable Properties.
        public ObservableCollection<ConnectionViewModel> Connections
        {
            get { return mConnections; }
            protected set {
                if (mConnections != value) {
                    mConnections = value;
                    OnPropertyChanged("Connections");
        } } }
        public ObservableCollection<FlatPackedPackage>             Packages
        {
            get { return mPackages; }
            protected set {
                if (mPackages != value) {
                    mPackages = value;
                    OnPropertyChanged("Packages");
        } } }

        private ObservableCollection<ConnectionViewModel> mConnections;
        private ObservableCollection<FlatPackedPackage> mPackages;


        // Constructor.
        public InterfaceViewModel(Interface model) : base(model)
        {
            // Listen for changes within the model:
            nModel.ConnectionAdded          += Connections_Added;
            nModel.ConnectionRemoved        += Connections_Removed;
            nModel.Packages.PackageAdded    += Packages_Added;
            nModel.Packages.PackageRemoved  += Packages_Removed;
            nModel.PropertyChanged          += Interface_PropertyChanged;
            nModel.Layer.PropertyChanged    += Interface_PropertyChanged;
            nModel.Packages.PropertyChanged += Interface_PropertyChanged;

            // Expose collections within the model:
            Connections = new ObservableCollection<ConnectionViewModel>(nModel.Connections.Select(x => new ConnectionViewModel(x)));
            Packages = new ObservableCollection<FlatPackedPackage>(nModel.Packages.Packages);
        }
        

        // View Model Methods.
        public void AddConnection(String gameType, String hostname, UInt16 port, String password, String additional)
        {
            nModel.AddConnection(
                CommandInitiator.Local,
                gameType,
                hostname,
                port,
                password,
                additional);
        }
        public void RemoveConnection(String gameType, String hostname, UInt16 port)
        {
            nModel.RemoveConnection(
                CommandInitiator.Local,
                gameType,
                hostname,
                port);
        }


        // Wraps the Added & Removed events.
        private void Connections_Added(Interface parent, Connection item)
        {
            // Force the UI thread to execute this method.
            if (ChangeDispatcher(() => Connections_Added(parent, item)))
                return;

            // Add the new connection.
            Connections.Add(new ConnectionViewModel(item));
        }
        private void Connections_Removed(Interface parent, Connection item)
        {
            // Force the UI thread to execute this method.
            if (ChangeDispatcher(() => Connections_Removed(parent, item)))
                return;

            // Remove the old connection.
            Connections.Remove(Connections.SingleOrDefault(x => x.ModelEquals(item)));
        }
        private void Packages_Added(PackageController parent, FlatPackedPackage item)
        {
            // Force the UI thread to execute this method.
            if (ChangeDispatcher(() => Packages_Added(parent, item)))
                return;

            // Add the new package.
            Packages.Add(item);
        }
        private void Packages_Removed(PackageController parent, FlatPackedPackage item)
        {
            // Force the UI thread to execute this method.
            if (ChangeDispatcher(() => Packages_Removed(parent, item)))
                return;

            // Remove the old package.
            Packages.Remove(item);
        }
        // Wraps the Interface's property changed events.
        private void Interface_PropertyChanged(Object sender, PropertyChangedEventArgs e)
        {
            // Force the UI thread to execute this method.
            if (ChangeDispatcher(() => Interface_PropertyChanged(sender, e)))
                return;

            if (e.PropertyName == "Connections") {
                // Removes models that no longer exist.
                for (int i = 0; i < Connections.Count; i++)
                    if (nModel.Connections.SingleOrDefault(x => Connections[i].ModelEquals(x)) == null)
                        Connections.RemoveAt(i--);
                // Adds models that are new.
                for (int i = 0; i < nModel.Connections.Count; i++)
                    if (Connections.SingleOrDefault(x => x.ModelEquals(nModel.Connections[i])) == null)
                        Connections.Add(new ConnectionViewModel(nModel.Connections[i]));
            }

            if (e.PropertyName == "Packages") {
                // Re-set the event listeners.
                if (sender == nModel) {
                    nModel.Packages.PackageAdded    += Packages_Added;
                    nModel.Packages.PackageRemoved  += Packages_Removed;
                    nModel.Packages.PropertyChanged += Interface_PropertyChanged;
                }
                // Removes models that no longer exist.
                for (int i = 0; i < Packages.Count; i++)
                    if (!nModel.Packages.Packages.Contains(Packages[i]))
                        Packages.RemoveAt(i--);
                // Adds models that are new.
                for (int i = 0; i < nModel.Packages.Packages.Count; i++)
                    if (!Packages.Contains(nModel.Packages.Packages[i]))
                        Packages.Add(nModel.Packages.Packages[i]);
            }

            if (sender == nModel.Layer
                && (e.PropertyName == "Hostname"
                    || e.PropertyName == "Port"
                    || e.PropertyName == "Username"
                    || e.PropertyName == "Password"
                    || e.PropertyName == "ConnectionState"))
                OnPropertyChanged(e.PropertyName);
        }
    }
}
