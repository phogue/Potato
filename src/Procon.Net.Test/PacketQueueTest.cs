using System;
using NUnit.Framework;
using Procon.Net.Test.Mocks;

namespace Procon.Net.Test {
    [TestFixture]
    public class PacketQueueTest {

        /// <summary>
        /// Tests that a packet is returned immediately from telling the queue it has been sent,
        /// inferring the packet is not queued and should be sent immediately.
        /// </summary>
        [Test]
        public void TestPacketSendImmediate() {

            PacketQueue queue = new PacketQueue();
            Packet packet = new MockPacket() {
                Origin = PacketOrigin.Client,
                Type = PacketType.Request,
                RequestId = 1
            };

            Packet poppedPacket = queue.PacketSend(packet);

            // Client would send to the server.

            Assert.AreEqual(packet, poppedPacket);
            Assert.AreEqual(1, queue.OutgoingPackets.Count);
            Assert.AreEqual(0, queue.QueuedPackets.Count);
        }

        /// <summary>
        /// Tests that a packet will not be sent to the server if another packet
        /// is currently sent to the server and awaiting a response.
        /// </summary>
        [Test]
        public void TestPacketSendQueued() {

            PacketQueue queue = new PacketQueue();
            Packet firstPacket = new MockPacket() {
                Origin = PacketOrigin.Client,
                Type = PacketType.Request,
                RequestId = 1
            };

            Packet poppedPacket = queue.PacketSend(firstPacket);

            // Client would send to the server.

            Assert.AreEqual(firstPacket, poppedPacket);
            Assert.AreEqual(1, queue.OutgoingPackets.Count);

            Packet secondPacket = new MockPacket() {
                Origin = PacketOrigin.Client,
                Type = PacketType.Request,
                RequestId = 1
            };

            poppedPacket = queue.PacketSend(secondPacket);

            // Popped packet is null, client would essentially ignore it until later.

            Assert.IsNull(poppedPacket);
            Assert.AreEqual(1, queue.OutgoingPackets.Count);
            Assert.AreEqual(1, queue.QueuedPackets.Count);
        }

        /// <summary>
        /// Tests that response to a request will remove from the outgoing list of packets.
        /// </summary>
        [Test]
        public void TestPacketReceivedRemoveFromOutgoing() {

            PacketQueue queue = new PacketQueue();

            Packet sentPacket = new MockPacket() {
                Origin = PacketOrigin.Client,
                Type = PacketType.Request,
                RequestId = 1
            };

            Packet recievedPacket = new MockPacket() {
                Origin = PacketOrigin.Client,
                Type = PacketType.Response,
                RequestId = 1
            };

            Packet poppedPacket = queue.PacketSend(sentPacket);

            // Client would send to the server.

            Assert.AreEqual(sentPacket, poppedPacket);
            Assert.AreEqual(1, queue.OutgoingPackets.Count);
            Assert.AreEqual(0, queue.QueuedPackets.Count);

            poppedPacket = queue.PacketReceived(recievedPacket);

            Assert.IsNull(poppedPacket);
            Assert.AreEqual(0, queue.OutgoingPackets.Count);
            Assert.AreEqual(0, queue.QueuedPackets.Count);
        }

        /// <summary>
        /// Tests that a packet will be removed from the outgoing packets and a new packet is dequeued.
        /// </summary>
        [Test]
        public void TestPacketReceivedRemovedAndPopped() {
            PacketQueue queue = new PacketQueue();

            Packet firstPacketRequest = new MockPacket() {
                Origin = PacketOrigin.Client,
                Type = PacketType.Request,
                RequestId = 1
            };

            Packet secondPacketRequest = new MockPacket() {
                Origin = PacketOrigin.Client,
                Type = PacketType.Request,
                RequestId = 2
            };

            Packet firstPacketResponse = new MockPacket() {
                Origin = PacketOrigin.Client,
                Type = PacketType.Response,
                RequestId = 1
            };

            queue.PacketSend(firstPacketRequest);
            queue.PacketSend(secondPacketRequest);
            Assert.AreEqual(1, queue.OutgoingPackets.Count);
            Assert.AreEqual(1, queue.QueuedPackets.Count);

            Packet poppedPacket = queue.PacketReceived(firstPacketResponse);
            Assert.AreEqual(secondPacketRequest, poppedPacket);

            queue.PacketSend(poppedPacket);

            Assert.AreEqual(1, queue.OutgoingPackets.Count);
            Assert.AreEqual(0, queue.QueuedPackets.Count);
        }

        /// <summary>
        /// Tests that a connection restart will be required if a packet has expired (2 minutes)
        /// </summary>
        [Test]
        public void TestRestartConnectionOnQueueFailureTruey() {
            PacketQueue queue = new PacketQueue();
            Packet packet = new MockPacket() {
                Origin = PacketOrigin.Client,
                Type = PacketType.Request,
                RequestId = 1,
                Stamp = DateTime.Now.AddMinutes(-5)
            };

            Packet poppedPacket = queue.PacketSend(packet);

            Assert.AreEqual(packet, poppedPacket);
            Assert.AreEqual(1, queue.OutgoingPackets.Count);

            Assert.IsTrue(queue.RestartConnectionOnQueueFailure());
        }

        /// <summary>
        /// Tests that no restart of a connection will be required if the queue contains
        /// no old packets.
        /// </summary>
        [Test]
        public void TestRestartConnectionOnQueueFailureFalsey() {
            PacketQueue queue = new PacketQueue();
            Packet packet = new MockPacket() {
                Origin = PacketOrigin.Client,
                Type = PacketType.Request,
                RequestId = 1
            };

            Packet poppedPacket = queue.PacketSend(packet);

            Assert.AreEqual(packet, poppedPacket);
            Assert.AreEqual(1, queue.OutgoingPackets.Count);

            Assert.IsFalse(queue.RestartConnectionOnQueueFailure());
        }

        /// <summary>
        /// Tests that we can get the original request packet given a response packet tht exists
        /// </summary>
        [Test]
        public void TestGetRequestPacketExists() {
            PacketQueue queue = new PacketQueue();
            Packet packetRequest = new MockPacket() {
                Origin = PacketOrigin.Client,
                Type = PacketType.Request,
                RequestId = 1
            };

            Packet packetResponse = new MockPacket() {
                Origin = PacketOrigin.Client,
                Type = PacketType.Response,
                RequestId = 1
            };

            queue.PacketSend(packetRequest);
            Assert.AreEqual(1, queue.OutgoingPackets.Count);

            Packet fetchedRequestPacket = queue.GetRequestPacket(packetResponse);

            Assert.AreEqual(packetRequest, fetchedRequestPacket);
        }

        /// <summary>
        /// Tests that we get a null value back when no request has been requested
        /// from a response.
        /// </summary>
        [Test]
        public void TestGetRequestPacketDoesNotExists() {
            PacketQueue queue = new PacketQueue();
            Packet packetRequest = new MockPacket() {
                Origin = PacketOrigin.Client,
                Type = PacketType.Request,
                RequestId = 1
            };

            Packet packetResponse = new MockPacket() {
                Origin = PacketOrigin.Client,
                Type = PacketType.Response,
                RequestId = 2
            };

            queue.PacketSend(packetRequest);
            Assert.AreEqual(1, queue.OutgoingPackets.Count);

            Packet fetchedRequestPacket = queue.GetRequestPacket(packetResponse);

            Assert.IsNull(fetchedRequestPacket);
        }
    }
}
