using System;
using System.Collections.Generic;
using System.Net;
using Procon.Net.Shared;

namespace Procon.Net {

    [Serializable]
    public sealed class Packet : IPacket {

        /// <summary>
        /// When this packet was created
        /// </summary>
        public DateTime Stamp { get; set; }

        /// <summary>
        /// The origin of the packet. This is useful when the server sends back responses to packets, we
        /// can say the packet originiated from the client and this is the response.
        /// </summary>
        public PacketOrigin Origin { get; set; }

        /// <summary>
        /// If this is a response or not to a previous packet.
        /// </summary>
        public PacketType Type { get; set; }

        /// <summary>
        /// The sequence id for this command/event
        /// </summary>
        public int? RequestId { get; set; }

        /// <summary>
        /// The raw bytes used to deserialize this packet.
        /// </summary>
        public byte[] Data { get; set; }

        /// <summary>
        /// Textual representation of this packet
        /// </summary>
        public String Text { get; set; }

        /// <summary>
        /// Textual representation of the packet
        /// </summary>
        public String DebugText { get; set; }

        /// <summary>
        /// The wordified version of the text string version of the packet.
        /// </summary>
        public List<String> Words { get; set; }

        /// <summary>
        /// The remote end point for the packet.
        /// </summary>
        public IPEndPoint RemoteEndPoint { get; set; }

        public Packet() {
            this.RequestId = null;
            this.Stamp = DateTime.Now;
            this.Words = new List<String>();
        }
    }
}
