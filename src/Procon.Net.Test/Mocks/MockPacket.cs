using System;

namespace Procon.Net.Test.Mocks {
    public class MockPacket : Packet {

        public MockPacket() : base() {
            this.Stamp = DateTime.Now;
        }

        public MockPacket(PacketOrigin origin, PacketType type) : base(origin, type) {
            
        }

        /// <summary>
        /// Exposes the null packet method
        /// </summary>
        public new void NullPacket() {
            base.NullPacket();
        }

        public override string ToString() {
            return String.Format("{0} {1} {2}", this.Origin, this.Type, this.RequestId);
        }
    }
}
