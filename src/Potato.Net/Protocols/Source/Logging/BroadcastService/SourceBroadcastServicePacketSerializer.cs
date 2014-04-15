using System;
using System.Text;

namespace Potato.Net.Protocols.Source.Logging.BroadcastService {
    public class SourceBroadcastServicePacketSerializer : IPacketSerializer {
        /// <summary>
        /// The minimum packet size requires to be passed into the packet serializer. Anything smaller
        /// and it the full header of a packet wouldn't be available, therefore we wouldn't know
        /// how many bytes the full packet is.
        /// </summary>
        public uint PacketHeaderSize { get; set; }

        public SourceBroadcastServicePacketSerializer()
            : base() {
                this.PacketHeaderSize = sizeof(int) * 2 + sizeof(ushort);
        }

        /// <summary>
        /// Serializes a packet into an array of bytes to send to the server.
        /// </summary>
        /// <param name="packet">The packe to serialize</param>
        /// <returns>An array of bytes to send to the server.</returns>
        public byte[] Serialize(Packet packet) {
            SourceBroadcastServicePacket sourceBroadcastServicePacket = packet as SourceBroadcastServicePacket;
            byte[] returnPacket = null;

            if (sourceBroadcastServicePacket != null) {
                returnPacket = new byte[this.PacketHeaderSize + sourceBroadcastServicePacket.String1.Length + 1];

                returnPacket[0] = returnPacket[1] = returnPacket[2] = returnPacket[3] = 0xFF;
                packet.RemoteEndPoint.Address.GetAddressBytes().CopyTo(returnPacket, 4);
                BitConverter.GetBytes((ushort)packet.RemoteEndPoint.Port).CopyTo(returnPacket, 8);

                Encoding.GetEncoding(1252).GetBytes(sourceBroadcastServicePacket.String1 + Convert.ToChar(0x00)).CopyTo(returnPacket, this.PacketHeaderSize);
            }

            return returnPacket;
        }

        /// <summary>
        /// Deserializes an array of bytes into a Packet of type P
        /// </summary>
        /// <param name="packetData">The array to deserialize to a packet. Must be exact length of bytes.</param>
        /// <returns>A new packet with data extracted from packetDate</returns>
        public Packet Deserialize(byte[] packetData) {
            SourceBroadcastServicePacket packet = new SourceBroadcastServicePacket() {
                Type = PacketType.Request,
                Origin = PacketOrigin.Server
            };

            if (packetData[0] == 0xFF && packetData[1] == 0xFF && packetData[2] == 0xFF && packetData[3] == 0xFF) {
                for (int offset = 4; offset < packetData.Length; offset++) {
                    if (packetData[offset] == 0) {

                        packet.String1 = Encoding.Default.GetString(packetData, 4, offset - 4).TrimEnd('\r', '\n');
                        // Never needed..
                        // packet.String1Words = packet.String1.Wordify();
                        break;
                    }
                }
            }



            return packet;
        }

        /// <summary>
        /// Fetches the full packet size by reading the header of a packet.
        /// </summary>
        /// <param name="packetData">The possibly incomplete packet data, or as much data as we have recieved from the server.</param>
        /// <returns>The total size, in bytes, that is requires for the header + data to be deserialized.</returns>
        public long ReadPacketSize(byte[] packetData) {

            return 0;
        }

    }
}
