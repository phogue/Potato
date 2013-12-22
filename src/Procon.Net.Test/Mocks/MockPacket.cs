using System;
using Procon.Net.Shared;
using Procon.Net.Shared.Utils;
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
    }
}
