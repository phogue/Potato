using NUnit.Framework;
using Procon.Config.Core;

namespace Procon.Config.Test.TestConfigController {
    [TestFixture]
    public class TestDispatch {
        /// <summary>
        /// Tests a null result when passing in a nulled command
        /// </summary>
        [Test]
        public void TestNulledResultForNulledCommand() {
            Assert.IsNull(ConfigController.Dispatch(null, null));
        }

        /// <summary>
        /// Tests a null result from passing in an unknown command
        /// </summary>
        [Test]
        public void TestNulledResultForUnknownCommand() {
            Assert.IsNull(ConfigController.Dispatch("This command does not exist", null));
        }
    }
}
