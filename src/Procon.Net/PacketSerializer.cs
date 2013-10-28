using System;

namespace Procon.Net {
    public abstract class PacketSerializer<P> where P : Packet {
        
        public UInt32 PacketHeaderSize { get; protected set; }

        protected PacketSerializer() {
            this.PacketHeaderSize = 0;
        }

        public abstract P Deserialize(byte[] packetData);
        public abstract byte[] Serialize(P packet);
        public abstract long ReadPacketSize(byte[] packetData);
    }
}
