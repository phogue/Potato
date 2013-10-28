using System;
using System.Net;
using System.Text;

namespace Procon.Net.Protocols.Source.Logging.BroadcastListener {
    using Procon.Net.Utils;
    public class SourceBroadcastListenerPacketSerializer : PacketSerializer<SourceBroadcastListenerPacket> {

        // This should never be called.
        public override byte[] Serialize(SourceBroadcastListenerPacket packet) {
            throw new NotImplementedException();
        }

        public override SourceBroadcastListenerPacket Deserialize(byte[] packetData) {
            //throw new NotImplementedException();
            SourceBroadcastListenerPacket packet = new SourceBroadcastListenerPacket() {
                IsResponse = false,
                Origin = PacketOrigin.Server
            };

            if (packetData[0] == 0xFF && packetData[1] == 0xFF && packetData[2] == 0xFF && packetData[3] == 0xFF) {

                packet.RemoteEndPoint = new IPEndPoint(BitConverter.ToUInt32(packetData, 4), BitConverter.ToUInt16(packetData, 8));

                for (int offset = 10; offset < packetData.Length; offset++) {
                    if (packetData[offset] == 0) {

                        packet.String1 = Encoding.Default.GetString(packetData, 10, offset - 10).TrimEnd('\r', '\n');
                        packet.String1Words = packet.String1.Wordify();
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
