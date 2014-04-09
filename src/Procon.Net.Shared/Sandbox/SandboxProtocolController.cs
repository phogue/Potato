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

        public SandboxProtocolCallback Bubble { get; set; }

        /// <summary>
        /// The protocol instance loaded in the sandboxed appdomain.
        /// </summary>
        public IProtocol SandboxedProtocol { get; set; }

        public IClient Client {
            get {
                return this.SandboxedProtocol != null ? this.SandboxedProtocol.Client : null;
            }
        }

        public IProtocolState State {
            get {
                return this.SandboxedProtocol != null ? this.SandboxedProtocol.State : null;
            }
        }

        public IProtocolSetup Options {
            get {
                return this.SandboxedProtocol != null ? this.SandboxedProtocol.Options : null;
            }
        }

        public IProtocolType ProtocolType {
            get {
                return this.SandboxedProtocol != null ? this.SandboxedProtocol.ProtocolType : null;
            }
        }

        /// <summary>
        /// Assigns events from the sandboxed protocol to bubble into the bubble object, provided
        /// an object has beeen set and a delegate added to the bubble object.
        /// </summary>
        public void AssignEvents() {
            if (this.SandboxedProtocol != null) {
                this.SandboxedProtocol.ProtocolEvent += (protocol, args) => {
                    if (this.Bubble != null && this.Bubble.ProtocolEvent != null) {
                        this.Bubble.ProtocolEvent(this, args);
                    }
                };

                this.SandboxedProtocol.ClientEvent += (protocol, args) => {
                    if (this.Bubble != null && this.Bubble.ClientEvent != null) {
                        this.Bubble.ClientEvent(this, args);
                    }
                };
            }
        }

        public bool Create(String assemblyFile, IProtocolType type) {
            this.SandboxedProtocol = null;

            try {
                // Load the assembly into our AppDomain
                var loaded = Assembly.LoadFile(assemblyFile);

                // Fetch a list of available game types by their attributes
                var protocolType = loaded.GetTypes()
                    .Where(loadedType => typeof(IProtocol).IsAssignableFrom(loadedType))
                    .First(loadedType => {
                        var firstOrDefault = loadedType.GetCustomAttributes(typeof (IProtocolType), false).Cast<IProtocolType>().FirstOrDefault();
                        return firstOrDefault != null && String.Equals(firstOrDefault.Provider, type.Provider) && String.Equals(firstOrDefault.Type, type.Type);
                    });

                this.SandboxedProtocol = (IProtocol)Activator.CreateInstance(protocolType);

                this.AssignEvents();
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

            return this.SandboxedProtocol != null;
        }

        public IProtocolSetupResult Setup(IProtocolSetup setup) {
            return this.SandboxedProtocol != null ? this.SandboxedProtocol.Setup(setup) : null;
        }

        public List<IPacket> Action(INetworkAction action) {
            return this.SandboxedProtocol != null ? this.SandboxedProtocol.Action(action) : null;
        }

        public IPacket Send(IPacketWrapper packet) {
            return this.SandboxedProtocol != null ? this.SandboxedProtocol.Send(packet) : null;
        }

        public void AttemptConnection() {
            if (this.SandboxedProtocol != null) {
                this.SandboxedProtocol.AttemptConnection();
            }
        }

        public void Shutdown() {
            if (this.SandboxedProtocol != null) {
                this.SandboxedProtocol.Shutdown();

                this.SandboxedProtocol = null;
            }
        }

        public void Synchronize() {
            if (this.SandboxedProtocol != null) {
                this.SandboxedProtocol.Synchronize();
            }
        }
    }
}