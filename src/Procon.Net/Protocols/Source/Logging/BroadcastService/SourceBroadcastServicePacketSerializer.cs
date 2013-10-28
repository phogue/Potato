using System;
using System.Text;

namespace Procon.Net.Protocols.Source.Logging.BroadcastService {
    public class SourceBroadcastServicePacketSerializer : PacketSerializer<SourceBroadcastServicePacket> {

        public SourceBroadcastServicePacketSerializer()
            : base() {
                this.PacketHeaderSize = sizeof(int) * 2 + sizeof(ushort);
        }

        public override byte[] Serialize(SourceBroadcastServicePacket packet) {
            byte[] returnPacket = new byte[this.PacketHeaderSize + packet.String1.Length + 1];

            returnPacket[0] = returnPacket[1] = returnPacket[2] = returnPacket[3] = 0xFF;
            packet.RemoteEndPoint.Address.GetAddressBytes().CopyTo(returnPacket, 4);
            BitConverter.GetBytes((ushort)packet.RemoteEndPoint.Port).CopyTo(returnPacket, 8);

            Encoding.GetEncoding(1252).GetBytes(packet.String1 + Convert.ToChar(0x00)).CopyTo(returnPacket, this.PacketHeaderSize);

            return returnPacket;
        }

        public override SourceBroadcastServicePacket Deserialize(byte[] packetData) {
            //throw new NotImplementedException();
            SourceBroadcastServicePacket packet = new SourceBroadcastServicePacket() {
                IsResponse = false,
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

        public override long ReadPacketSize(byte[] packetData) {
            //throw new NotImplementedException();

            return 0;
        }

    }
}
