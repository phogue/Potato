using System;
using System.Net;

namespace Procon.Net.Test.Mocks {
    public class MockUdpClient : Procon.Net.UdpClient {
        
        public MockUdpClient(String hostName, ushort port)
            : base(hostName, port) {

            this.PacketSerializer = new MockPacketSerializer();
        }

        public MockUdpClient(System.Net.Sockets.UdpClient client)
            : base(((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString(), (ushort)((IPEndPoint)client.Client.RemoteEndPoint).Port) {
            this.Client = client;
            this.ConnectionState = Net.ConnectionState.ConnectionLoggedIn;

            this.PacketSerializer = new MockPacketSerializer();
        }
    }
}
