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
using System.Windows.Threading;

using Procon.Core;
using Procon.Core.Interfaces;

namespace Procon.UI.API.ViewModels
{
    /// <summary>Wraps the Instance of Procon so that it can be used in the UI.</summary>
    public class InstanceViewModel : ViewModel<Instance>
    {
        // View Model Properties
        public  ObservableCollection<InterfaceViewModel> Interfaces
        {
            get { return mInterfaces; }
            protected set {
                if (mInterfaces != value) {
                    mInterfaces = value;
                    OnPropertyChanged(this, "Interfaces");
                }
            }
        }
        private ObservableCollection<InterfaceViewModel> mInterfaces;

        /// <summary>Creates an instance of InstanceViewModel and initalizes its properties.</summary>
        /// <param name="model">A reference to the current instance of procon.</param>
        public InstanceViewModel(Instance model) : base(model)
        {
            // Listen for changes within the model:
            Model.InterfaceAdded   += Interfaces_Added;
            Model.InterfaceRemoved += Interfaces_Removed;
            Model.PropertyChanged  += Instance_PropertyChanged;

            // Expose collections within the model:
            Interfaces = new ObservableCollection<InterfaceViewModel>(Model.Interfaces.Select(x => new InterfaceViewModel(x)));
        }



        /// <summary>
        /// Begins the execution of Procon.Core.
        /// </summary>
        public void Execute()  { Model.Execute(); }
        /// <summary>
        /// Ends the execution of Procon.Core.
        /// </summary>
        public void Shutdown() { Model.Dispose(); }



        /// <summary>
        /// Attempts to add an interface to the instance.
        /// </summary>
        public void AddInterface(String hostname, UInt16 port, String username, String password)
        {
            Model.CreateRemoteInterface(
                CommandInitiator.Local,
                hostname,
                port,
                username,
                password);
        }
        /// <summary>
        /// Attempts to remove an interface from the instance.
        /// </summary>
        public void RemoveInterface(String hostname, UInt16 port)
        {
            Model.DestroyRemoteInterface(
                CommandInitiator.Local,
                hostname,
                port);
        }



        /// <summary>
        /// Lets the UI's view model know we added an interface.
        /// </summary>
        private void Interfaces_Added(Instance parent, Interface item)
        {
            // Force the UI thread to execute this method.
            if (Dispatcher.CurrentDispatcher != MainQueue) {
                MainQueue.Invoke(new System.Action(() => Interfaces_Added(parent, item)));
                return;
            }

            // Add the new interface.
            InterfaceViewModel tTemp = new InterfaceViewModel(item);
            Interfaces.Add(tTemp);
            OnInterfaceAdded(tTemp);
        }
        /// <summary>
        /// Lets the UI's view model know we removed an interface.
        /// </summary>
        private void Interfaces_Removed(Instance parent, Interface item)
        {
            // Force the UI thread to execute this method.
            if (Dispatcher.CurrentDispatcher != MainQueue) {
                MainQueue.Invoke(new System.Action(() => Interfaces_Removed(parent, item)));
                return;
            }

            // Remove the old interface.
            for (int i = 0; i < Interfaces.Count; i++)
                if (Interfaces[i].ModelEquals(item)) {
                    InterfaceViewModel tTemp = Interfaces[i];
                    Interfaces.Remove(tTemp);
                    OnInterfaceRemoved(tTemp);
                    break;
                }
        }
        /// <summary>
        /// Lets the UI's view model know a property in the model changed.
        /// </summary>
        private void Instance_PropertyChanged(Object sender, PropertyChangedEventArgs e)
        {
            // Force the UI thread to execute this method.
            if (Dispatcher.CurrentDispatcher != MainQueue) {
                MainQueue.Invoke(new System.Action(() => Instance_PropertyChanged(sender, e)));
                return;
            }

            // Interfaces collection was re-set?
            if (e.PropertyName == "Interfaces") {
                Boolean exists;
                // Removes models that no longer exist.
                for (int i = 0; i < Interfaces.Count; i++) {
                    exists = false;
                    for (int j = 0; j < Model.Interfaces.Count; j++)
                        if (exists = Interfaces[i].ModelEquals(Model.Interfaces[j]))
                            break;
                    if (!exists) Interfaces.RemoveAt(i--);
                }
                // Adds models that are new.
                for (int i = 0; i < Model.Interfaces.Count; i++) {
                    exists = false;
                    for (int j = 0; j < Interfaces.Count; j++)
                        if (exists = Interfaces[j].ModelEquals(Model.Interfaces[i]))
                            break;
                    if (exists) Interfaces.Add(new InterfaceViewModel(Model.Interfaces[i]));
                }
            }
            // Other.
            else OnPropertyChanged(this, e.PropertyName);
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
