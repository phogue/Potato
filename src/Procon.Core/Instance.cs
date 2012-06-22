using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Procon.Core {
    using Procon.Core.Interfaces;
    using Procon.Core.Interfaces.Layer;
    using Procon.Core.Utils;

    public class Instance : Executable<Instance>
    {
        // Public Accessors/Mutators.
        public List<Interface> Interfaces {
            get { return mInterfaces;  }
            protected set {
                if (mInterfaces != value) {
                    mInterfaces = value;
                    OnPropertyChanged(this, "Interfaces");
        } } }
        // Internal Variables.
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
        // -- Preps and saves the settings for this instance of procon.
        // -- Disposes of all interfaces.
        public override void Dispose()
        {
            Config mConfig = new Config().Generate(GetType());
            WriteConfig(mConfig);
            foreach (Interface i in Interfaces)
                i.Dispose();
            mConfig.Save(new FileInfo(Path.Combine(Defines.CONFIGS_DIRECTORY, String.Format("{0}.xml", GetType().Namespace))));
            Interfaces.Clear();
        }
        // WriteConfig:
        // -- Saves all the interfaces to the config file.
        internal override void WriteConfig(Config config)
        {
            foreach (Interface tInterface in Interfaces)
                if (tInterface is LocalInterface) {
                    LayerListener tLayer  = tInterface.Layer as LayerListener;
                    Config        tConfig = new Config().Generate(tInterface.GetType());
                    config.Root.Add(new XElement("command", // hostname, port, isCompressed, isEncrypted
                        new XAttribute("name", CommandName.InstanceLayerSetup),
                        new XElement("hostname",     tLayer.Hostname),
                        new XElement("port",         tLayer.Port),
                        new XElement("isCompressed", tLayer.IsCompressed),
                        new XElement("isEncrypted",  tLayer.IsEncrypted)));
                    tInterface.WriteConfig(tConfig);
                    config.Add(tConfig);
                }
                else {
                    LayerGame tLayer  = tInterface.Layer as LayerGame;
                    config.Root.Add(new XElement("command", // hostname, port, username, password
                        new XAttribute("name", CommandName.InstanceAddRemoteInterface),
                        new XElement("hostname", tLayer.Hostname),
                        new XElement("port",     tLayer.Port),
                        new XElement("username", tLayer.Username),
                        new XElement("password", tLayer.Password)));
                }
        }


        // Manage the interfaces.
        [Command(Command = CommandName.InstanceLayerSetup)]
        public LocalInterface SetupLayer(CommandInitiator initiator, String hostname, UInt16 port, Boolean isCompressed = false, Boolean isEncrypted = false)
        {
            Interface local = Interfaces
                                  .Where(x => x is LocalInterface)
                                  .FirstOrDefault();
            if (local != null) {
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
            Interface remote = Interfaces
                                   .Where(x => x.Layer.Hostname == hostname && x.Layer.Port == port)
                                   .FirstOrDefault();
            if (remote == null) {
                remote = new RemoteInterface(hostname, port, username, password).Execute();
                Interfaces.Add(remote);
                OnInterfaceAdded(this, remote);
            }
            return remote as RemoteInterface;
        }
        [Command(Command = CommandName.InstanceRemoveRemoteInterface)]
        public RemoteInterface DestroyRemoteInterface(CommandInitiator initiator, String hostname, UInt16 port)
        {
            Interface remote = Interfaces
                                   .Where(x => x.Layer.Hostname == hostname && x.Layer.Port == port)
                                   .FirstOrDefault();
            if (remote != null) {
                Interfaces.Remove(remote);
                OnInterfaceRemoved(this, remote);
                remote.Dispose();
            }
            return remote as RemoteInterface;
        }


        // Events.
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
