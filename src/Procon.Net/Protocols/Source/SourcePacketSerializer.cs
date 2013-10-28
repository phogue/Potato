using System;
using System.Text;

namespace Procon.Net.Protocols.Source {
    using Procon.Net.Utils;
    public class SourcePacketSerializer : PacketSerializer<SourcePacket> {

        public SourcePacketSerializer()
            : base() {
                this.PacketHeaderSize = sizeof(int) * 3;
        }

        public override byte[] Serialize(SourcePacket packet) {
            byte[] returnPacket = new byte[this.PacketHeaderSize + packet.String1.Length + packet.String2.Length + 2];

            // Length from request id to end null terminator
            BitConverter.GetBytes((int)returnPacket.Length - sizeof(int)).CopyTo(returnPacket, 0);

            // Request ID
            BitConverter.GetBytes((int)packet.RequestId).CopyTo(returnPacket, 4);

            // Request Type
            BitConverter.GetBytes((int)packet.RequestType).CopyTo(returnPacket, 8);

            Encoding.GetEncoding(1252).GetBytes(packet.String1 + Convert.ToChar(0x00)).CopyTo(returnPacket, this.PacketHeaderSize);

            Encoding.GetEncoding(1252).GetBytes(packet.String2 + Convert.ToChar(0x00)).CopyTo(returnPacket, this.PacketHeaderSize + packet.String1.Length + 1);

            return returnPacket;
        }

        public override SourcePacket Deserialize(byte[] packetData) {
            SourcePacket packet = new SourcePacket();

            packet.IsResponse = true;
            packet.Origin = PacketOrigin.Client;
            packet.RequestId = BitConverter.ToInt32(packetData, 4);
            packet.ResponseType = (SourceResponseType)BitConverter.ToInt32(packetData, 8);

            int offset = (int)this.PacketHeaderSize;

            while (offset < packetData.Length && packetData[offset] != 0) {

                packet.String1 += Encoding.Default.GetString(new byte[] { packetData[offset] });

                offset++;
            }

            offset++;

            while (offset < packetData.Length && packetData[offset] != 0) {
                packet.String2 += Encoding.Default.GetString(new byte[] { packetData[offset] });

                offset++;
            }

            packet.String1Words = packet.String1.Wordify();

            return packet;
        }

        public override long ReadPacketSize(byte[] packetData) {
            long length = 0;

            if (packetData.Length >= this.PacketHeaderSize) {
                length = BitConverter.ToUInt32(packetData, 0) + sizeof(int);
            }

            return length;
        }

    }
}
