using System;
using System.Collections.Generic;
using NUnit.Framework;
using Procon.Net.Shared.Actions;
using Procon.Net.Shared.Actions.Deferred;
using Procon.Net.Shared.Models;

namespace Procon.Net.Shared.Test.Actions.Deferred {
    [TestFixture]
    public class WaitingActionsTest {

        /// <summary>
        /// Tests a simple wait on a single action with a single packet will be successfully done
        /// </summary>
        [Test]
        public void TestSuccessfulSinglePacketDoneAction() {
            WaitingActions waitingActions = new WaitingActions();
            bool doneFlag = false;

            waitingActions.Done = (action, requests, responses) => { doneFlag = true; };

            waitingActions.Wait(new NetworkAction(), new List<IPacket>() {
                new Packet() {
                    Origin = PacketOrigin.Client,
                    Type = PacketType.Request,
                    RequestId = 1
                }
            });

            waitingActions.Mark(new Packet() {
                Origin = PacketOrigin.Client,
                Type = PacketType.Response,
                RequestId = 1
            });

            Assert.IsTrue(doneFlag);
        }

        /// <summary>
        /// Tests that done will not be fired if a packet with a different request id is passed in
        /// that does not match the waiting packet.
        /// </summary>
        [Test]
        public void TestFailedDoneAction() {
            WaitingActions waitingActions = new WaitingActions();
            bool doneFlag = false;

            waitingActions.Done = (action, requests, responses) => { doneFlag = true; };

            waitingActions.Wait(new NetworkAction(), new List<IPacket>() {
                new Packet() {
                    Origin = PacketOrigin.Client,
                    Type = PacketType.Request,
                    RequestId = 1
                }
            });

            waitingActions.Mark(new Packet() {
                Origin = PacketOrigin.Client,
                Type = PacketType.Response,
                RequestId = 2
            });

            Assert.IsFalse(doneFlag);
        }

        /// <summary>
        /// Tests an action waiting for two packets will be successful
        /// </summary>
        [Test]
        public void TestSuccessfulDoublePacketDoneAction() {
            WaitingActions waitingActions = new WaitingActions();
            bool doneFlag = false;

            waitingActions.Done = (action, requests, responses) => { doneFlag = true; };

            waitingActions.Wait(new NetworkAction(), new List<IPacket>() {
                new Packet() {
                    Origin = PacketOrigin.Client,
                    Type = PacketType.Request,
                    RequestId = 1
                },
                new Packet() {
                    Origin = PacketOrigin.Client,
                    Type = PacketType.Request,
                    RequestId = 2
                }
            });

            waitingActions.Mark(new Packet() {
                Origin = PacketOrigin.Client,
                Type = PacketType.Response,
                RequestId = 1
            });

            waitingActions.Mark(new Packet() {
                Origin = PacketOrigin.Client,
                Type = PacketType.Response,
                RequestId = 2
            });

            Assert.IsTrue(doneFlag);
        }

        /// <summary>
        /// Tests an action waiting for two packets won't successful complete if two identical packets are
        /// passed in, but it's waiting for two different packets
        /// </summary>
        [Test]
        public void TestFailedDoublePacketActionWithTwoSameResponses() {
            WaitingActions waitingActions = new WaitingActions();
            bool doneFlag = false;

            waitingActions.Done = (action, requests, responses) => { doneFlag = true; };

            waitingActions.Wait(new NetworkAction(), new List<IPacket>() {
                new Packet() {
                    Origin = PacketOrigin.Client,
                    Type = PacketType.Request,
                    RequestId = 1
                },
                new Packet() {
                    Origin = PacketOrigin.Client,
                    Type = PacketType.Request,
                    RequestId = 2
                }
            });

            waitingActions.Mark(new Packet() {
                Origin = PacketOrigin.Client,
                Type = PacketType.Response,
                RequestId = 1
            });

            // Now send in the same packet
            waitingActions.Mark(new Packet() {
                Origin = PacketOrigin.Client,
                Type = PacketType.Response,
                RequestId = 1
            });

            Assert.IsFalse(doneFlag);
        }

        /// <summary>
        /// Tests actions will be expired after their expiration time has elapsed
        /// </summary>
        [Test]
        public void TestExpiredAction() {
            WaitingActions waitingActions = new WaitingActions();
            bool expiredFlag = false;

            waitingActions.Expired = (action, requests, responses) => { expiredFlag = true; };

            waitingActions.Wait(new NetworkAction(), new List<IPacket>() {
                new Packet() {
                    Origin = PacketOrigin.Client,
                    Type = PacketType.Request,
                    RequestId = 1
                }
            }, DateTime.Now.AddSeconds(-1));

            waitingActions.Flush();

            Assert.IsTrue(expiredFlag);
            Assert.AreEqual(0, waitingActions.Waiting.Count);
        }
    }
}
