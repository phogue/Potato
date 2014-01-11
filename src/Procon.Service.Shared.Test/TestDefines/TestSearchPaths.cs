using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;

namespace Procon.Service.Shared.Test.TestDefines {
    [TestFixture]
    public class TestSearchPaths {
        [SetUp]
        public void SetUp() {
            this.Cleanup();
        }

        [TearDown]
        public void Cleanup() {
            var directory = new DirectoryInfo(Path.Combine(Defines.PackagesDirectory.FullName, "Exists.1.0.0"));
            if (directory.Exists == true) directory.Delete(true);

            FileInfo file = new FileInfo(Path.Combine(Defines.BaseDirectory.FullName, "Procon.Core.txt"));
            if (file.Exists == true) file.Delete();
        }

        /// <summary>
        /// Tests a file can be found in the base directory
        /// </summary>
        [Test]
        public void TestFileExistsInBaseDirectory() {
            List<String> paths = Defines.SearchPaths("Procon.Service.Shared.Test.dll", new List<String>() {
                Defines.BaseDirectory.FullName
            });

            Assert.IsNotEmpty(paths);
        }

        /// <summary>
        /// Tests a file can be found in the base directory
        /// </summary>
        [Test]
        public void TestFileDoesNotExist() {
            List<String> paths = Defines.SearchPaths("lulz.dll", new List<String>() {
                Defines.BaseDirectory.FullName
            });

            Assert.IsEmpty(paths);
        }

        /// <summary>
        /// Tests that a file will be found in the packages folder
        /// </summary>
        [Test]
        public void TestFileExistsInPackagesDirectory() {
            DirectoryInfo directory = new DirectoryInfo(Path.Combine(Defines.PackagesDirectory.FullName, "Exists.1.0.0", "lib", "net40"));
            FileInfo file = new FileInfo(Path.Combine(directory.FullName, "Procon.Core.txt"));
            directory.Create();

            File.WriteAllText(file.FullName, "Test Output");

            List<String> paths = Defines.SearchPaths(file.Name, new List<String>() {
                directory.FullName
            });

            Assert.IsNotEmpty(paths);
        }

        /// <summary>
        /// Tests that a file will be found in the packages folder and base directory
        /// </summary>
        [Test]
        public void TestFileExistsInPackagesAndBaseDirectory() {
            DirectoryInfo directory = new DirectoryInfo(Path.Combine(Defines.PackagesDirectory.FullName, "Exists.1.0.0", "lib", "net40"));
            FileInfo file = new FileInfo(Path.Combine(directory.FullName, "Procon.Core.txt"));
            directory.Create();

            File.WriteAllText(file.FullName, "Test Output");

            File.WriteAllText(Path.Combine(Defines.BaseDirectory.FullName, "Procon.Core.txt"), "Test Output");

            List<String> paths = Defines.SearchPaths(file.Name, new List<String>() {
                Defines.BaseDirectory.FullName,
                directory.FullName
            });

            Assert.IsNotEmpty(paths);
            Assert.AreEqual(2, paths.Count);
        }
    }
}
