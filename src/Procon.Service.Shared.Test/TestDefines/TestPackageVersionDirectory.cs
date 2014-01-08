using System;
using System.IO;
using NUnit.Framework;

namespace Procon.Service.Shared.Test.TestDefines {
    [TestFixture]
    public class TestPackageVersionDirectory {
        /// <summary>
        /// Clears out all files in the packages directory and ensures the packages directory is created.
        /// </summary>
        [SetUp]
        public void CleanPackagesDirectory() {
            Directory.Delete(Defines.PackagesDirectory, true);
            Directory.CreateDirectory(Defines.PackagesDirectory);
        }

        /// <summary>
        /// Tests that a package directory will not be found if it does not exist.
        /// </summary>
        [Test]
        public void PackageDirectoryDoesNotExist() {
            String path = Defines.PackageVersionDirectory(Defines.PackagesDirectory, "does-not-exist");

            Assert.IsNull(path);
        }

        /// <summary>
        /// Tests that a package directory can be found
        /// </summary>
        [Test]
        public void PackageDirectoryExists() {
            String packagePath = Path.Combine(Defines.PackagesDirectory, "Exists.1.0.0");

            Directory.CreateDirectory(packagePath);

            String path = Defines.PackageVersionDirectory(Defines.PackagesDirectory, "Exists");

            Assert.AreEqual(packagePath, path);
        }
    }
}
