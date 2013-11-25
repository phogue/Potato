using System;
using System.Collections.Specialized;
using System.Net;

namespace Procon.Net.Protocols.Daemon {
    [Serializable]
    public class DaemonPacket : IPacketWrapper {

        /// <summary>
        /// The requested Uri
        /// </summary>
        public Uri Request { get; set; }

        /// <summary>
        /// The status code, used when responding to the request.
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }

        /// <summary>
        /// The method used during the request/response
        /// </summary>
        public String Method { get; set; }

        /// <summary>
        /// The protocol version being used (1.0.0.0 or 1.1.0.0)
        /// </summary>
        public Version ProtocolVersion { get; set; }

        /// <summary>
        /// All of the headers for this http request/response.
        /// </summary>
        public WebHeaderCollection Headers { get; set; }

        /// <summary>
        /// The post/get data attached to the request.
        /// </summary>
        public NameValueCollection Query { get; set; }

        /// <summary>
        /// The raw header data recieved or to be sent.
        /// </summary>
        public String Header { get; set; }

        /// <summary>
        /// The raw data from a POST request or the data to be output as a response.
        /// </summary>
        public String Content { get; set; }

        public IPacket Packet { get; set; }

        public DaemonPacket() {
            this.Headers = new WebHeaderCollection();
            this.Query = new NameValueCollection();
            this.Content = String.Empty;

            this.Packet = new Packet();
        }
    }
}
