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
        // Public Accessors/Mutators.
        public List<Interface> Interfaces
        {
            get { return mInterfaces;  }
            protected set {
                if (mInterfaces != value) {
                    mInterfaces = value;
                    OnPropertyChanged(this, "Interfaces");
        } } }

        // Private Variables.
        private List<Interface> mInterfaces;


        // Constructor.
        public Instance() : base() {
            Interfaces = new List<Interface>();
        }

        // Execute:
        // -- Creates the local interface.
        // -- Loads the configuration file.
        public override Instance Execute()
        {
            Interface local = new LocalInterface().Execute();
            Interfaces.Add(local);
            OnInterfaceAdded(this, local);

            return base.Execute();
        }
        // Dispose:
        // -- Disposes of all it's interfaces.
        // -- Requests the base class to dispose itself.
        public override void Dispose()
        {
            foreach (Interface i in Interfaces)
                i.Dispose();
            base.Dispose();
        }
        // WriteConfig:
        // -- Saves all the interfaces to the config file.
        protected override void WriteConfig(XElement config)
        {
            foreach (Interface @interface in Interfaces)
                if (@interface is LocalInterface) {
                    LayerListener l = @interface.Layer as LayerListener;
                    config.Add(new XElement("command", // hostname, port, isCompressed, isEncrypted
                        new XAttribute("name", CommandName.InstanceLayerSetup),
                        new XElement("hostname",     l.Hostname),
                        new XElement("port",         l.Port),
                        new XElement("isCompressed", l.IsCompressed),
                        new XElement("isEncrypted",  l.IsEncrypted)));
                }
                else {
                    LayerGame l = @interface.Layer as LayerGame;
                    config.Add(new XElement("command", // hostname, port, isCompressed, isEncrypted
                        new XAttribute("name", CommandName.InstanceAddRemoteInterface),
                        new XElement("hostname", l.Hostname),
                        new XElement("port",     l.Port),
                        new XElement("username", l.Username),
                        new XElement("password", l.Password)));
                }
        }


        // Manage the interfaces.
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
        [Command(Command = CommandName.InstanceAddRemoteInterface)]
        public RemoteInterface CreateRemoteInterface(CommandInitiator initiator, String hostname, UInt16 port, String username, String password)
        {
            Interface remote = new RemoteInterface(hostname, port, username, password).Execute();

            Interfaces.Add(remote);
            OnInterfaceAdded(this, remote);

            return remote as RemoteInterface;
        }
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


        // Events
        public delegate void InterfaceHandler(Instance parent, Interface item);
        public event InterfaceHandler InterfaceAdded;
        public event InterfaceHandler InterfaceRemoved;
        protected void OnInterfaceAdded(Instance parent, Interface item)
        {
            if (InterfaceAdded != null)
                InterfaceAdded(parent, item);
        }
        protected void OnInterfaceRemoved(Instance parent, Interface item)
        {
            if (InterfaceRemoved != null)
                InterfaceRemoved(parent, item);
        }
    }
}
