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
            Assert.AreEqual(@"{""Exceptions"":null,""Packets"":null,""Actions"":null}", Json.Minimal.Serialize(new ClientEventData()));
        }

        /// <summary>
        /// Tests default deserialization is successful
        /// </summary>
        [Test]
        public void TestDefaultDeserialization() {
            var deseralized = Json.Minimal.Deserialize<ClientEventData>(@"{""Exceptions"":null,""Packets"":null,""Actions"":null}");

            Assert.IsNull(deseralized.Exceptions);
            Assert.IsNull(deseralized.Packets);
            Assert.IsNull(deseralized.Actions);
        }

        /// <summary>
        /// Tests a populated object with attributes immediately attached to the type will serialize successfully.
        /// </summary>
        [Test]
        public void TestSingleDepthPopulationSerialization() {
            var serialized = Json.Minimal.Serialize(new ClientEventData() {
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
            });

            Assert.AreEqual(@"{""Exceptions"":[""""],""Packets"":[{""Stamp"":""2000-10-10T10:10:10"",""Origin"":0,""Type"":0,""RequestId"":null,""Data"":null,""Text"":null,""DebugText"":null,""Words"":[],""RemoteEndPoint"":null}],""Actions"":[{""Name"":null,""ActionType"":0,""Uid"":""00000000-0000-0000-0000-000000000000"",""Origin"":0,""Scope"":{""Players"":null,""Content"":null,""Groups"":null,""Points"":null,""Items"":null,""Maps"":null,""Times"":null,""HumanHitLocations"":null},""Then"":{""Players"":null,""Content"":null,""Groups"":null,""Points"":null,""Items"":null,""Maps"":null,""Times"":null,""HumanHitLocations"":null},""Now"":{""Players"":null,""Content"":null,""Groups"":null,""Points"":null,""Items"":null,""Maps"":null,""Times"":null,""HumanHitLocations"":null}}]}", serialized);
        }

        /// <summary>
        /// Tests a populated object with attributes immediately attached to the type will deserialize successfully.
        /// </summary>
        [Test]
        public void TestSingleDepthPopulationDeserialization() {
            var deseralized = Json.Minimal.Deserialize<ClientEventData>(@"{""Exceptions"":[""""],""Packets"":[{""Stamp"":""2000-10-10T10:10:10"",""Origin"":0,""Type"":0,""RequestId"":null,""Data"":null,""Text"":null,""DebugText"":null,""Words"":[],""RemoteEndPoint"":null}],""Actions"":[{""Name"":null,""ActionType"":0,""Uid"":""00000000-0000-0000-0000-000000000000"",""Origin"":0,""Scope"":{""Players"":null,""Content"":null,""Groups"":null,""Points"":null,""Items"":null,""Maps"":null,""Times"":null,""HumanHitLocations"":null},""Then"":{""Players"":null,""Content"":null,""Groups"":null,""Points"":null,""Items"":null,""Maps"":null,""Times"":null,""HumanHitLocations"":null},""Now"":{""Players"":null,""Content"":null,""Groups"":null,""Points"":null,""Items"":null,""Maps"":null,""Times"":null,""HumanHitLocations"":null}}]}");

            Assert.IsNotEmpty(deseralized.Exceptions);
            Assert.IsNotEmpty(deseralized.Packets);
            Assert.IsNotEmpty(deseralized.Actions);

        }
    }
}
