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
using Procon.Net.Shared.Actions;
using Procon.Net.Shared.Serialization;

namespace Procon.Net.Shared.Test.TestSerialization.TestJson {
    [TestFixture]
    public class TestClientEventArgs {
        /// <summary>
        /// Tests a default serialization is successful
        /// </summary>
        [Test]
        public void TestDefaultSerialization() {
            var original = new ClientEventArgs();
            var serialized = JsonSerialization.Minimal.Serialize(original);
            var deseralized = JsonSerialization.Minimal.Deserialize<ClientEventArgs>(serialized);

            Assert.IsNotNull(deseralized.Then);
            Assert.IsNotNull(deseralized.Now);
            Assert.IsNotNull(deseralized.Stamp);
            Assert.AreEqual(ConnectionState.ConnectionDisconnected, deseralized.ConnectionState);
            Assert.AreEqual(ClientEventType.None, deseralized.EventType);
        }

        /// <summary>
        /// Tests a populated object with attributes immediately attached to the type will serialize successfully.
        /// </summary>
        [Test]
        public void TestSingleDepthPopulationSerialization() {
            var original = new ClientEventArgs() {
                ConnectionState = ConnectionState.ConnectionLoggedIn,
                EventType = ClientEventType.ClientConnectionStateChange,
                Now = {
                    Actions = new List<INetworkAction>() {
                        new NetworkAction() {
                            Uid = Guid.Empty
                        }
                    },
                    Exceptions = new List<String>() {
                        ""
                    },
                    Packets = new List<IPacket>() {
                        new Packet() {
                            Stamp = new DateTime(2000, 10, 10, 10, 10, 10)
                        }
                    }
                },
                Then = {
                    Actions = new List<INetworkAction>() {
                        new NetworkAction() {
                            Uid = Guid.Empty
                        }
                    },
                    Exceptions = new List<String>() {
                        ""
                    },
                    Packets = new List<IPacket>() {
                        new Packet() {
                            Stamp = new DateTime(2000, 10, 10, 10, 10, 10)
                        }
                    }
                }
            };
            var serialized = JsonSerialization.Minimal.Serialize(original);
            var deseralized = JsonSerialization.Minimal.Deserialize<ClientEventArgs>(serialized);

            Assert.IsNotNull(deseralized.Then);
            Assert.IsNotNull(deseralized.Now);
            Assert.IsNotNull(deseralized.Stamp);
            Assert.AreEqual(ConnectionState.ConnectionLoggedIn, deseralized.ConnectionState);
            Assert.AreEqual(ClientEventType.ClientConnectionStateChange, deseralized.EventType);
            Assert.IsNotEmpty(deseralized.Then.Exceptions);
            Assert.IsNotEmpty(deseralized.Then.Packets);
            Assert.IsNotEmpty(deseralized.Then.Actions);
            Assert.IsNotEmpty(deseralized.Now.Exceptions);
            Assert.IsNotEmpty(deseralized.Now.Packets);
            Assert.IsNotEmpty(deseralized.Now.Actions);
        }
    }
}
