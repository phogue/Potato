using System.Collections.Generic;
using Procon.Net.Protocols.Objects;

namespace Procon.Net.Test.Mocks {

    [GameDeclaration(Type = "MOCK_3", Name = "MockGame 3", Provider = "Myrcon")]
    public class MockGame : Game {
        public MockGame(string hostName, ushort port) : base(hostName, port) {
        }

        protected override void Login(string password) {
            // Do nothing.
        }

        protected override List<IPacket> Action(Chat chat) {
            throw new System.NotImplementedException();
        }

        protected override List<IPacket> Action(Kick kick) {
            throw new System.NotImplementedException();
        }

        protected override List<IPacket> Action(Ban ban) {
            throw new System.NotImplementedException();
        }

        protected override List<IPacket> Action(Map map) {
            throw new System.NotImplementedException();
        }

        protected override List<IPacket> Action(Kill kill) {
            throw new System.NotImplementedException();
        }

        protected override List<IPacket> Action(Move move) {
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
