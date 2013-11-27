using System.Collections.Generic;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using Procon.Net.Actions;
using Procon.Net.Test.Mocks;
using Procon.Net.Test.Mocks.Game;

namespace Procon.Net.Test {
    [TestFixture]
    public class GameTest {

        /// <summary>
        /// Tests the attribute will be fetched and converted to a GameType object.
        /// </summary>
        [Test]
        public void TestGameTypeAttributeConversion() {
            MockGame game = new MockGame("localhost", 5000);

            Assert.AreEqual("MOCK_3", game.GameType.Type);
            Assert.AreEqual("MockGame 3", game.GameType.Name);
            Assert.AreEqual("Myrcon", game.GameType.Provider);
            Assert.IsInstanceOf<GameType>(game.GameType);
        }

        /// <summary>
        /// Simply tests that actions are dispatched correctly. Coverage.
        /// </summary>
        [Test]
        public void TestGameActionDispatch() {
            List<IPacket> packets = new List<IPacket>();

            MockActionDispatchGame game = new MockActionDispatchGame("localhost", 5000);

            packets.AddRange(game.Action(new Chat()));
            packets.AddRange(game.Action(new Kick()));
            packets.AddRange(game.Action(new Ban()));
            packets.AddRange(game.Action(new Map()));
            packets.AddRange(game.Action(new Kill()));
            packets.AddRange(game.Action(new Move()));
            packets.AddRange(game.Action(new Raw()));

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
            MockActionChatNullResultGame game = new MockActionChatNullResultGame("localhost", 5000);

            List<IPacket> packets = game.Action(new Chat());

            Assert.AreEqual(0, packets.Count);
        }
    }
}
