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
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Potato.Core.Protocols;
using Potato.Service.Shared;

namespace Potato.Core.Test.Protocols.TestProtocolController {
    [TestFixture]
    public class TestGetProtocolPackages {
        protected DirectoryInfo TestTestGetProtocolPackages = new DirectoryInfo(Path.Combine(Defines.PackagesDirectory.FullName, "TestGetProtocolPackages"));

        /// <summary>
        /// Clears out all files in the packages directory and ensures the packages directory is created.
        /// </summary>
        [SetUp]
        public void CleanPackagesDirectory() {
            this.TestTestGetProtocolPackages.Refresh();

            if (this.TestTestGetProtocolPackages.Exists) {
                this.TestTestGetProtocolPackages.Delete(true);
            }

            this.TestTestGetProtocolPackages.Create();
        }

        [TearDown]
        public void TearDownCleanPackagesDirectory() {
            this.CleanPackagesDirectory();
        }

        /// <summary>
        /// Tests that the package directory will be returned at a single depth
        /// </summary>
        [Test]
        public void TestPackageDirectoryReturnedSingleDepth() {
            DirectoryInfo package = new DirectoryInfo(Path.Combine(this.TestTestGetProtocolPackages.FullName, "Something.Protocols.Something"));

            var dll = new FileInfo(Path.Combine(package.FullName, "Something.Protocols.Something.dll"));
            if (dll.Directory != null) dll.Directory.Create();

            File.WriteAllText(dll.FullName, @"binary");

            var protocols = new ProtocolController() {
                PackagesDirectory = this.TestTestGetProtocolPackages
            };

            var packages = protocols.GetProtocolPackages(new List<FileInfo>() { dll });

            Assert.AreEqual(1, packages.Count);
            Assert.AreEqual(package.FullName, packages.First().FullName);
        }

        /// <summary>
        /// Tests the package directory will be returned if the assembly is in a sub directory of the package
        /// </summary>
        [Test]
        public void TestPackageDirectoryReturnedSecondDepth() {
            DirectoryInfo package = new DirectoryInfo(Path.Combine(this.TestTestGetProtocolPackages.FullName, "Something.Protocols.Something"));

            var dll = new FileInfo(Path.Combine(package.FullName, "SubDirectory", "Something.Protocols.Something.dll"));
            if (dll.Directory != null) dll.Directory.Create();

            File.WriteAllText(dll.FullName, @"binary");

            var protocols = new ProtocolController() {
                PackagesDirectory = this.TestTestGetProtocolPackages
            };

            var packages = protocols.GetProtocolPackages(new List<FileInfo>() { dll });

            Assert.AreEqual(1, packages.Count);
            Assert.AreEqual(package.FullName, packages.First().FullName);
        }

        /// <summary>
        /// Tests that if multiple versions of identical packages exist, we will get the latest version returned.
        /// </summary>
        [Test]
        public void TestLatestPackagePathReturned() {
            DirectoryInfo latest = new DirectoryInfo(Path.Combine(this.TestTestGetProtocolPackages.FullName, "Something.Protocols.Something.2.0.0"));
            DirectoryInfo oldest = new DirectoryInfo(Path.Combine(this.TestTestGetProtocolPackages.FullName, "Something.Protocols.Something.1.0.0"));

            var latestDll = new FileInfo(Path.Combine(latest.FullName, "SubDirectory", "Something.Protocols.Something.dll"));
            if (latestDll.Directory != null) latestDll.Directory.Create();
            File.WriteAllText(latestDll.FullName, @"binary");

            var oldestDll = new FileInfo(Path.Combine(oldest.FullName, "SubDirectory", "Something.Protocols.Something.dll"));
            if (oldestDll.Directory != null) oldestDll.Directory.Create();
            File.WriteAllText(oldestDll.FullName, @"binary");

            var protocols = new ProtocolController() {
                PackagesDirectory = this.TestTestGetProtocolPackages
            };

            var packages = protocols.GetProtocolPackages(new List<FileInfo>() { latestDll, oldestDll });

            Assert.AreEqual(1, packages.Count);
            Assert.AreEqual(latest.FullName, packages.First().FullName);
        }
    }
}
