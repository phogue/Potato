using Myrcon.Protocols.Test;
using NUnit.Framework;
using Procon.Net.Shared.Sandbox;

namespace Procon.Net.Shared.Test.TestShared.TestSandboxProtocolController {
    [TestFixture]
    public class TestBubble {

        protected SandboxProtocolController Controller { get; set; }

        [SetUp]
        public void LoadMeta() {
            this.Controller = new SandboxProtocolController() {
                SandboxedProtocol = new MockIntegrationTestProtocol()
            };

            this.Controller.AssignEvents();
        }

        /// <summary>
        /// Tests that protocol events are passed through, provided the bubble attribute is set
        /// and has a delegate for the protocol event.
        /// </summary>
        [Test]
        public void TestSuccessProtocolEventBubbledThrough() {
            bool called = false;

            this.Controller.Bubble = new SandboxProtocolCallbackProxy() {
                ProtocolEvent = (args) => { called = true; }
            };

            ((MockIntegrationTestProtocol)this.Controller.SandboxedProtocol).MockProtocolEvent(null);

            Assert.IsTrue(called);
        }

        /// <summary>
        /// Tests that client events are passed through, provided the bubble attribute is set
        /// and has a delegate for the client event.
        /// </summary>
        [Test]
        public void TestSuccessClientEventBubbledThrough() {
            bool called = false;

            this.Controller.Bubble = new SandboxProtocolCallbackProxy() {
                ClientEvent = (args) => { called = true; }
            };

            ((MockIntegrationTestProtocol)this.Controller.SandboxedProtocol).MockClientEvent(null);

            Assert.IsTrue(called);
        }
    }
}
