using System;
using System.Text;
using Procon.Net.Shared;

namespace Procon.Net.Test.Mocks {
    public class MockPacketSerializer : IPacketSerializer {

        public uint PacketHeaderSize { get; set; }

        public MockPacketSerializer() {
            this.PacketHeaderSize = 16;
        }

        public IPacketWrapper Deserialize(byte[] packetData) {
            int length = BitConverter.ToInt32(packetData, 12);

            MockPacket wrapper = new MockPacket {
                Packet = {
                    Origin = (PacketOrigin)BitConverter.ToInt32(packetData, 0),
                    Type = (PacketType)BitConverter.ToInt32(packetData, 4),
                    RequestId = BitConverter.ToInt32(packetData, 8)
                },
                Text = Encoding.ASCII.GetString(packetData, (int)this.PacketHeaderSize, length)
            };

            wrapper.Packet.DebugText = String.Format("{0} {1} {2} {3}", wrapper.Packet.Origin, wrapper.Packet.Type, wrapper.Packet.RequestId, wrapper.Packet.Text);

            return wrapper;
        }

        public byte[] Serialize(IPacketWrapper wrapper) {
            MockPacket mockPacket = wrapper as MockPacket;
            byte[] serialized = null;

            if (mockPacket != null) {
                if (mockPacket.Packet.RequestId == null)
                    mockPacket.Packet.RequestId = 111;

                serialized = new byte[this.PacketHeaderSize + mockPacket.Text.Length];

                BitConverter.GetBytes((int)mockPacket.Packet.Origin).CopyTo(serialized, 0);
                BitConverter.GetBytes((int)mockPacket.Packet.Type).CopyTo(serialized, 4);
                BitConverter.GetBytes(mockPacket.Packet.RequestId.Value).CopyTo(serialized, 8);
                BitConverter.GetBytes(mockPacket.Text.Length).CopyTo(serialized, 12);
                Encoding.ASCII.GetBytes(mockPacket.Text).CopyTo(serialized, this.PacketHeaderSize);

                mockPacket.Packet.DebugText = String.Format("{0} {1} {2} {3}", mockPacket.Packet.Origin, mockPacket.Packet.Type, mockPacket.Packet.RequestId, mockPacket.Packet.Text);
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
