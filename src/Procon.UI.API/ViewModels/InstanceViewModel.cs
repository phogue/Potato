using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

using Procon.Core;
using Procon.Core.Interfaces;

namespace Procon.UI.API.ViewModels
{
    // Wraps an Instance
    public class InstanceViewModel : ViewModel<Instance>
    {
        // View Model Public Accessors/Mutators.
        public ObservableCollection<InterfaceViewModel> Interfaces
        {
            get { return mInterfaces; }
            protected set {
                if (mInterfaces != value) {
                    mInterfaces = value;
                    OnPropertyChanged(this, "Interfaces");
        } } }

        // View Model Private Variables.
        private ObservableCollection<InterfaceViewModel> mInterfaces;

        
        // Constructor.
        public InstanceViewModel(Instance model) : base(model)
        {
            // Listen for changes within the model:
            Model.InterfaceAdded   += Interfaces_Added;
            Model.InterfaceRemoved += Interfaces_Removed;
            Model.PropertyChanged  += Instance_PropertyChanged;

            // Expose collections within the model:
            Interfaces = new ObservableCollection<InterfaceViewModel>(Model.Interfaces.Select(x => new InterfaceViewModel(x)));
        }

        // View Model Methods.
        public void Execute()
        {
            Model.Execute();
        }
        public void Shutdown()
        {
            Model.Dispose();
        }
        public void CreateInterface(String hostname, UInt16 port, String username, String password)
        {
            Model.CreateRemoteInterface(
                CommandInitiator.Local,
                hostname,
                port,
                username,
                password);
        }
        public void DestroyInterface(String hostname, UInt16 port)
        {
            Model.DestroyRemoteInterface(
                CommandInitiator.Local,
                hostname,
                port);
        }


        // Wraps the InterfaceAdded/InterfaceRemoved events.
        private void Interfaces_Added(Instance parent, Interface item)
        {
            // Force the UI thread to execute this method.
            if (ChangeDispatcher(() => Interfaces_Added(parent, item)))
                return;

            // Add the new interface.
            InterfaceViewModel tViewModel = new InterfaceViewModel(item);
            Interfaces.Add(tViewModel);
            OnInterfaceAdded(tViewModel);
        }
        private void Interfaces_Removed(Instance parent, Interface item)
        {
            // Force the UI thread to execute this method.
            if (ChangeDispatcher(() => Interfaces_Removed(parent, item)))
                return;

            // Remove the old interface.
            InterfaceViewModel tViewModel = Interfaces.SingleOrDefault(x => x.ModelEquals(item));
            Interfaces.Remove(tViewModel);
            OnInterfaceRemoved(tViewModel);
        }

        // Wraps the Instances's property changed events.
        private void Instance_PropertyChanged(Object sender, PropertyChangedEventArgs e)
        {
            // Force the UI thread to execute this method.
            if (ChangeDispatcher(() => Instance_PropertyChanged(sender, e)))
                return;

            if (e.PropertyName == "Interfaces") {
                // Removes models that no longer exist.
                for (int i = 0; i < Interfaces.Count; i++)
                    if (Model.Interfaces.SingleOrDefault(x => Interfaces[i].ModelEquals(x)) == null)
                        Interfaces.RemoveAt(i--);
                // Adds models that are new.
                for (int i = 0; i < Model.Interfaces.Count; i++)
                    if (Interfaces.SingleOrDefault(x => x.ModelEquals(Model.Interfaces[i])) == null)
                        Interfaces.Add(new InterfaceViewModel(Model.Interfaces[i]));
            }
        }


        // Events.
        public delegate void InterfaceHandler(InstanceViewModel parent, InterfaceViewModel item);
        public event InterfaceHandler InterfaceAdded;
        public event InterfaceHandler InterfaceRemoved;
        protected void OnInterfaceAdded(InterfaceViewModel i)
        {
            if (InterfaceAdded != null)
                InterfaceAdded(this, i);
        }
        protected void OnInterfaceRemoved(InterfaceViewModel i)
        {
            if (InterfaceRemoved != null)
                InterfaceRemoved(this, i);
        }
    }
}
