using System;
using System.Text;

namespace Procon.Net.Protocols.Source {
    using Procon.Net.Utils;
    public class SourcePacketSerializer : IPacketSerializer {

        public uint PacketHeaderSize { get; set; }

        public SourcePacketSerializer()
            : base() {
                this.PacketHeaderSize = sizeof(int) * 3;
        }

        public byte[] Serialize(Packet packet) {
            SourcePacket sourcePacket = packet as SourcePacket;
            byte[] serialized = null;

            if (sourcePacket != null && sourcePacket.RequestId != null) {
                serialized = new byte[this.PacketHeaderSize + sourcePacket.String1.Length + sourcePacket.String2.Length + 2];

                // Length from request id to end null terminator
                BitConverter.GetBytes(serialized.Length - sizeof(int)).CopyTo(serialized, 0);

                // Request ID
                BitConverter.GetBytes((int)sourcePacket.RequestId).CopyTo(serialized, 4);

                // Request Type
                BitConverter.GetBytes((int)sourcePacket.RequestType).CopyTo(serialized, 8);

                Encoding.GetEncoding(1252).GetBytes(sourcePacket.String1 + Convert.ToChar(0x00)).CopyTo(serialized, this.PacketHeaderSize);

                Encoding.GetEncoding(1252).GetBytes(sourcePacket.String2 + Convert.ToChar(0x00)).CopyTo(serialized, this.PacketHeaderSize + sourcePacket.String1.Length + 1);

            }

            return serialized;
        }

        public Packet Deserialize(byte[] packetData) {
            SourcePacket packet = new SourcePacket();

            packet.Type = PacketType.Response;
            packet.Origin = PacketOrigin.Client;
            packet.RequestId = BitConverter.ToInt32(packetData, 4);
            packet.ResponseType = (SourceResponseType)BitConverter.ToInt32(packetData, 8);

            int offset = (int)this.PacketHeaderSize;

            while (offset < packetData.Length && packetData[offset] != 0) {

                packet.String1 += Encoding.Default.GetString(new[] { packetData[offset] });

                offset++;
            }

            offset++;

            while (offset < packetData.Length && packetData[offset] != 0) {
                packet.String2 += Encoding.Default.GetString(new[] { packetData[offset] });

                offset++;
            }

            packet.Words = packet.String1.Wordify();

            return packet;
        }

        public long ReadPacketSize(byte[] packetData) {
            long length = 0;

            if (packetData.Length >= this.PacketHeaderSize) {
                length = BitConverter.ToUInt32(packetData, 0) + sizeof(int);
            }

            return length;
        }

    }
}
