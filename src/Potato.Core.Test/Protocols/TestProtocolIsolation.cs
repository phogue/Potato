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
using System;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Potato.Core.Protocols;
using Potato.Core.Shared;

namespace Potato.Core.Test.Protocols {
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
            var instance = (PotatoController)new PotatoController().Execute();

            var meta = ((ProtocolController)instance.Protocols).Protocols.First(m => m.Name == "Myrcon.Protocols.Test");

            var protocol = meta.ProtocolTypes.First(p => p.Type == "MockProtocol");

            // Now readd the same connection we just added.
            ICommandResult result = instance.Tunnel(CommandBuilder.PotatoAddConnection(protocol.Provider, protocol.Type, "1.1.1.1", 27516, "password", "").SetOrigin(CommandOrigin.Local));

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