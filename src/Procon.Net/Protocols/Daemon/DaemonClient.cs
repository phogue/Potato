using System.Net;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;

namespace Procon.Net.Protocols.Daemon {
    /// <summary>
    /// Handles a https client issuing a daemon request
    /// </summary>
    /// <remarks>We can recycle this from the client since a client does not matter where the origin/server is.</remarks>
    public class DaemonClient : Procon.Net.TcpClient {

        /// <summary>
        /// The loaded daemon.pfx certificate to encrypt incoming stream
        /// </summary>
        protected X509Certificate2 Certificate { get; set; }

        /// <summary>
        /// Initializes the TcpClient and sets up an SslStream against the certificate
        /// </summary>
        /// <param name="client"></param>
        /// <param name="certificate"></param>
        public DaemonClient(System.Net.Sockets.TcpClient client, X509Certificate2 certificate)
            : base(((IPEndPoint)client.Client.RemoteEndPoint).Address.ToString(), (ushort)((IPEndPoint)client.Client.RemoteEndPoint).Port) {
            this.Client = client;

            this.PacketSerializer = new DaemonPacketSerializer();

            this.Certificate = certificate;

            this.Stream = this.Authenticate(new SslStream(client.GetStream(), false));
        }

        /// <summary>
        /// Authenticates as server
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        protected SslStream Authenticate(SslStream stream) {
            try {
                stream.AuthenticateAsServer(this.Certificate);

                this.ConnectionState = Net.ConnectionState.ConnectionReady;
            }
            catch (AuthenticationException e) {
                this.Shutdown(e);
            }

            return stream;
        }

        protected override long ReadPacketPeekShiftSize {
            get { return this.PacketStream != null ? this.PacketStream.Size() : 0; }
        }
    }
}
