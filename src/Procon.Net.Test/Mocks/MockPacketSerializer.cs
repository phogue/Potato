using System;
using System.Text;

namespace Procon.Net.Test.Mocks {
    public class MockPacketSerializer : IPacketSerializer {

        public uint PacketHeaderSize { get; set; }

        public MockPacketSerializer() {
            this.PacketHeaderSize = 16;
        }

        public Packet Deserialize(byte[] packetData) {
            int length = BitConverter.ToInt32(packetData, 12);

            return new MockPacket {
                Origin = (PacketOrigin) BitConverter.ToInt32(packetData, 0),
                Type = (PacketType) BitConverter.ToInt32(packetData, 4),
                RequestId = BitConverter.ToInt32(packetData, 8),
                Text = Encoding.ASCII.GetString(packetData, (int) this.PacketHeaderSize, length)
            };
        }

        public byte[] Serialize(Packet packet) {
            MockPacket mockPacket = packet as MockPacket;
            byte[] serialized = null;

            if (mockPacket != null) {
                if (mockPacket.RequestId == null)
                    mockPacket.RequestId = 111;

                serialized = new byte[this.PacketHeaderSize + mockPacket.Text.Length];

                BitConverter.GetBytes((int)mockPacket.Origin).CopyTo(serialized, 0);
                BitConverter.GetBytes((int)mockPacket.Type).CopyTo(serialized, 4);
                BitConverter.GetBytes(mockPacket.RequestId.Value).CopyTo(serialized, 8);
                BitConverter.GetBytes(mockPacket.Text.Length).CopyTo(serialized, 12);
                Encoding.ASCII.GetBytes(mockPacket.Text).CopyTo(serialized, this.PacketHeaderSize);
            }

            return serialized;
        }

        public long ReadPacketSize(byte[] packetData) {
            long length = 0;

            if (packetData.Length >= this.PacketHeaderSize) {
                length = BitConverter.ToUInt32(packetData, 12) + this.PacketHeaderSize;
            }

            return length;
        }
    }
}
