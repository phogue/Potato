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
using Procon.Net.Shared;
using Procon.Net.Test.Mocks;

namespace Procon.Net.Test {
    /// <summary>
    /// These tests are for coverage, nothing more.
    /// </summary>
    [TestFixture]
    public class PacketTest {

        [Test]
        public void TestPacketEmptyConstructor() {
            MockPacket packet = new MockPacket();

            Assert.AreEqual(PacketOrigin.None, packet.Packet.Origin);
            Assert.AreEqual(PacketType.None, packet.Packet.Type);
            Assert.IsNull(packet.Packet.RequestId);
            Assert.GreaterOrEqual(DateTime.Now, packet.Packet.Stamp);
        }

        [Test]
        public void TestPacketParameterConstructor() {
            MockPacket packet = new MockPacket() {
                Packet = {
                    Origin = PacketOrigin.Server,
                    Type = PacketType.Response
                }
            };

            Assert.AreEqual(PacketOrigin.Server, packet.Packet.Origin);
            Assert.AreEqual(PacketType.Response, packet.Packet.Type);
            Assert.IsNull(packet.Packet.RequestId);
            Assert.GreaterOrEqual(DateTime.Now, packet.Packet.Stamp);
        }
    }
}
