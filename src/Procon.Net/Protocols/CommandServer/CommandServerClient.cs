#region Copyright
// Copyright 2014 Myrcon Pty. Ltd.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
using System.Net;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using Procon.Net.Shared;

namespace Procon.Net.Protocols.CommandServer {
    /// <summary>
    /// Handles a https client issuing a command server request
    /// </summary>
    /// <remarks>We can recycle this from the client since a client does not matter where the origin/server is.</remarks>
    public class CommandServerClient : TcpClient {

        /// <summary>
        /// The loaded CommandServer.pfx certificate to encrypt incoming stream
        /// </summary>
        protected X509Certificate2 Certificate { get; set; }

        /// <summary>
        /// Initializes the TcpClient and sets up an SslStream against the certificate
        /// </summary>
        /// <param name="client"></param>
        /// <param name="certificate"></param>
        public CommandServerClient(System.Net.Sockets.TcpClient client, X509Certificate2 certificate) : base() {
            this.Client = client;
            this.RemoteEndPoint = (IPEndPoint) client.Client.RemoteEndPoint;

            this.PacketSerializer = new CommandServerPacketSerializer();

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

                this.ConnectionState = ConnectionState.ConnectionReady;
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
