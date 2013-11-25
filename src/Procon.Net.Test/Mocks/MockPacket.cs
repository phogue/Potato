using System;
using Procon.Net.Utils;

namespace Procon.Net.Test.Mocks {
    public class MockPacket : IPacketWrapper {
        /// <summary>
        /// The underlying simple packet class 
        /// </summary>
        public IPacket Packet { get; set; }

        /// <summary>
        /// Basic text to pass bac kand forth
        /// </summary>
        public String Text {
            get { return this.Packet.Text; }
            set {
                this.Packet.Text = value;
                this.Packet.Words = value.Wordify();
            }
        }

        public MockPacket() : base() {
            this.Packet = new Packet();
        }

        public MockPacket(PacketOrigin origin, PacketType type) {
            this.Packet = new Packet(origin, type);
        }

        public override string ToString() {
            return String.Format("{0} {1} {2} {3}", this.Packet.Origin, this.Packet.Type, this.Packet.RequestId, this.Packet.Text);
        }
    }
}
