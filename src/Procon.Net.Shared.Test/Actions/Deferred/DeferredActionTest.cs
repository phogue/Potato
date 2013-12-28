using System.Collections.Generic;
using NUnit.Framework;
using Procon.Net.Shared.Actions;
using Procon.Net.Shared.Actions.Deferred;

namespace Procon.Net.Shared.Test.Actions.Deferred {
    [TestFixture]
    public class DeferredActionTest {

        /// <summary>
        /// Fetches the current action attached to the deferred action. Coverage. Just coverage.
        /// </summary>
        [Test]
        public void TestDeferredActionGetAction() {
            Chat chat = new Chat();

            DeferredAction<Chat> deferredAction = new DeferredAction<Chat>() {
                Action = chat
            };
            
            Assert.AreEqual(chat.Uid, deferredAction.GetAction().Uid);
        }

        /// <summary>
        /// Tests an action's each callback will be called.
        /// </summary>
        [Test]
        public void TestDeferredActionEach() {
            bool eachFlag = false;
            Chat chat = new Chat();

            DeferredAction<Chat> deferredAction = new DeferredAction<Chat>() {
                Action = chat,
                Each = (action, request, response) => {
                    eachFlag = true;
                }
            };

            Assert.IsTrue(deferredAction.TryInsertDone(chat, new List<IPacket>() {
                new Packet() {
                    Origin = PacketOrigin.Client,
                    Type = PacketType.Request,
                    RequestId = 1
                }
            }, new List<IPacket>() {
                new Packet() {
                    Origin = PacketOrigin.Client,
                    Type = PacketType.Response,
                    RequestId = 1
                }
            }));

            Assert.IsTrue(eachFlag);
        }

        /// <summary>
        /// Tests an action's done callback will be called.
        /// </summary>
        [Test]
        public void TestDeferredActionDone() {
            bool doneFlag = false;
            Chat chat = new Chat();

            DeferredAction<Chat> deferredAction = new DeferredAction<Chat>() {
                Action = chat,
                Done = (action, requests, responses) => {
                    doneFlag = true;
                }
            };

            Assert.IsTrue(deferredAction.TryInsertDone(chat, new List<IPacket>() {
                new Packet() {
                    Origin = PacketOrigin.Client,
                    Type = PacketType.Request,
                    RequestId = 1
                }
            }, new List<IPacket>() {
                new Packet() {
                    Origin = PacketOrigin.Client,
                    Type = PacketType.Response,
                    RequestId = 1
                }
            }));
            Assert.IsTrue(doneFlag);
        }

        /// <summary>
        /// Tests an action's sent callback will be called.
        /// </summary>
        [Test]
        public void TestDeferredActionSent() {
            bool sentFlag = false;
            Chat chat = new Chat();

            DeferredAction<Chat> deferredAction = new DeferredAction<Chat>() {
                Action = chat,
                Sent = (action, requests) => {
                    sentFlag = true;
                }
            };

            Assert.IsTrue(deferredAction.TryInsertSent(chat, new List<IPacket>() {
                new Packet() {
                    Origin = PacketOrigin.Client,
                    Type = PacketType.Request,
                    RequestId = 1
                }
            }));
            Assert.IsTrue(sentFlag);
        }

        /// <summary>
        /// Tests an action's expired callback will be called.
        /// </summary>
        [Test]
        public void TestDeferredActionExpired() {
            bool expiredFlag = false;
            Chat chat = new Chat();

            DeferredAction<Chat> deferredAction = new DeferredAction<Chat>() {
                Action = chat,
                Expired = (action, requests, responses) => {
                    expiredFlag = true;
                }
            };

            Assert.IsTrue(deferredAction.TryInsertExpired(chat, new List<IPacket>() {
                new Packet() {
                    Origin = PacketOrigin.Client,
                    Type = PacketType.Request,
                    RequestId = 1
                }
            }, new List<IPacket>() {
                new Packet() {
                    Origin = PacketOrigin.Client,
                    Type = PacketType.Response,
                    RequestId = 1
                }
            }));

            Assert.IsTrue(expiredFlag);
        }

        /// <summary>
        /// Tests an action's always callback will be called.
        /// </summary>
        [Test]
        public void TestDeferredActionAlways() {
            bool alwaysFlag = false;
            Chat chat = new Chat();

            DeferredAction<Chat> deferredAction = new DeferredAction<Chat>() {
                Action = chat,
                Always = action => {
                    alwaysFlag = true;
                }
            };

            Assert.IsTrue(deferredAction.TryInsertAlways(chat));

            Assert.IsTrue(alwaysFlag);
        }

        /// <summary>
        /// Tests callbacks will be nulled out on release.
        /// </summary>
        [Test]
        public void TestDeferredActionRelease() {
            Chat chat = new Chat();

            DeferredAction<Chat> deferredAction = new DeferredAction<Chat>() {
                Action = chat,
                Sent = (action, requests) => {

                },
                Each = (action, request, response) => {
                    
                },
                Done = (action, requests, responses) => {
                    
                },
                Expired = (action, requests, responses) => {
                    
                },
                Always = action => {
                    
                }
            };

            deferredAction.Release();

            Assert.IsNull(deferredAction.Sent);
            Assert.IsNull(deferredAction.Each);
            Assert.IsNull(deferredAction.Done);
            Assert.IsNull(deferredAction.Expired);
            Assert.IsNull(deferredAction.Always);
        }
    }
}
