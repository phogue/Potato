using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

using Procon.Core;
using Procon.Core.Interfaces;
using Procon.UI.API.Utils;

namespace Procon.UI.API.ViewModels
{
    public class InstanceViewModel : ViewModel<Instance>
    {
        // Observable Properties.
        public  NotifiableCollection<InterfaceViewModel> Interfaces
        {
            get { return mInterfaces; }
            protected set {
                if (mInterfaces != value) {
                    mInterfaces = value;
                    OnPropertyChanged("Interfaces");
        } } }
        private NotifiableCollection<InterfaceViewModel> mInterfaces;

        
        // Constructor.
        public InstanceViewModel(Instance model) : base(model)
        {
            // Listen for changes within the model:
            nModel.InterfaceAdded   += Interfaces_Added;
            nModel.InterfaceRemoved += Interfaces_Removed;
            nModel.PropertyChanged  += Instance_PropertyChanged;

            // Expose collections within the model:
            Interfaces = new NotifiableCollection<InterfaceViewModel>(nModel.Interfaces.Select(x => new InterfaceViewModel(x)));
        }


        // View Model Methods.
        public void Execute()
        {
            nModel.Execute();
        }
        public void Shutdown()
        {
            nModel.Dispose();
        }
        public void ExecuteLayer()
        {
            Interface local = nModel.Interfaces
                                  .Where(x => x is LocalInterface)
                                  .FirstOrDefault();
            local.Layer.Begin();
        }
        public void ShutdownLayer()
        {
            Interface local = nModel.Interfaces
                                  .Where(x => x is LocalInterface)
                                  .FirstOrDefault();
            local.Layer.Shutdown();
        }
        public void CreateInterface(String hostname, UInt16 port, String username, String password)
        {
            nModel.CreateRemoteInterface(
                CommandInitiator.Local,
                hostname,
                port,
                username,
                password);
        }
        public void DestroyInterface(String hostname, UInt16 port)
        {
            nModel.DestroyRemoteInterface(
                CommandInitiator.Local,
                hostname,
                port);
        }


        // Wraps the Added & Removed events.
        private void Interfaces_Added(Instance parent, Interface item)
        {
            // Force the UI thread to execute this method.
            if (ChangeDispatcher(() => Interfaces_Added(parent, item)))
                return;

            // Add the new interface.
            InterfaceViewModel tViewModel = new InterfaceViewModel(item);
            Interfaces.Add(tViewModel);
        }
        private void Interfaces_Removed(Instance parent, Interface item)
        {
            // Force the UI thread to execute this method.
            if (ChangeDispatcher(() => Interfaces_Removed(parent, item)))
                return;

            // Remove the old interface.
            InterfaceViewModel tViewModel = Interfaces.Single(x => x.ModelEquals(item));
            Interfaces.Remove(tViewModel);
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
                    if (nModel.Interfaces.SingleOrDefault(x => Interfaces[i].ModelEquals(x)) == null)
                        Interfaces.RemoveAt(i--);
                // Adds models that are new.
                for (int i = 0; i < nModel.Interfaces.Count; i++)
                    if (Interfaces.SingleOrDefault(x => x.ModelEquals(nModel.Interfaces[i])) == null)
                        Interfaces.Add(new InterfaceViewModel(nModel.Interfaces[i]));
            }
        }
    }
}
