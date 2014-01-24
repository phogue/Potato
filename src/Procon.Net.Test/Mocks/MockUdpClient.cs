using Procon.Net.Shared;

namespace Procon.Net.Test.Mocks {
    public class MockUdpClient : Procon.Net.UdpClient {
        
        public MockUdpClient() : base() {

            this.PacketSerializer = new MockPacketSerializer();
        }

        public MockUdpClient(System.Net.Sockets.UdpClient client) : base() {
            this.Client = client;
            this.ConnectionState = ConnectionState.ConnectionLoggedIn;

            this.PacketSerializer = new MockPacketSerializer();
        }
    }
}
