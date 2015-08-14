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
using System.Linq;
using NUnit.Framework;
using Potato.Net.Shared.Actions;
using Potato.Net.Shared.Test.Mocks;
using Potato.Net.Shared.Test.Mocks.Game;

namespace Potato.Net.Shared.Test {
    [TestFixture]
    public class GameTest {

        /// <summary>
        /// Tests the attribute will be fetched and converted to a GameType object.
        /// </summary>
        [Test]
        public void TestGameTypeAttributeConversion() {
            var game = new MockGame();

            Assert.AreEqual("MOCK_3", game.ProtocolType.Type);
            Assert.AreEqual("MockGame 3", game.ProtocolType.Name);
            Assert.AreEqual("Myrcon", game.ProtocolType.Provider);
            Assert.IsInstanceOf<ProtocolType>(game.ProtocolType);
        }

        /// <summary>
        /// Simply tests that actions are dispatched correctly. Coverage.
        /// </summary>
        [Test]
        public void TestGameActionDispatch() {
            var packets = new List<IPacket>();

            var game = new MockActionDispatchGame();

            packets.AddRange(game.Action(new NetworkAction() { ActionType = NetworkActionType.NetworkTextSay }));
            packets.AddRange(game.Action(new NetworkAction() { ActionType = NetworkActionType.NetworkPlayerKick }));
            packets.AddRange(game.Action(new NetworkAction() { ActionType = NetworkActionType.NetworkPlayerBan }));
            packets.AddRange(game.Action(new NetworkAction() { ActionType = NetworkActionType.NetworkMapAppend }));
            packets.AddRange(game.Action(new NetworkAction() { ActionType = NetworkActionType.NetworkPlayerKill }));
            packets.AddRange(game.Action(new NetworkAction() { ActionType = NetworkActionType.NetworkPlayerMove }));
            packets.AddRange(game.Action(new NetworkAction() { ActionType = NetworkActionType.NetworkPacketSend }));

            Assert.IsTrue(packets.Any(packet => packet.Text == "Chat"));
            Assert.IsTrue(packets.Any(packet => packet.Text == "Kick"));
            Assert.IsTrue(packets.Any(packet => packet.Text == "Ban"));
            Assert.IsTrue(packets.Any(packet => packet.Text == "Map"));
            Assert.IsTrue(packets.Any(packet => packet.Text == "Kill"));
            Assert.IsTrue(packets.Any(packet => packet.Text == "Move"));
            Assert.IsTrue(packets.Any(packet => packet.Text == "Chat"));
        }

        /// <summary>
        /// Tests that null values are removed during a dispatch.
        /// </summary>
        [Test]
        public void TestGameActionDispatchNullsRemoved() {
            var game = new MockActionChatNullResultGame();

            var packets = game.Action(new NetworkAction() {
                ActionType = NetworkActionType.NetworkTextSay
            });

            Assert.AreEqual(0, packets.Count);
        }
    }
}
