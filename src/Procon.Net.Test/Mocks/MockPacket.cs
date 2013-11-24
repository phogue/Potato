using System;
using Procon.Net.Utils;

namespace Procon.Net.Test.Mocks {
    public class MockPacket : Packet {
        private string _text;

        /// <summary>
        /// Basic text to pass bac kand forth
        /// </summary>
        public String Text {
            get { return _text; }
            set {
                _text = value;
                this.Words = _text.Wordify();
            }
        }

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
            return String.Format("{0} {1} {2} {3}", this.Origin, this.Type, this.RequestId, this.Text);
        }
    }
}
