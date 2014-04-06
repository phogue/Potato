﻿using System.IO;
using System.Linq;
using NUnit.Framework;
using Procon.Core.Protocols;
using Procon.Service.Shared;

namespace Procon.Core.Test.Protocols.TestProtocolController {
    [TestFixture]
    public class TestGetProtocolAssemblies {
        /// <summary>
        /// Clears out all files in the packages directory and ensures the packages directory is created.
        /// </summary>
        [SetUp]
        public void CleanPackagesDirectory() {
            Defines.PackagesDirectory.Delete(true);
            Defines.PackagesDirectory.Create();
        }

        /// <summary>
        /// Tests that a file matching the pattern "*.Protocols.*.dll" is found
        /// </summary>
        [Test]
        public void TestAllProtocolAssembliesFilesFoundInPackageRoot() {
            var dll = new FileInfo(Path.Combine(Defines.PackagesDirectory.FullName, "Something.Protocols.Something", "Something.Protocols.Something.dll"));
            if (dll.Directory != null) dll.Directory.Create();

            File.WriteAllText(dll.FullName, @"binary");

            var protocols = new ProtocolController();

            var files = protocols.GetProtocolAssemblies();

            Assert.AreEqual(1, files.Count);
            Assert.AreEqual(dll.FullName, files.First().FullName);
        }

        /// <summary>
        /// Tests that anything not matching the file pattern "*.Protocols.*.dll" is not found
        /// </summary>
        [Test]
        public void TestNonProtocolAssemblyNotDiscovered() {
            var dll = new FileInfo(Path.Combine(Defines.PackagesDirectory.FullName, "Something.Protocols.Something", "Something.Protocols.Something.dll"));
            var json = new FileInfo(Path.Combine(Defines.PackagesDirectory.FullName, "Something.Protocols.Something", "Something.Protocols.Something.json"));
            if (dll.Directory != null) dll.Directory.Create();

            File.WriteAllText(dll.FullName, @"binary");
            File.WriteAllText(json.FullName, @"{ }");

            var protocols = new ProtocolController();

            var files = protocols.GetProtocolAssemblies();

            Assert.AreEqual(1, files.Count);
            Assert.AreEqual(dll.FullName, files.First().FullName);
        }
    }
}