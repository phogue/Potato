using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

using Procon.Core;
using Procon.Core.Interfaces;
using Procon.Core.Interfaces.Connections;
using Procon.Core.Interfaces.Layer;
using Procon.Core.Interfaces.Packages;
using Procon.Net;
using Procon.Net.Protocols.Objects;

namespace Procon.UI.API.ViewModels
{
    // Wraps an Interface
    public class InterfaceViewModel : ViewModel<Interface>
    {
        // View Model Public Accessors/Mutators.
        public String Hostname
        {
            get { return Model.Layer.Hostname; }
            set { Model.Layer.Hostname = value; }
        }
        public UInt16 Port
        {
            get { return Model.Layer.Port; }
            set { Model.Layer.Port = value; }
        }
        public String Username
        {
            get { return (Model.Layer is LayerGame) ? (Model.Layer as LayerGame).Username : null; }
            set { if (Model.Layer is LayerGame) (Model.Layer as LayerGame).Username = value; }
        }
        public String Password
        {
            get { return (Model.Layer is LayerGame) ? (Model.Layer as LayerGame).Password : null; }
            set { if (Model.Layer is LayerGame) (Model.Layer as LayerGame).Password = value; }
        }
        public ConnectionState ConnectionState
        {
            get { return Model.Layer.ConnectionState; }
        }
        
        public ObservableCollection<ConnectionViewModel> Connections
        {
            get { return mConnections; }
            protected set {
                if (mConnections != value) {
                    mConnections = value;
                    OnPropertyChanged(this, "Connections");
        } } }
        public ObservableCollection<PackageViewModel>    Packages
        {
            get { return mPackages; }
            protected set {
                if (mPackages != value) {
                    mPackages = value;
                    OnPropertyChanged(this, "Packages");
        } } }
        public ObservableCollection<DataVariable>        Variables
        {
            get { return mVariables; }
            protected set {
                if (mVariables != value) {
                    mVariables = value;
                    OnPropertyChanged(this, "Variables");
        } } }

        // View Model Private Variables.
        private ObservableCollection<ConnectionViewModel> mConnections;
        private ObservableCollection<PackageViewModel>    mPackages;
        private ObservableCollection<DataVariable>        mVariables;

        // Custom Properties
        public Boolean IsLocal
        {
            get { return Model is LocalInterface; }
        }


        // Constructor.
        public InterfaceViewModel(Interface model) : base(model)
        {
            // Listen for changes within the model:
            Model.ConnectionAdded          += Connections_Added;
            Model.ConnectionRemoved        += Connections_Removed;
            Model.Packages.PackageAdded    += Packages_Added;
            Model.Packages.PackageRemoved  += Packages_Removed;
            Model.PropertyChanged          += Interface_PropertyChanged;
            Model.Layer.PropertyChanged    += Interface_PropertyChanged;
            Model.Packages.PropertyChanged += Interface_PropertyChanged;

            // Expose collections within the model:
            Connections = new ObservableCollection<ConnectionViewModel>(Model.Connections.Select(x => new ConnectionViewModel(x)));
            Packages    = new ObservableCollection<PackageViewModel>(Model.Packages.Packages.Select(x => new PackageViewModel(x)));
        }
        
        // View Model Methods.
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
        public void RemoveConnection(String gameType, String hostname, UInt16 port)
        {
            Model.RemoveConnection(
                CommandInitiator.Local,
                gameType,
                hostname,
                port);
        }


        // Wraps the ConnectionAdded/ConnectionRemoved events.
        private void Connections_Added(Interface parent, Connection item)
        {
            // Force the UI thread to execute this method.
            if (ChangeDispatcher(() => Connections_Added(parent, item)))
                return;

            // Add the new connection.
            ConnectionViewModel tViewModel = new ConnectionViewModel(item);
            Connections.Add(tViewModel);
            OnConnectionAdded(tViewModel);
        }
        private void Connections_Removed(Interface parent, Connection item)
        {
            // Force the UI thread to execute this method.
            if (ChangeDispatcher(() => Connections_Removed(parent, item)))
                return;

            // Remove the old connection.
            ConnectionViewModel tViewModel = Connections.SingleOrDefault(x => x.ModelEquals(item));
            Connections.Remove(tViewModel);
            OnConnectionRemoved(tViewModel);
        }

        // Wraps the Packages.PackageAdded/Packages.PackageRemoved events.
        private void Packages_Added(PackageController parent, Package item)
        {
            // Force the UI thread to execute this method.
            if (ChangeDispatcher(() => Packages_Added(parent, item)))
                return;

            // Add the new connection.
            PackageViewModel tViewModel = new PackageViewModel(item);
            Packages.Add(tViewModel);
            OnPackageAdded(tViewModel);
        }
        private void Packages_Removed(PackageController parent, Package item)
        {
            // Force the UI thread to execute this method.
            if (ChangeDispatcher(() => Packages_Removed(parent, item)))
                return;

            // Add the new connection.
            PackageViewModel tViewModel = Packages.SingleOrDefault(x => x.ModelEquals(item));
            Packages.Add(tViewModel);
            OnPackageAdded(tViewModel);
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
                    if (Model.Connections.SingleOrDefault(x => Connections[i].ModelEquals(x)) == null)
                        Connections.RemoveAt(i--);
                // Adds models that are new.
                for (int i = 0; i < Model.Connections.Count; i++)
                    if (Connections.SingleOrDefault(x => x.ModelEquals(Model.Connections[i])) == null)
                        Connections.Add(new ConnectionViewModel(Model.Connections[i]));
            }

            if (e.PropertyName == "Packages") {
                // Re-set the event listeners.
                if (sender == Model) {
                    Model.Packages.PackageAdded    += Packages_Added;
                    Model.Packages.PackageRemoved  += Packages_Removed;
                    Model.Packages.PropertyChanged += Interface_PropertyChanged;
                }
                // Removes models that no longer exist.
                for (int i = 0; i < Packages.Count; i++)
                    if (Model.Packages.Packages.SingleOrDefault(x => Packages[i].ModelEquals(x)) == null)
                        Packages.RemoveAt(i--);
                // Adds models that are new.
                for (int i = 0; i < Model.Packages.Packages.Count; i++)
                    if (Packages.SingleOrDefault(x => x.ModelEquals(Model.Packages.Packages[i])) == null)
                        Connections.Add(new ConnectionViewModel(Model.Connections[i]));
            }

            if (sender == Model.Layer)
                if (e.PropertyName == "Hostname" || e.PropertyName == "Port"     ||
                    e.PropertyName == "Username" || e.PropertyName == "Password" ||
                    e.PropertyName == "ConnectionState")
                    OnPropertyChanged(this, e.PropertyName);
        }


        // Events.
        public delegate void ConnectionHandler(InterfaceViewModel parent, ConnectionViewModel item);
        public delegate void PackageHandler(InterfaceViewModel parent, PackageViewModel item);
        public event ConnectionHandler ConnectionAdded;
        public event ConnectionHandler ConnectionRemoved;
        public event PackageHandler PackageAdded;
        public event PackageHandler PackageRemoved;
        public void OnConnectionAdded(ConnectionViewModel i)
        {
            if (ConnectionAdded != null)
                ConnectionAdded(this, i);
        }
        public void OnConnectionRemoved(ConnectionViewModel i)
        {
            if (ConnectionRemoved != null)
                ConnectionRemoved(this, i);
        }
        public void OnPackageAdded(PackageViewModel i)
        {
            if (PackageAdded != null)
                PackageAdded(this, i);
        }
        public void OnPackageRemoved(PackageViewModel i)
        {
            if (PackageRemoved != null)
                PackageRemoved(this, i);
        }
    }
}
