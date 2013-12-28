using System.Net;
using NUnit.Framework;
using Procon.Net.Shared.Utils;

namespace Procon.Net.Shared.Test.Utils {
    [TestFixture]
    public class NetworkTest {
        [Test]
        public void TestGetExternalIpAddressPingLocalhost() {
            IPAddress ip = Network.GetExternalIpAddress("localhost");

            Assert.AreEqual("127.0.0.1", ip.ToString());
        }
    }
}
