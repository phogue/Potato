using System;
using System.Net;
using System.Text;

namespace Procon.Net.Protocols.Source.Logging.BroadcastListener {
    using Procon.Net.Utils;
    public class SourceBroadcastListenerPacketSerializer : IPacketSerializer {
        /// <summary>
        /// The minimum packet size requires to be passed into the packet serializer. Anything smaller
        /// and it the full header of a packet wouldn't be available, therefore we wouldn't know
        /// how many bytes the full packet is.
        /// </summary>
        public uint PacketHeaderSize { get; set; }

        /// <summary>
        /// Serializes a packet into an array of bytes to send to the server.
        /// </summary>
        /// <param name="packet">The packe to serialize</param>
        /// <returns>An array of bytes to send to the server.</returns>
        public byte[] Serialize(Packet packet) {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deserializes an array of bytes into a Packet of type P
        /// </summary>
        /// <param name="packetData">The array to deserialize to a packet. Must be exact length of bytes.</param>
        /// <returns>A new packet with data extracted from packetDate</returns>
        public Packet Deserialize(byte[] packetData) {
            //throw new NotImplementedException();
            SourceBroadcastListenerPacket packet = new SourceBroadcastListenerPacket() {
                Type = PacketType.Request,
                Origin = PacketOrigin.Server
            };

            if (packetData[0] == 0xFF && packetData[1] == 0xFF && packetData[2] == 0xFF && packetData[3] == 0xFF) {

                packet.RemoteEndPoint = new IPEndPoint(BitConverter.ToUInt32(packetData, 4), BitConverter.ToUInt16(packetData, 8));

                for (int offset = 10; offset < packetData.Length; offset++) {
                    if (packetData[offset] == 0) {

                        packet.String1 = Encoding.Default.GetString(packetData, 10, offset - 10).TrimEnd('\r', '\n');
                        packet.Words = packet.String1.Wordify();
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
            //throw new NotImplementedException();

            return 0;
        }

    }
}
