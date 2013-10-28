using System.Net;
using System.Net.Sockets;

namespace Procon.Net.Protocols.Daemon {
    // We can recycle this from the client since a client does not matter where the origin/server is.
    public class DaemonClient : TcpClient<DaemonPacket> {

        public DaemonClient(TcpClient client) : base(((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString(), (ushort)((IPEndPoint)client.Client.RemoteEndPoint).Port) {
            this.Client = client;
            this.NetworkStream = client.GetStream();
            this.ConnectionState = Net.ConnectionState.ConnectionReady;

            this.PacketSerializer = new DaemonPacketSerializer();
        }

    }
}
