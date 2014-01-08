using NUnit.Framework;

namespace Procon.Service.Shared.Test {
    [TestFixture]
    public class TestServiceMessage {
        /// <summary>
        /// Tests the initial properties are setup
        /// </summary>
        [Test]
        public void TestInitialValues() {
            var message = new ServiceMessage();

            Assert.IsNotNull(message.Arguments);
            Assert.IsNotNull(message.Stamp);
        }

        /// <summary>
        /// Tests values are nulled out on dispose
        /// </summary>
        [Test]
        public void TestDispose() {
            var message = new ServiceMessage();

            message.Dispose();

            Assert.IsNull(message.Arguments);
            Assert.IsNull(message.Name);
        }
    }
}
