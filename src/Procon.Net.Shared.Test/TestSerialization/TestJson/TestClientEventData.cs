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
