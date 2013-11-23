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

            Assert.AreEqual(PacketOrigin.None, packet.Origin);
            Assert.AreEqual(PacketType.None, packet.Type);
            Assert.IsNull(packet.RequestId);
            Assert.GreaterOrEqual(DateTime.Now, packet.Stamp);
        }

        [Test]
        public void TestPacketParameterConstructor() {
            MockPacket packet = new MockPacket(PacketOrigin.Server, PacketType.Response);

            Assert.AreEqual(PacketOrigin.Server, packet.Origin);
            Assert.AreEqual(PacketType.Response, packet.Type);
            Assert.IsNull(packet.RequestId);
            Assert.GreaterOrEqual(DateTime.Now, packet.Stamp);
        }

        [Test]
        public void TestPacketNullPacket() {
            MockPacket packet = new MockPacket(PacketOrigin.Server, PacketType.Response);

            packet.NullPacket();

            Assert.AreEqual(PacketOrigin.Client, packet.Origin);
            Assert.AreEqual(PacketType.Request, packet.Type);
            Assert.IsNull(packet.RequestId);
        }

        [Test]
        public void TestPacketToDebugString() {
            MockPacket packet = new MockPacket(PacketOrigin.Server, PacketType.Response);
            packet.RequestId = 222;

            Assert.AreEqual("Server Response 222", packet.ToDebugString());
        }
    }
}
