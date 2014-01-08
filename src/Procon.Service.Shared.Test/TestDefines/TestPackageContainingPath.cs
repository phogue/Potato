using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;

namespace Procon.Service.Shared.Test.TestDefines {
    [TestFixture]
    public class TestPackageContainingPath {

        public readonly String PackagesExists100Path = Path.Combine(Defines.PackagesDirectory, "Exists.1.0.0");

        /// <summary>
        /// Clears out all files in the packages directory and ensures the packages directory is created.
        /// </summary>
        [SetUp]
        public void CleanPackagesDirectory() {
            Directory.Delete(Defines.PackagesDirectory, true);
            Directory.CreateDirectory(Defines.PackagesDirectory);
            Directory.CreateDirectory(PackagesExists100Path);
        }

        /// <summary>
        /// Tests a sub directory in the packages directory will correctly find the packages path
        /// </summary>
        [Test]
        public void TestDirectoryExistsOneDeep() {
            var directory = new DirectoryInfo(Path.Combine(PackagesExists100Path, "SubDirectoryA"));

            directory.Create();

            var path = Defines.PackageContainingPath(directory.FullName);

            Assert.AreEqual(PackagesExists100Path, path.FullName);
        }

        /// <summary>
        /// Tests a two deep sub directory in the packages directory will correctly find the packages path
        /// </summary>
        [Test]
        public void TestDirectoryExistsTwoDeep() {
            var directory = new DirectoryInfo(Path.Combine(PackagesExists100Path, "SubDirectoryA", "SubDirectoryB"));

            directory.Create();

            var path = Defines.PackageContainingPath(directory.FullName);

            Assert.AreEqual(PackagesExists100Path, path.FullName);
        }


        /// <summary>
        /// Tests a file in the package root path will be found
        /// </summary>
        [Test]
        public void TestFileExistsRoot() {
            var file = new FileInfo(Path.Combine(PackagesExists100Path, "A.txt"));

            file.Create().Close();

            var path = Defines.PackageContainingPath(file.FullName);

            Assert.AreEqual(PackagesExists100Path, path.FullName);
        }

        /// <summary>
        /// Tests a file in a sub directory in the packages directory will correctly find the packages path
        /// </summary>
        [Test]
        public void TestFileExistsOneDeep() {
            var file = new FileInfo(Path.Combine(PackagesExists100Path, "SubDirectoryA", "A.txt"));

            file.Directory.Create();
            file.Create().Close();

            var path = Defines.PackageContainingPath(file.FullName);

            Assert.AreEqual(PackagesExists100Path, path.FullName);
        }

        /// <summary>
        /// Tests a file in a two-deep sub directory in the packages directory will correctly find the packages path
        /// </summary>
        [Test]
        public void TestFileExistsTwoDeep() {
            var file = new FileInfo(Path.Combine(PackagesExists100Path, "SubDirectoryA", "SubDirectoryB", "A.txt"));

            file.Directory.Create();
            file.Create().Close();

            var path = Defines.PackageContainingPath(file.FullName);

            Assert.AreEqual(PackagesExists100Path, path.FullName);
        }

        /// <summary>
        /// Tests that a path that does not exist will travel up to find the base directory.
        /// </summary>
        [Test]
        public void TestDoesNotExistStopAtBaseDirectory() {
            var path = Defines.PackageContainingPath("does-not-exist");

            Assert.AreEqual(Defines.BaseDirectory, path.FullName);
        }
    }
}
