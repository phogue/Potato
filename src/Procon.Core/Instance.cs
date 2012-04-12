// Copyright 2011 Geoffrey 'Phogue' Green
// Modified by Cameron 'Imisnew2' Gunnin
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
using System.Linq;
using System.Xml.Linq;

namespace Procon.Core {
    using Procon.Core.Interfaces;
    using Procon.Core.Interfaces.Layer;

    public class Instance : Executable<Instance>
    {
        // Public Objects
        public  List<Interface> Interfaces
        {
            get { return mInterfaces;  }
            protected set {
                if (mInterfaces != value) {
                    mInterfaces = value;
                    OnPropertyChanged(this, "Interfaces");
                }
            }
        }
        private List<Interface> mInterfaces;

        // Default Initialization
        public Instance() : base() {
            Interfaces = new List<Interface>();
        }



        #region Executable

        /// <summary>
        /// Adds the local interface to the list of interfaces.
        /// Executes the commands specified in the config file and returns a reference itself.
        /// </summary>
        public override Instance Execute()
        {
            Interface local = new LocalInterface().Execute();
            Interfaces.Add(local);
            OnInterfaceAdded(this, local);

            return base.Execute();
        }

        /// <summary>
        /// Disposes of all the Interfaces before calling the base dispose.
        /// </summary>
        public override void Dispose()
        {
            foreach (Interface i in Interfaces)
                i.Dispose();
            base.Dispose();
        }

        /// <summary>
        /// Saves the local layer settings and remote interface information.
        /// </summary>
        protected override void WriteConfig(XElement config, ref FileInfo xFile)
        {
            foreach (Interface @interface in this.Interfaces)
                if (@interface is LocalInterface)
                {
                    LayerListener l = @interface.Layer as LayerListener;
                    config.Add(new XElement("command", // hostname, port, isCompressed, isEncrypted
                        new XAttribute("name", CommandName.InstanceLayerSetup),
                        new XElement("hostname",     l.Hostname),
                        new XElement("port",         l.Port),
                        new XElement("isCompressed", l.IsCompressed),
                        new XElement("isEncrypted",  l.IsEncrypted)));
                }
                else
                {
                    LayerGame l = @interface.Layer as LayerGame;
                    config.Add(new XElement("command", // hostname, port, isCompressed, isEncrypted
                        new XAttribute("name", CommandName.InstanceAddRemoteInterface),
                        new XElement("hostname", l.Hostname),
                        new XElement("port",     l.Port),
                        new XElement("username", l.Username),
                        new XElement("password", l.Password)));
                }
        }

        #endregion
        #region Events

        // -- Adding an Interface --
        public delegate void InterfaceAddedHandler(Instance parent, Interface item);
        public event         InterfaceAddedHandler InterfaceAdded;
        protected void OnInterfaceAdded(Instance parent, Interface item)
        {
            if (InterfaceAdded != null)
                InterfaceAdded(parent, item);
        }
        // -- Removing an Interface --
        public delegate void InterfaceRemovedHandler(Instance parent, Interface item);
        public event         InterfaceRemovedHandler InterfaceRemoved;
        protected void OnInterfaceRemoved(Instance parent, Interface item)
        {
            if (InterfaceRemoved != null)
                InterfaceRemoved(parent, item);
        }

        #endregion



        /// <summary>
        /// Finds an instance of the local interface in the Interfaces list, and sets the
        /// layer-specific values of the local interface.  Does nothing if local interface
        /// could not be found.
        /// </summary>
        [Command(Command = CommandName.InstanceLayerSetup)]
        public LocalInterface SetupLayer(CommandInitiator initiator, String hostname, UInt16 port, Boolean isCompressed = false, Boolean isEncrypted = false)
        {
            Interface local = Interfaces
                                  .Where(x => x is LocalInterface)
                                  .FirstOrDefault();
            if (local != null)
            {
                local.Layer.Hostname     = hostname;
                local.Layer.Port         = port;
                local.Layer.IsEncrypted  = isEncrypted;
                local.Layer.IsCompressed = isCompressed;
            }
            return local as LocalInterface;
        }

        /// <summary>
        /// Creates an instance of a remote interface and adds it to the Interfaces list.
        /// Sets the interface's language and arguments to the same as the instance's.
        /// </summary>
        [Command(Command = CommandName.InstanceAddRemoteInterface)]
        public RemoteInterface CreateRemoteInterface(CommandInitiator initiator, String hostname, UInt16 port, String username, String password)
        {
            Interface remote = new RemoteInterface(hostname, port, username, password).Execute();

            Interfaces.Add(remote);
            OnInterfaceAdded(this, remote);

            return remote as RemoteInterface;
        }

        /// <summary>
        /// Removes a remote interface from the Interfaces list whose hostname and port
        /// match those passed in.
        /// </summary>
        [Command(Command = CommandName.InstanceRemoveRemoteInterface)]
        public RemoteInterface DestroyRemoteInterface(CommandInitiator initiator, String hostname, UInt16 port)
        {
            Interface remote = Interfaces
                                   .Where(x => x.Layer.Hostname == hostname && x.Layer.Port == port)
                                   .FirstOrDefault();
            if (remote != null)
            {
                remote.Dispose();
                Interfaces.Remove(remote);
                OnInterfaceRemoved(this, remote);
            }
            return remote as RemoteInterface;
        }
    }
}
