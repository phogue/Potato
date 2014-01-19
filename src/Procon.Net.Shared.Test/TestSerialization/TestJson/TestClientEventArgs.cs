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
            var serialized = Json.Minimal.Serialize(original);
            var deseralized = Json.Minimal.Deserialize<ClientEventArgs>(serialized);

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
            var serialized = Json.Minimal.Serialize(original);
            var deseralized = Json.Minimal.Deserialize<ClientEventArgs>(serialized);

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
