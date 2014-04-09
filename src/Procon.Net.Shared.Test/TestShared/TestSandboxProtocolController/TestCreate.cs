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
using System.IO;
using System.Linq;
using NUnit.Framework;
using Procon.Net.Shared.Sandbox;

namespace Procon.Net.Shared.Test.TestShared.TestSandboxProtocolController {
    [TestFixture]
    public class TestCreate {
        private ProtocolAssemblyMetadata Meta { get; set; }

        [SetUp]
        public void LoadMeta() {
            this.Meta = new ProtocolAssemblyMetadata() {
                Directory = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory),
                Assembly = new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Myrcon.Protocols.Test.dll")),
                Meta = new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Myrcon.Protocols.Test.json")),
                Name = "Myrcon.Protocols.Test"
            };

            this.Meta.Load();
        }

        /// <summary>
        /// Tests that loading an assembly, creating a protocol from a type etc will not fail
        /// if correct information is passed through
        /// </summary>
        [Test]
        public void TestSuccess() {
            var controller = new SandboxProtocolController();

            controller.Create(this.Meta.Assembly.FullName, this.Meta.ProtocolTypes.FirstOrDefault(type => type.Type == "MyrconTestProtocol8"));

            Assert.IsNotNull(controller.SandboxedProtocol);
        }

        /// <summary>
        /// Tests that the return value of Create is true if the sandboxed protocol is loaded correctly.
        /// </summary>
        [Test]
        public void TestSuccessReturnsTrue() {
            var controller = new SandboxProtocolController();

            var result = controller.Create(this.Meta.Assembly.FullName, this.Meta.ProtocolTypes.FirstOrDefault(type => type.Type == "MyrconTestProtocol8"));

            Assert.IsTrue(result);
            Assert.IsNotNull(controller.SandboxedProtocol);
        }

        /// <summary>
        /// Passes in a fake path to load
        /// </summary>
        [Test]
        public void TestFailureWhenAssemblyDoesNotExist() {
            var controller = new SandboxProtocolController();

            controller.Create("Protocol.dll", this.Meta.ProtocolTypes.FirstOrDefault(type => type.Type == "MyrconTestProtocol8"));

            Assert.IsNull(controller.SandboxedProtocol);
        }

        /// <summary>
        /// Tests that the return value of Create is false if the sandboxed protocol is not loaded correctly.
        /// </summary>
        [Test]
        public void TestFailureReturnsFalse() {
            var controller = new SandboxProtocolController();

            var result = controller.Create("Protocol.dll", this.Meta.ProtocolTypes.FirstOrDefault(type => type.Type == "MyrconTestProtocol8"));

            Assert.IsFalse(result);
            Assert.IsNull(controller.SandboxedProtocol);
        }

        /// <summary>
        /// Passes in an unknown provider
        /// </summary>
        [Test]
        public void TestFailureWhenProtocolProviderDoesNotExist() {
            var controller = new SandboxProtocolController();

            controller.Create(this.Meta.Assembly.FullName, new ProtocolType() {
                Provider = "Fake",
                Type = "MyrconTestProtocol8"
            });

            Assert.IsNull(controller.SandboxedProtocol);
        }

        /// <summary>
        /// Passes in an unknown provider
        /// </summary>
        [Test]
        public void TestFailureWhenProtocolTypeDoesNotExist() {
            var controller = new SandboxProtocolController();

            controller.Create(this.Meta.Assembly.FullName, new ProtocolType() {
                Provider = "Myrcon",
                Type = "Fake"
            });

            Assert.IsNull(controller.SandboxedProtocol);
        }
    }
}
