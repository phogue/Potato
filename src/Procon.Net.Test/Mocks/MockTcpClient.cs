using System;
using System.Net;

namespace Procon.Net.Test.Mocks {
    public class MockTcpClient : TcpClient {

        public MockTcpClient(String hostName, ushort port)
            : base(hostName, port) {

            this.PacketSerializer = new MockPacketSerializer();
        }

        public MockTcpClient(System.Net.Sockets.TcpClient client)
            : base(((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString(), (ushort)((IPEndPoint)client.Client.RemoteEndPoint).Port) {
            this.Client = client;
            this.Stream = client.GetStream();
            this.ConnectionState = Net.ConnectionState.ConnectionLoggedIn;

            this.PacketSerializer = new MockPacketSerializer();
        }
    }
}
