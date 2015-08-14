#region Copyright
// Copyright 2014 Myrcon Pty. Ltd.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System;
using NUnit.Framework;
using Potato.Net.Shared.Test.Mocks;

namespace Potato.Net.Shared.Test {
    [TestFixture]
    public class PacketQueueTest {

        /// <summary>
        /// Tests that a packet is returned immediately from telling the queue it has been sent,
        /// inferring the packet is not queued and should be sent immediately.
        /// </summary>
        [Test]
        public void TestPacketSendImmediate() {

            var queue = new PacketQueue();
            IPacketWrapper packet = new MockPacket() {
                Packet = {
                    Origin = PacketOrigin.Client,
                    Type = PacketType.Request,
                    RequestId = 1
                }
            };

            var poppedPacket = queue.PacketSend(packet);

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

            var queue = new PacketQueue();
            IPacketWrapper firstPacket = new MockPacket() {
                Packet = {
                    Origin = PacketOrigin.Client,
                    Type = PacketType.Request,
                    RequestId = 1
                }
            };

            var poppedPacket = queue.PacketSend(firstPacket);

            // Client would send to the server.

            Assert.AreEqual(firstPacket, poppedPacket);
            Assert.AreEqual(1, queue.OutgoingPackets.Count);

            IPacketWrapper secondPacket = new MockPacket() {
                Packet = {
                    Origin = PacketOrigin.Client,
                    Type = PacketType.Request,
                    RequestId = 1
                }
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

            var queue = new PacketQueue();

            IPacketWrapper sentPacket = new MockPacket() {
                Packet = {
                    Origin = PacketOrigin.Client,
                    Type = PacketType.Request,
                    RequestId = 1
                }
            };

            IPacketWrapper recievedPacket = new MockPacket() {
                Packet = {
                    Origin = PacketOrigin.Client,
                    Type = PacketType.Response,
                    RequestId = 1
                }
            };

            var poppedPacket = queue.PacketSend(sentPacket);

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
            var queue = new PacketQueue();

            IPacketWrapper firstPacketRequest = new MockPacket() {
                Packet = {
                    Origin = PacketOrigin.Client,
                    Type = PacketType.Request,
                    RequestId = 1
                }
            };

            IPacketWrapper secondPacketRequest = new MockPacket() {
                Packet = {
                    Origin = PacketOrigin.Client,
                    Type = PacketType.Request,
                    RequestId = 2
                }
            };

            IPacketWrapper firstPacketResponse = new MockPacket() {
                Packet = {
                    Origin = PacketOrigin.Client,
                    Type = PacketType.Response,
                    RequestId = 1
                }
            };

            queue.PacketSend(firstPacketRequest);
            queue.PacketSend(secondPacketRequest);
            Assert.AreEqual(1, queue.OutgoingPackets.Count);
            Assert.AreEqual(1, queue.QueuedPackets.Count);

            var poppedPacket = queue.PacketReceived(firstPacketResponse);
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
            var queue = new PacketQueue();
            IPacketWrapper packet = new MockPacket() {
                Packet = {
                    Origin = PacketOrigin.Client,
                    Type = PacketType.Request,
                    RequestId = 1,
                    Stamp = DateTime.Now.AddMinutes(-5)
                }
            };

            var poppedPacket = queue.PacketSend(packet);

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
            var queue = new PacketQueue();
            IPacketWrapper packet = new MockPacket() {
                Packet = {
                    Origin = PacketOrigin.Client,
                    Type = PacketType.Request,
                    RequestId = 1
                }
            };

            var poppedPacket = queue.PacketSend(packet);

            Assert.AreEqual(packet, poppedPacket);
            Assert.AreEqual(1, queue.OutgoingPackets.Count);

            Assert.IsFalse(queue.RestartConnectionOnQueueFailure());
        }

        /// <summary>
        /// Tests that we can get the original request packet given a response packet tht exists
        /// </summary>
        [Test]
        public void TestGetRequestPacketExists() {
            var queue = new PacketQueue();
            IPacketWrapper packetRequest = new MockPacket() {
                Packet = {
                    Origin = PacketOrigin.Client,
                    Type = PacketType.Request,
                    RequestId = 1
                }
            };

            IPacketWrapper packetResponse = new MockPacket() {
                Packet = {
                    Origin = PacketOrigin.Client,
                    Type = PacketType.Response,
                    RequestId = 1
                }
            };

            queue.PacketSend(packetRequest);
            Assert.AreEqual(1, queue.OutgoingPackets.Count);

            var fetchedRequestPacket = queue.GetRequestPacket(packetResponse);

            Assert.AreEqual(packetRequest, fetchedRequestPacket);
        }

        /// <summary>
        /// Tests that we get a null value back when no request has been requested
        /// from a response.
        /// </summary>
        [Test]
        public void TestGetRequestPacketDoesNotExists() {
            var queue = new PacketQueue();
            IPacketWrapper packetRequest = new MockPacket() {
                Packet = {
                    Origin = PacketOrigin.Client,
                    Type = PacketType.Request,
                    RequestId = 1
                }
            };

            IPacketWrapper packetResponse = new MockPacket() {
                Packet = {
                    Origin = PacketOrigin.Client,
                    Type = PacketType.Response,
                    RequestId = 2
                }
            };

            queue.PacketSend(packetRequest);
            Assert.AreEqual(1, queue.OutgoingPackets.Count);

            var fetchedRequestPacket = queue.GetRequestPacket(packetResponse);

            Assert.IsNull(fetchedRequestPacket);
        }
    }
}
