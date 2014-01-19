using System.Collections.Generic;
using Procon.Net.Shared;
using Procon.Net.Shared.Actions;

namespace Procon.Net.Test.Mocks {

    [ProtocolDeclaration(Type = "MOCK_3", Name = "MockGame 3", Provider = "Myrcon")]
    public class MockGame : Procon.Net.Protocol {
        public MockGame(string hostName, ushort port) : base(hostName, port) {
        }

        protected override void Login(string password) {
            // Do nothing.
        }

        protected override List<IPacketWrapper> DispatchAction(INetworkAction action) {
            return new List<IPacketWrapper>();
        }

        protected override List<IPacketWrapper> ActionChat(INetworkAction action) {
            throw new System.NotImplementedException();
        }

        protected override List<IPacketWrapper> ActionKill(INetworkAction action) {
            throw new System.NotImplementedException();
        }

        protected override List<IPacketWrapper> ActionKick(INetworkAction action) {
            throw new System.NotImplementedException();
        }

        protected override List<IPacketWrapper> ActionBan(INetworkAction action) {
            throw new System.NotImplementedException();
        }

        protected override List<IPacketWrapper> ActionMove(INetworkAction action) {
            throw new System.NotImplementedException();
        }

        protected override List<IPacketWrapper> ActionMap(INetworkAction action) {
            throw new System.NotImplementedException();
        }

        protected override IClient CreateClient(string hostName, ushort port) {
            return new MockTcpClient(hostName, port);
        }

        protected override IPacketWrapper CreatePacket(string format, params object[] args) {
            return new MockPacket() {
                Text = string.Format(format, args)
            };
        }

        protected override IPacketWrapper WrapPacket(IPacket packet) {
            return new MockPacket() {
                Packet = packet
            };
        }
    }
}
