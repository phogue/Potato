using System;
using System.Collections.Generic;
using Procon.Net;
using Procon.Net.Shared;
using Procon.Net.Shared.Actions;
using Procon.Net.Shared.Models;

namespace Procon.Core.Test.Mocks.Protocols {
    [ProtocolDeclaration(Type = "MockProtocol", Name = "Mock Protocol 3", Provider = "Myrcon")]
    public class MockProtocol : IProtocol {

        public MockProtocol() {
            this.State = new ProtocolState();
            this.Client = new MockClient(String.Empty, 0);
            this.ProtocolType = new ProtocolType() {
                Name = "Mock Protocol 3",
                Type = "MockProtocol",
                Provider = "Myrcon"
            };
        }

        public MockProtocol(string hostName, ushort port) : this() {
            this.Client = new MockClient(hostName, port);
        }

        public IClient Client { get; private set; }
        public ProtocolState State { get; private set; }
        public string Password { get; set; }
        public string Additional { get; set; }
        public IProtocolType ProtocolType { get; private set; }
        public string ProtocolsConfigDirectory { get; set; }
        public event Action<IProtocol, ProtocolEventArgs> ProtocolEvent;
        public event Action<IProtocol, IClientEventArgs> ClientEvent;
        public List<IPacket> Action(INetworkAction action) {
            return null;
        }

        public IPacket Send(IPacketWrapper packet) {
            return null;
        }

        public void AttemptConnection() {
            
        }

        public void Shutdown() {
            
        }

        public void Synchronize() {
            
        }
    }
}
