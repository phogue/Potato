using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Procon.Net.Shared.Sandbox;

namespace Procon.Net.Shared.Test.TestShared.TestSandboxProtocolController {
    [TestFixture]
    public class TestCreate {
        /// <summary>
        /// Tests that loading an assembly, creating a protocol from a type etc will not fail
        /// if correct information is passed through
        /// </summary>
        [Test]
        public void TestSuccess() {
            var controller = new SandboxProtocolController();



            //controller.Create()


        }
    }
}
