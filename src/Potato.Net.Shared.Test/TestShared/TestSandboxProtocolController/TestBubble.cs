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
using Myrcon.Protocols.Test;
using NUnit.Framework;
using Potato.Net.Shared.Sandbox;

namespace Potato.Net.Shared.Test.TestShared.TestSandboxProtocolController {
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
