using System.Net;
using System.Net.Sockets;

namespace Procon.Net.Protocols.Daemon {
    // We can recycle this from the client since a client does not matter where the origin/server is.
    public class DaemonClient : Procon.Net.TcpClient {

        public DaemonClient(System.Net.Sockets.TcpClient client)
            : base(((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString(), (ushort)((IPEndPoint)client.Client.RemoteEndPoint).Port) {
            this.Client = client;
            this.NetworkStream = client.GetStream();
            this.ConnectionState = Net.ConnectionState.ConnectionReady;

            this.PacketSerializer = new DaemonPacketSerializer();
        }

        protected override long ReadPacketPeekShiftSize {
            get { return this.PacketStream != null ? this.PacketStream.Size() : 0; }
        }
    }
}
