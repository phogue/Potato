using System.Text;

namespace Potato.Net.Protocols.CallOfDuty {
    public class CallOfDutyPacketSerializer : IPacketSerializer {
        /// <summary>
        /// The minimum packet size requires to be passed into the packet serializer. Anything smaller
        /// and it the full header of a packet wouldn't be available, therefore we wouldn't know
        /// how many bytes the full packet is.
        /// </summary>
        public uint PacketHeaderSize { get; set; }

        public CallOfDutyPacketSerializer()
            : base() {
                this.PacketHeaderSize = 2 + sizeof(int) + 1;
        }

        /// <summary>
        /// Serializes a packet into an array of bytes to send to the server.
        /// </summary>
        /// <param name="packet">The packe to serialize</param>
        /// <returns>An array of bytes to send to the server.</returns>
        public byte[] Serialize(Packet packet) {
            CallOfDutyPacket callOfDutyPacket = packet as CallOfDutyPacket;
            byte[] encodedPacket = null;

            if (callOfDutyPacket != null) {
                encodedPacket = new byte[(7 + callOfDutyPacket.Password.Length + callOfDutyPacket.Message.Length)];
                encodedPacket[0] = encodedPacket[1] = encodedPacket[2] = encodedPacket[3] = 0xFF;
                encodedPacket[4] = 0x00;

                Encoding.ASCII.GetBytes(callOfDutyPacket.Password).CopyTo(encodedPacket, 5);
                encodedPacket[5 + callOfDutyPacket.Password.Length] = 0x20;
                Encoding.ASCII.GetBytes(callOfDutyPacket.Message).CopyTo(encodedPacket, 6 + callOfDutyPacket.Password.Length);
                encodedPacket[(6 + callOfDutyPacket.Password.Length + callOfDutyPacket.Message.Length)] = 0x00;
            }

            return encodedPacket;
        }

        /// <summary>
        /// Deserializes an array of bytes into a Packet of type P
        /// </summary>
        /// <param name="packetData">The array to deserialize to a packet. Must be exact length of bytes.</param>
        /// <returns>A new packet with data extracted from packetDate</returns>
        public Packet Deserialize(byte[] packetData) {
            CallOfDutyPacket packet = new CallOfDutyPacket() {
                Type = PacketType.Request,
                Origin = PacketOrigin.Client
            };

            string[] packetStrings = Encoding.ASCII.GetString(packetData, 5, packetData.Length - 5).Split(new char[] { '\n' }, 2);

            if (packetStrings.Length == 2) {
                packet.Command = packetStrings[0];
                packet.Message = packetStrings[1];

                packet.IsEOP = (packet.Message.Length == 0 || (packet.Message.Length >= 2 && packet.Message[packet.Message.Length - 1] == '\n' && packet.Message[packet.Message.Length - 2] == '\n'));
            }

            return packet;
        }

        /// <summary>
        /// Unused by CallOfDutyClient (UDP packet and not reliable)
        /// Fetches the full packet size by reading the header of a packet.
        /// </summary>
        /// <param name="packetData">The possibly incomplete packet data, or as much data as we have recieved from the server.</param>
        /// <returns>The total size, in bytes, that is requires for the header + data to be deserialized.</returns>
        public long ReadPacketSize(byte[] packetData) {
            return 0;
        }
    }
}
