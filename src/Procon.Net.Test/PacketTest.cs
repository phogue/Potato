using System;
using NUnit.Framework;
using Procon.Net.Test.Mocks;

namespace Procon.Net.Test {
    /// <summary>
    /// These tests are for coverage, nothing more.
    /// </summary>
    [TestFixture]
    public class PacketTest {

        [Test]
        public void TestPacketEmptyConstructor() {
            MockPacket packet = new MockPacket();

            Assert.AreEqual(PacketOrigin.None, packet.Packet.Origin);
            Assert.AreEqual(PacketType.None, packet.Packet.Type);
            Assert.IsNull(packet.Packet.RequestId);
            Assert.GreaterOrEqual(DateTime.Now, packet.Packet.Stamp);
        }

        [Test]
        public void TestPacketParameterConstructor() {
            MockPacket packet = new MockPacket(PacketOrigin.Server, PacketType.Response);

            Assert.AreEqual(PacketOrigin.Server, packet.Packet.Origin);
            Assert.AreEqual(PacketType.Response, packet.Packet.Type);
            Assert.IsNull(packet.Packet.RequestId);
            Assert.GreaterOrEqual(DateTime.Now, packet.Packet.Stamp);
        }
    }
}
