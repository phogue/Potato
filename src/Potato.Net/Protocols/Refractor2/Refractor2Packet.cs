using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Potato.Net.Protocols.Refractor2
{
    class Refractor2Packet : Packet
    {
        /// <summary>Contains the information to be sent or information received across the network.</summary>
        public String Data { get; set; }

        /// <summary>Default constructor, takes an array of bytes.</summary>
        /// <param name="packet">The information section of a packet.</param>
        public Refractor2Packet(byte[] packet)
            : base(packet) {
        }

        /// <summary>Encodes a packet into network sendable bytes.</summary>
        /// <returns>The packet to encode.</returns>
        public override byte[] EncodePacket()
        {
            return Encoding.Default.GetBytes(Data);
        }

        /// <summary>Decodes a packet into readable information.</summary>
        /// <param name="packet">The packet to decode.</param>
        public override void DecodePacket(byte[] packet)
        {
            Data = Encoding.Default.GetString(packet, 0, packet.Length);
        }

        /// <summary>Override the method used to determine the size of a packet.</summary>
        /// <param name="packet">The bytes received so far.</param>
        /// <returns>The size of packet.  UInt32.MaxValue if could not determined size.</returns>
        public new static UInt32 DecodePacketSize(byte[] packet)
        {
            // Check the received bytes so far for the delimiter.
            Int32 packetSize = Encoding.Default.GetString(packet, 0, packet.Length).IndexOf('\n');

            // Return the length of the packet.
            if (packetSize >= 0)
                return (UInt32)packetSize;
            return UInt32.MaxValue;
        }
    }
}
