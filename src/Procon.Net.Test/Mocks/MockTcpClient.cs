using Procon.Net.Shared;

namespace Procon.Net.Test.Mocks {
    public class MockTcpClient : TcpClient {

        public MockTcpClient() : base() {

            this.PacketSerializer = new MockPacketSerializer();
        }

        public MockTcpClient(System.Net.Sockets.TcpClient client) : base() {
            this.Client = client;
            this.Stream = client.GetStream();
            this.ConnectionState = ConnectionState.ConnectionLoggedIn;

            this.PacketSerializer = new MockPacketSerializer();
        }
    }
}
