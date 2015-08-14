#region Copyright
// Copyright 2015 Geoff Green.
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
using System.Collections.Generic;
using NUnit.Framework;
using Potato.Net.Shared.Actions;
using Potato.Net.Shared.Actions.Deferred;

namespace Potato.Net.Shared.Test.Actions.Deferred {
    [TestFixture]
    public class DeferredActionTest {

        /// <summary>
        /// Fetches the current action attached to the deferred action. Coverage. Just coverage.
        /// </summary>
        [Test]
        public void TestDeferredActionGetAction() {
            INetworkAction chat = new NetworkAction() {
                ActionType = NetworkActionType.NetworkTextSay
            };

            var deferredAction = new DeferredAction<INetworkAction>() {
                Action = chat
            };
            
            Assert.AreEqual(chat.Uid, deferredAction.GetAction().Uid);
        }

        /// <summary>
        /// Tests an action's each callback will be called.
        /// </summary>
        [Test]
        public void TestDeferredActionEach() {
            var eachFlag = false;
            INetworkAction chat = new NetworkAction();

            var deferredAction = new DeferredAction<INetworkAction>() {
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
            var doneFlag = false;
            INetworkAction chat = new NetworkAction();

            var deferredAction = new DeferredAction<INetworkAction>() {
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
            var sentFlag = false;
            INetworkAction chat = new NetworkAction();

            var deferredAction = new DeferredAction<INetworkAction>() {
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
            var expiredFlag = false;
            INetworkAction chat = new NetworkAction();

            var deferredAction = new DeferredAction<INetworkAction>() {
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
            var alwaysFlag = false;
            INetworkAction chat = new NetworkAction();

            var deferredAction = new DeferredAction<INetworkAction>() {
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
            INetworkAction chat = new NetworkAction();

            var deferredAction = new DeferredAction<INetworkAction>() {
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
