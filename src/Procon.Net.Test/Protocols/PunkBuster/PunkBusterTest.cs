using NUnit.Framework;
using Procon.Net.Protocols.PunkBuster;
using Procon.Net.Protocols.PunkBuster.Packets;

namespace Procon.Net.Test.Protocols.PunkBuster {
    [TestFixture]
    public class PunkBusterTest {

        /// <summary>
        /// Tests a punkbuster player from a player list will match
        /// </summary>
        [Test]
        public void TestPunkBusterSerializerPlayer() {
            IPunkBuster punkBuster = PunkBusterSerializer.Deserialize("PunkBuster Server: 1  b88b60a36365592b1ae94fa04c5763ed(-) 111.222.0.1:3659 OK   1 3.0 0 (V) \"PhogueZero\"");

            Assert.IsInstanceOf<PunkBusterPlayer>(punkBuster);

            PunkBusterPlayer player = punkBuster as PunkBusterPlayer;

            Assert.AreEqual(1, player.SlotId);
            Assert.AreEqual("b88b60a36365592b1ae94fa04c5763ed", player.Guid);
            Assert.AreEqual("111.222.0.1:3659", player.Ip);
            Assert.AreEqual("PhogueZero", player.Name);
        }

        /// <summary>
        /// Tests a punkbuster player list begin will match
        /// </summary>
        [Test]
        public void TestPunkBusterSerializerBeginPlayerList() {
            IPunkBuster punkBuster = PunkBusterSerializer.Deserialize("PunkBuster Server: Player List: [Slot #] [GUID] [Address] [Status] [Power] [Auth Rate] [Recent SS] [O/S] [Name]");

            Assert.IsInstanceOf<PunkBusterBeginPlayerList>(punkBuster);
        }

        /// <summary>
        /// Tests a punkbuster player list end will match
        /// </summary>
        [Test]
        public void TestPunkBusterSerializerEndPlayerList() {
            IPunkBuster punkBuster = PunkBusterSerializer.Deserialize("PunkBuster Server: End of Player List (1 Players)");

            Assert.IsInstanceOf<PunkBusterEndPlayerList>(punkBuster);

            PunkBusterEndPlayerList endPlayerList = punkBuster as PunkBusterEndPlayerList;

            Assert.AreEqual(1, endPlayerList.PlayerCount);
        }
    }
}
