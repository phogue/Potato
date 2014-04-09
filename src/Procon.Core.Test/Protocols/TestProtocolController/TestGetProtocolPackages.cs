using System.IO;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Procon.Core.Protocols;
using Procon.Service.Shared;

namespace Procon.Core.Test.Protocols.TestProtocolController {
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
