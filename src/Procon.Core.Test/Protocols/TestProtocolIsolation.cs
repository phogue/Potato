using System;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Procon.Core.Protocols;
using Procon.Core.Shared;

namespace Procon.Core.Test.Protocols {
    [TestFixture]
    public class TestProtocolIsolation {
        [SetUp]
        public void Initialize() {
            SharedReferences.Setup();
        }

        /// <summary>
        /// Tests that simply loading a protocol does not load the assembly into the current AppDomain.
        /// </summary>
        [Test]
        public void TestIsolationSuccessOnLoad() {
            var instance = (InstanceController)new InstanceController().Execute();

            var meta = ((ProtocolController)instance.Protocols).Protocols.First(m => m.Name == "Myrcon.Protocols.Test");

            var protocol = meta.ProtocolTypes.First(p => p.Type == "MockProtocol");

            // Now readd the same connection we just added.
            ICommandResult result = instance.Tunnel(CommandBuilder.InstanceAddConnection(protocol.Provider, protocol.Type, "1.1.1.1", 27516, "password", "").SetOrigin(CommandOrigin.Local));

            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Success, result.CommandResultType);

            // Now make sure our current appdomain is clean of the test plugin
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies()) {
                Assert.IsFalse(assembly.FullName.Contains("Myrcon.Protocols.Test"));
            }

            instance.Dispose();
        }
    }
}