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
            this.Client = new MockClient();
            this.ProtocolType = new ProtocolType() {
                Name = String.Empty,
                Type = String.Empty,
                Provider = String.Empty
            };
        }

        public IClient Client { get; private set; }
        public ProtocolState State { get; private set; }
        public string Password { get; set; }
        public string Additional { get; set; }
        public IProtocolType ProtocolType { get; private set; }
        public string ProtocolsConfigDirectory { get; set; }
        public event Action<IProtocol, ProtocolEventArgs> ProtocolEvent;
        public event Action<IProtocol, ClientEventArgs> ClientEvent;
        public List<IPacket> Action(NetworkAction action) {
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
