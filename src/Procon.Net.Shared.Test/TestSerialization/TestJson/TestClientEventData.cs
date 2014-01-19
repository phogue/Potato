using System;
using System.Collections.Generic;
using NUnit.Framework;
using Procon.Net.Shared.Actions;
using Procon.Net.Shared.Serialization;

namespace Procon.Net.Shared.Test.TestSerialization.TestJson {
    [TestFixture]
    public class TestClientEventData  {
        /// <summary>
        /// Tests a default serialization is successful
        /// </summary>
        [Test]
        public void TestDefaultSerialization() {
            var original = new ClientEventData();
            var serialized = JsonSerialization.Minimal.Serialize(original);
            var deseralized = JsonSerialization.Minimal.Deserialize<ClientEventData>(serialized);

            Assert.IsNull(deseralized.Exceptions);
            Assert.IsNull(deseralized.Packets);
            Assert.IsNull(deseralized.Actions);
        }

        /// <summary>
        /// Tests a populated object with attributes immediately attached to the type will serialize successfully.
        /// </summary>
        [Test]
        public void TestSingleDepthPopulationSerialization() {
            var original = new ClientEventData() {
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
            };
            var serialized = JsonSerialization.Minimal.Serialize(original);
            var deseralized = JsonSerialization.Minimal.Deserialize<ClientEventData>(serialized);

            Assert.IsNotEmpty(deseralized.Exceptions);
            Assert.IsNotEmpty(deseralized.Packets);
            Assert.IsNotEmpty(deseralized.Actions);
        }
    }
}
