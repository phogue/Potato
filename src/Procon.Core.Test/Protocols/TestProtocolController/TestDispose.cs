using NUnit.Framework;
using Procon.Core.Protocols;

namespace Procon.Core.Test.Protocols.TestProtocolController {
    [TestFixture]
    public class TestDispose {
        /// <summary>
        /// Tests all data is removed during dispose.
        /// </summary>
        [Test]
        public void TestSuccess() {
            var protocols = new ProtocolController();

            protocols.Dispose();

            Assert.IsNull(protocols.Protocols);
        }
    }
}
