using System;
using System.Collections.Generic;
using Procon.Net;
using Procon.Net.Shared;
using Procon.Net.Shared.Actions;

namespace Myrcon.Protocols.Test {
    [ProtocolDeclaration(Type = "MyrconTestProtocol8", Name = "Test Protocol 8", Provider = "Myrcon")]
    public class TestProtocol : IProtocol {
        public IClient Client { get; private set; }
        public IProtocolState State { get; private set; }
        public IProtocolSetup Options { get; private set; }
        public IProtocolType ProtocolType { get; private set; }
        public string ProtocolsConfigDirectory { get; set; }
        public event Action<IProtocol, IProtocolEventArgs> ProtocolEvent;
        public event Action<IProtocol, IClientEventArgs> ClientEvent;
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

        public void Shutdown() {
            throw new NotImplementedException();
        }

        public void Synchronize() {
            throw new NotImplementedException();
        }
    }
}
