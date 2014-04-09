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
using System.IO;
using System.Linq;
using NUnit.Framework;
using Procon.Core.Protocols;
using Procon.Service.Shared;

namespace Procon.Core.Test.Protocols.TestProtocolController {
    [TestFixture]
    public class TestLoadProtocolsMetadata {

        protected DirectoryInfo TestLoadProtocolsMetadataDirectory = new DirectoryInfo(Path.Combine(Defines.PackagesDirectory.FullName, "TestLoadProtocolsMetadata"));

        /// <summary>TestLoadProtocolsMetadataDirectory
        /// Clears out all files in the packages directory and ensures the packages directory is created.
        /// </summary>
        [SetUp]
        public void CleanPackagesDirectory() {
            this.TestLoadProtocolsMetadataDirectory.Refresh();

            if (this.TestLoadProtocolsMetadataDirectory.Exists) {
                this.TestLoadProtocolsMetadataDirectory.Delete(true);
            }

            this.TestLoadProtocolsMetadataDirectory.Create();
        }

        [TearDown]
        public void TearDownCleanPackagesDirectory() {
            this.CleanPackagesDirectory();
        }

        /// <summary>
        /// Tests that a protocol is loaded from a protocol package
        /// </summary>
        [Test]
        public void TestLoadedWithSingleVersionOfPackage() {
            DirectoryInfo package = new DirectoryInfo(Path.Combine(this.TestLoadProtocolsMetadataDirectory.FullName, "Something.Protocols.Something"));

            var dll = new FileInfo(Path.Combine(package.FullName, "lib", "Something.Protocols.Something.dll"));
            if (dll.Directory != null) dll.Directory.Create();
            File.WriteAllText(dll.FullName, @"binary");

            var json = new FileInfo(Path.Combine(package.FullName, "Content", "Something.Protocols.Something.json"));
            if (json.Directory != null) json.Directory.Create();
            File.WriteAllText(json.FullName, @"{ }");

            var protocols = new ProtocolController() {
                PackagesDirectory = this.TestLoadProtocolsMetadataDirectory
            };

            protocols.LoadProtocolsMetadata();

            Assert.AreEqual(1, protocols.Protocols.Count);
        }

        /// <summary>
        /// Tests that a protocol is loaded from a protocol metadata
        /// </summary>
        /// <remarks>We only test the process is succesful, loading protocol metadata is tested elsewhere.</remarks>
        [Test]
        public void TestProtocolVariablesLoaded() {
            DirectoryInfo package = new DirectoryInfo(Path.Combine(this.TestLoadProtocolsMetadataDirectory.FullName, "Something.Protocols.Something"));

            var dll = new FileInfo(Path.Combine(package.FullName, "lib", "Something.Protocols.Something.dll"));
            if (dll.Directory != null) dll.Directory.Create();
            File.WriteAllText(dll.FullName, @"binary");

            var json = new FileInfo(Path.Combine(package.FullName, "Content", "Something.Protocols.Something.json"));
            if (json.Directory != null) json.Directory.Create();
            File.WriteAllText(json.FullName, @"{ ""ProtocolTypes"": [ { ""Provider"": ""Myrcon"",""Name"": ""Battlefield 4"",""Type"": ""DiceBattlefield4"" } ] }");

            var protocols = new ProtocolController() {
                PackagesDirectory = this.TestLoadProtocolsMetadataDirectory
            };

            protocols.LoadProtocolsMetadata();

            Assert.AreEqual("DiceBattlefield4", protocols.Protocols.First().ProtocolTypes.First().Type);
        }

        /// <summary>
        /// Tests that a single protocol will be loaded when multiple versions of the package are available.
        /// </summary>
        [Test]
        public void TestSingleProtocolLoadedWithMultipleVersionsOfPackage() {
            DirectoryInfo newest = new DirectoryInfo(Path.Combine(this.TestLoadProtocolsMetadataDirectory.FullName, "Something.Protocols.Something.2.0.0"));
            DirectoryInfo oldest = new DirectoryInfo(Path.Combine(this.TestLoadProtocolsMetadataDirectory.FullName, "Something.Protocols.Something.1.0.0"));

            var newestdll = new FileInfo(Path.Combine(newest.FullName, "lib", "Something.Protocols.Something.dll"));
            if (newestdll.Directory != null) newestdll.Directory.Create();
            File.WriteAllText(newestdll.FullName, @"binary");

            var newestjson = new FileInfo(Path.Combine(newest.FullName, "Content", "Something.Protocols.Something.json"));
            if (newestjson.Directory != null) newestjson.Directory.Create();
            File.WriteAllText(newestjson.FullName, @"{ }");

            var oldestdll = new FileInfo(Path.Combine(oldest.FullName, "lib", "Something.Protocols.Something.dll"));
            if (oldestdll.Directory != null) oldestdll.Directory.Create();
            File.WriteAllText(oldestdll.FullName, @"binary");

            var oldestjson = new FileInfo(Path.Combine(oldest.FullName, "Content", "Something.Protocols.Something.json"));
            if (oldestjson.Directory != null) oldestjson.Directory.Create();
            File.WriteAllText(oldestjson.FullName, @"{ }");

            var protocols = new ProtocolController() {
                PackagesDirectory = this.TestLoadProtocolsMetadataDirectory
            };

            protocols.LoadProtocolsMetadata();

            Assert.AreEqual(1, protocols.Protocols.Count);
        }

        /// <summary>
        /// Tests the latest version of the packag is loaded.
        /// </summary>
        [Test]
        public void TestLatestLoadedLoadedWithMultipleVersionsOfPackage() {
            DirectoryInfo newest = new DirectoryInfo(Path.Combine(this.TestLoadProtocolsMetadataDirectory.FullName, "Something.Protocols.Something.2.0.0"));
            DirectoryInfo oldest = new DirectoryInfo(Path.Combine(this.TestLoadProtocolsMetadataDirectory.FullName, "Something.Protocols.Something.1.0.0"));

            var newestdll = new FileInfo(Path.Combine(newest.FullName, "lib", "Something.Protocols.Something.dll"));
            if (newestdll.Directory != null) newestdll.Directory.Create();
            File.WriteAllText(newestdll.FullName, @"binary");

            var newestjson = new FileInfo(Path.Combine(newest.FullName, "Content", "Something.Protocols.Something.json"));
            if (newestjson.Directory != null) newestjson.Directory.Create();
            File.WriteAllText(newestjson.FullName, @"{ }");

            var oldestdll = new FileInfo(Path.Combine(oldest.FullName, "lib", "Something.Protocols.Something.dll"));
            if (oldestdll.Directory != null) oldestdll.Directory.Create();
            File.WriteAllText(oldestdll.FullName, @"binary");

            var oldestjson = new FileInfo(Path.Combine(oldest.FullName, "Content", "Something.Protocols.Something.json"));
            if (oldestjson.Directory != null) oldestjson.Directory.Create();
            File.WriteAllText(oldestjson.FullName, @"{ }");

            var protocols = new ProtocolController() {
                PackagesDirectory = this.TestLoadProtocolsMetadataDirectory
            };

            protocols.LoadProtocolsMetadata();

            Assert.AreEqual(newest.FullName, protocols.Protocols.First().Directory.FullName);
        }
    }
}
