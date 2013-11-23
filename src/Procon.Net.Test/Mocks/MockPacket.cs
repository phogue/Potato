using System;

namespace Procon.Net.Test.Mocks {
    public class MockPacket : Packet {

        public MockPacket() {
            this.Stamp = DateTime.Now;
        }

    }
}
