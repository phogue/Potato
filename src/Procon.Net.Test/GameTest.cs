using System.Threading;
using NUnit.Framework;
using Procon.Net.Test.Mocks;

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
    }
}
