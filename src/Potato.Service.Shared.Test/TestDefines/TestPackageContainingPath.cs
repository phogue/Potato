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
using System;
using System.IO;
using NUnit.Framework;

namespace Potato.Service.Shared.Test.TestDefines {
    [TestFixture]
    public class TestPackageContainingPath {

        public readonly string PackagesExists100Path = Path.Combine(Defines.PackagesDirectory.FullName, "Exists.1.0.0");

        /// <summary>
        /// Clears out all files in the packages directory and ensures the packages directory is created.
        /// </summary>
        [SetUp]
        public void CleanPackagesDirectory() {
            if (Defines.PackagesDirectory.Exists == true) Defines.PackagesDirectory.Delete(true);
            Defines.PackagesDirectory.Create();
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

            if (file.Directory != null) file.Directory.Create();
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

            if (file.Directory != null) file.Directory.Create();
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

            Assert.AreEqual(Defines.BaseDirectory.FullName, path.FullName);
        }
    }
}
