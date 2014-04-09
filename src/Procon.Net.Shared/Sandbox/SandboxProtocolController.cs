using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using Procon.Net.Shared.Actions;

namespace Procon.Net.Shared.Sandbox {
    /// <summary>
    /// Controller to run on the sandbox side of the AppDomain. This acts as the proxy
    /// so anything outside of the Sandbox namespace does not need to worry about sandboxing at
    /// all. It will just happen with the available protocols. Allows others to build rcon tools
    /// with Procon as the underlying library, but not worry about sandboxing if they dont want to.
    /// </summary>
    public class SandboxProtocolController : MarshalByRefObject, ISandboxProtocolController {
        /// <summary>
        /// The protocol instance loaded in the sandboxed appdomain.
        /// </summary>
        public IProtocol SandboxedProtocol { get; set; }

        public IClient Client { get; private set; }
        public IProtocolState State { get; private set; }
        public IProtocolSetup Options { get; private set; }
        public IProtocolType ProtocolType { get; private set; }
        public string ProtocolsConfigDirectory { get; set; }
        public event Action<IProtocol, IProtocolEventArgs> ProtocolEvent;
        public event Action<IProtocol, IClientEventArgs> ClientEvent;

        public IProtocol Create(String assemblyFile, IProtocolType type) {
            this.SandboxedProtocol = null;

            try {
                // Load the assembly into our AppDomain
                var loaded = Assembly.LoadFile(assemblyFile);

                // Fetch a list of available game types by their attributes
                var protocolType = loaded.GetTypes()
                    .Where(loadedType => typeof(IProtocol).IsAssignableFrom(loadedType))
                    .First(loadedType => (loadedType.GetCustomAttributes(typeof(IProtocolType), false)).Cast<IProtocolType>().FirstOrDefault() == type);

                this.SandboxedProtocol = (IProtocol)Activator.CreateInstance(protocolType);
            }
            // [Obviously copy/pasted from the plugin controller, it has the same meaning here]
            // We don't do any exception logging here, as simply updating Procon may log a bunch of exceptions
            // for plugins that are deprecated or simply forgotten about by the user.
            // The exceptions wouldn't be terribly detailed anyway, it would just specify that a fault occured
            // while loading the assembly/type and ultimately the original developer needs to fix something.
            // I would also hope that beyond Beta we will not make breaking changes to the plugin interface,
            // differing from Procon 1 in generic behaviour for IPluginController/ICoreController
            catch {
                this.SandboxedProtocol = null;
            }

            return this;
        }

        public void Setup(IProtocolSetup setup) {
            throw new NotImplementedException();
        }

        public List<IPacket> Action(INetworkAction action) {
            throw new NotImplementedException();
        }

        public IPacket Send(IPacketWrapper packet) {
            throw new NotImplementedException();
        }

        public void AttemptConnection() {
            throw new NotImplementedException();
        }

        void ISandboxProtocolController.Shutdown() {
            throw new NotImplementedException();
        }

        public bool TryEnablePlugin(Guid pluginGuid) {
            throw new NotImplementedException();
        }

        public bool TryDisablePlugin(Guid pluginGuid) {
            throw new NotImplementedException();
        }

        public void GameEvent(List<IProtocolEventArgs> items) {
            throw new NotImplementedException();
        }

        public bool IsPluginEnabled(Guid pluginGuid) {
            throw new NotImplementedException();
        }

        void IProtocol.Shutdown() {
            throw new NotImplementedException();
        }

        public void Synchronize() {
            throw new NotImplementedException();
        }
    }
}