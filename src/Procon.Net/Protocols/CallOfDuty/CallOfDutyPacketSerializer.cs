using System.Text;

namespace Procon.Net.Protocols.CallOfDuty {
    public class CallOfDutyPacketSerializer : PacketSerializer<CallOfDutyPacket> {

        public CallOfDutyPacketSerializer()
            : base() {
                this.PacketHeaderSize = 2 + sizeof(int) + 1;
        }

        public override byte[] Serialize(CallOfDutyPacket packet) {

            byte[] encodedPacket = new byte[(7 + packet.Password.Length + packet.Message.Length)];
            encodedPacket[0] = encodedPacket[1] = encodedPacket[2] = encodedPacket[3] = 0xFF;
            encodedPacket[4] = 0x00;

            Encoding.ASCII.GetBytes(packet.Password).CopyTo(encodedPacket, 5);
            encodedPacket[5 + packet.Password.Length] = 0x20;
            Encoding.ASCII.GetBytes(packet.Message).CopyTo(encodedPacket, 6 + packet.Password.Length);
            encodedPacket[(6 + packet.Password.Length + packet.Message.Length)] = 0x00;

            return encodedPacket;
        }

        public override CallOfDutyPacket Deserialize(byte[] packetData) {
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

        // Unused by CallOfDutyClient (UDP packet and not reliable)
        public override long ReadPacketSize(byte[] packetData) {
            return 0;
        }
    }
}
