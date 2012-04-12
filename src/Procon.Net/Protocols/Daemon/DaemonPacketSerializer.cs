using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Procon.Net.Protocols.Daemon {
    public class DaemonPacketSerializer : PacketSerializer<DaemonPacket> {


        public DaemonPacketSerializer()
            : base() {
                this.PacketHeaderSize = 12;
        }

        public override byte[] Serialize(DaemonPacket packet) {

            return new byte[6];
        }

        public override DaemonPacket Deserialize(byte[] packetData) {

            return new DaemonPacket();
        }

        public override uint ReadPacketSize(byte[] packetData) {

            return 0;
        }
    }
}
