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
using System.Collections.Generic;
using NUnit.Framework;
using Procon.Net.Shared;
using Procon.Net.Test.Mocks;

namespace Procon.Net.Test {

    [TestFixture]
    public class PacketDispatcherTest {

        /// <summary>
        /// Tests that a packet will be successfully dispatched.
        /// </summary>
        [Test]
        public void TestMockPacketDispatcherSuccess() {
            bool dispatched = false;

            MockPacketDispatcher dispatcher = new MockPacketDispatcher();

            dispatcher.Append(new Dictionary<IPacketDispatch, Action<IPacketWrapper, IPacketWrapper>>() {
                { 
                    new PacketDispatch() {
                        Name = "TestMockPacketDispatcherSuccess", Origin = PacketOrigin.Client
                    }, (request, response) => {
                        dispatched = true;
                    } 
                }
            });

            dispatcher.Dispatch(new MockPacket() {
                Packet = {
                    Origin = PacketOrigin.Client,
                    RequestId = 2,
                    Type = PacketType.Request,
                },
                Text = "TestMockPacketDispatcherSuccess",
            });

            Assert.IsTrue(dispatched);
        }

        /// <summary>
        /// Tests that a dispatch will fail when the origin is different.
        /// </summary>
        [Test]
        public void TestMockPacketDispatcherDifferentOriginFailed() {
            bool dispatched = false;
            bool failed = false;

            MockPacketDispatcher dispatcher = new MockPacketDispatcher() {
                MissingDispatchHandler = (identifer, request, response) => { failed = true; }
            };

            dispatcher.Append(new Dictionary<IPacketDispatch, Action<IPacketWrapper, IPacketWrapper>>() {
                { 
                    new PacketDispatch() {
                        Name = "TestMockPacketDispatcherDifferentOriginFailed", Origin = PacketOrigin.Client
                    }, (request, response) => {
                        dispatched = true;
                    } 
                }
            });

            dispatcher.Dispatch(new MockPacket() {
                Packet = {
                    Origin = PacketOrigin.Server,
                    RequestId = 2,
                    Type = PacketType.Request,
                },
                Text = "TestMockPacketDispatcherDifferentOriginFailed",
            });

            Assert.IsFalse(dispatched);
            Assert.IsTrue(failed);
        }

        /// <summary>
        /// Tests that if origin matches, but a method name is not found the dispatch will fail.
        /// </summary>
        [Test]
        public void TestMockPacketDispatcherMissingDispatchNameFailed() {
            bool dispatched = false;
            bool failed = false;

            MockPacketDispatcher dispatcher = new MockPacketDispatcher() {
                MissingDispatchHandler = (identifer, request, response) => { failed = true; }
            };

            dispatcher.Append(new Dictionary<IPacketDispatch, Action<IPacketWrapper, IPacketWrapper>>() {
                { 
                    new PacketDispatch() {
                        Name = "TestMockPacketDispatcherMissingDispatchNameFailed", Origin = PacketOrigin.Client
                    }, (request, response) => {
                        dispatched = true;
                    } 
                }
            });

            dispatcher.Dispatch(new MockPacket() {
                Packet = {
                    Origin = PacketOrigin.Server,
                    RequestId = 2,
                    Type = PacketType.Request,
                },
                Text = "ThisDoesNotExist",
            });

            Assert.IsFalse(dispatched);
            Assert.IsTrue(failed);
        }

        /// <summary>
        /// Tests that appending two methods with identical dispatchers will replace the previous handler.
        /// Useful when overwriting functionality in a subclass of a game.
        /// </summary>
        [Test]
        public void TestMockPacketDispatcherReplacedDispatcherSuccess() {
            int handler = 0;

            MockPacketDispatcher dispatcher = new MockPacketDispatcher();

            dispatcher.Append(new Dictionary<IPacketDispatch, Action<IPacketWrapper, IPacketWrapper>>() {
                { 
                    new PacketDispatch() {
                        Name = "TestMockPacketDispatcherReplacedDispatcherSuccess", Origin = PacketOrigin.Client
                    }, (request, response) => {
                        handler = 1;
                    } 
                }
            });

            dispatcher.Append(new Dictionary<IPacketDispatch, Action<IPacketWrapper, IPacketWrapper>>() {
                { 
                    new PacketDispatch() {
                        Name = "TestMockPacketDispatcherReplacedDispatcherSuccess", Origin = PacketOrigin.Client
                    }, (request, response) => {
                        handler = 2;
                    } 
                }
            });

            dispatcher.Dispatch(new MockPacket() {
                Packet = {
                    Origin = PacketOrigin.Client,
                    RequestId = 2,
                    Type = PacketType.Request,
                },
                Text = "TestMockPacketDispatcherReplacedDispatcherSuccess"
            });

            Assert.AreEqual(2, handler);
        }
    }
}
