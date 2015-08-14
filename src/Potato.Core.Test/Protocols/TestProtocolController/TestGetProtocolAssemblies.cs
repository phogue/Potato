#region Copyright
// Copyright 2015 Geoff Green.
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
using System.IO;
using System.Linq;
using NUnit.Framework;
using Potato.Core.Protocols;
using Potato.Service.Shared;

namespace Potato.Core.Test.Protocols.TestProtocolController {
    [TestFixture]
    public class TestGetProtocolAssemblies {

        protected DirectoryInfo TestGetProtocolAssembliesDirectory = new DirectoryInfo(Path.Combine(Defines.PackagesDirectory.FullName, "TestGetProtocolAssemblies"));

        /// <summary>
        /// Clears out all files in the packages directory and ensures the packages directory is created.
        /// </summary>
        [SetUp]
        public void CleanPackagesDirectory() {
            TestGetProtocolAssembliesDirectory.Refresh();

            if (TestGetProtocolAssembliesDirectory.Exists) {
                TestGetProtocolAssembliesDirectory.Delete(true);
            }

            TestGetProtocolAssembliesDirectory.Create();
        }

        [TearDown]
        public void TearDownCleanPackagesDirectory() {
            CleanPackagesDirectory();
        }

        /// <summary>
        /// Tests that a file matching the pattern "*.Protocols.*.dll" is found
        /// </summary>
        [Test]
        public void TestAllProtocolAssembliesFilesFoundInPackageRoot() {
            var dll = new FileInfo(Path.Combine(TestGetProtocolAssembliesDirectory.FullName, "Something.Protocols.Something", "Something.Protocols.Something.dll"));
            if (dll.Directory != null) dll.Directory.Create();

            File.WriteAllText(dll.FullName, @"binary");

            var protocols = new ProtocolController() {
                PackagesDirectory = TestGetProtocolAssembliesDirectory
            };

            var files = protocols.GetProtocolAssemblies();

            Assert.AreEqual(1, files.Count);
            Assert.AreEqual(dll.FullName, files.First().FullName);
        }

        /// <summary>
        /// Tests that anything not matching the file pattern "*.Protocols.*.dll" is not found
        /// </summary>
        [Test]
        public void TestNonProtocolAssemblyNotDiscovered() {
            var dll = new FileInfo(Path.Combine(TestGetProtocolAssembliesDirectory.FullName, "Something.Protocols.Something", "Something.Protocols.Something.dll"));
            var json = new FileInfo(Path.Combine(TestGetProtocolAssembliesDirectory.FullName, "Something.Protocols.Something", "Something.Protocols.Something.json"));
            if (dll.Directory != null) dll.Directory.Create();

            File.WriteAllText(dll.FullName, @"binary");
            File.WriteAllText(json.FullName, @"{ }");

            var protocols = new ProtocolController() {
                PackagesDirectory = TestGetProtocolAssembliesDirectory
            };

            var files = protocols.GetProtocolAssemblies();

            Assert.AreEqual(1, files.Count);
            Assert.AreEqual(dll.FullName, files.First().FullName);
        }
    }
}
