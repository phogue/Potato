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
using NUnit.Framework;

namespace Potato.Service.Shared.Test.TestDefines {
    [TestFixture]
    public class TestPackageVersionDirectory {
        /// <summary>
        /// Clears out all files in the packages directory and ensures the packages directory is created.
        /// </summary>
        [SetUp]
        public void CleanPackagesDirectory() {
            Defines.PackagesDirectory.Delete(true);
            Defines.PackagesDirectory.Create();
        }

        /// <summary>
        /// Tests that a package directory will not be found if it does not exist.
        /// </summary>
        [Test]
        public void PackageDirectoryDoesNotExist() {
            String path = Defines.PackageVersionDirectory(Defines.PackagesDirectory.FullName, "does-not-exist");

            Assert.IsNull(path);
        }

        /// <summary>
        /// Tests that a package directory can be found
        /// </summary>
        [Test]
        public void PackageDirectoryExists() {
            String packagePath = Path.Combine(Defines.PackagesDirectory.FullName, "Exists.1.0.0");

            Directory.CreateDirectory(packagePath);

            String path = Defines.PackageVersionDirectory(Defines.PackagesDirectory.FullName, "Exists");

            Assert.AreEqual(packagePath, path);
        }

        /// <summary>
        /// Tests that the latest version of a package is found
        /// </summary>
        [Test]
        public void PackageLatestVersionDirectoryExists() {
            String oldest = Path.Combine(Defines.PackagesDirectory.FullName, "Exists.1.0.0");
            String latest = Path.Combine(Defines.PackagesDirectory.FullName, "Exists.1.1.0");

            Directory.CreateDirectory(oldest);
            Directory.CreateDirectory(latest);

            String path = Defines.PackageVersionDirectory(Defines.PackagesDirectory.FullName, "Exists");

            Assert.AreEqual(latest, path);
        }

        /// <summary>
        /// Tests that the latest version of a package is found from a range of packages
        /// </summary>
        [Test]
        public void PackageLatestVersionDirectoryExistsInRange() {
            for (var offset = 0; offset < 20; offset++) {
                Directory.CreateDirectory(Path.Combine(Defines.PackagesDirectory.FullName, String.Format("Exists.1.0.{0}", offset)));
            }

            String latest = Path.Combine(Defines.PackagesDirectory.FullName, "Exists.9.1.0");
            Directory.CreateDirectory(latest);

            String path = Defines.PackageVersionDirectory(Defines.PackagesDirectory.FullName, "Exists");

            Assert.AreEqual(latest, path);
        }

        /// <summary>
        /// Tests that an invalid semantic version is ignored
        /// </summary>
        [Test]
        public void InvalidSemanticVersionIsIgnored() {
            String latest = Path.Combine(Defines.PackagesDirectory.FullName, "Exists.1.0.0");
            String invalid = Path.Combine(Defines.PackagesDirectory.FullName, "Exists.9.j.0");

            Directory.CreateDirectory(latest);
            Directory.CreateDirectory(invalid);

            String path = Defines.PackageVersionDirectory(Defines.PackagesDirectory.FullName, "Exists");

            Assert.AreEqual(latest, path);
        }

        /// <summary>
        /// Tests that an invalid semantic version is ignored
        /// </summary>
        [Test]
        public void InvalidSemanticVersionIsIgnoredInRange() {
            String latest = Path.Combine(Defines.PackagesDirectory.FullName, "Exists.1.0.0");
            Directory.CreateDirectory(latest);

            for (var offset = 0; offset < 20; offset++) {
                Directory.CreateDirectory(Path.Combine(Defines.PackagesDirectory.FullName, String.Format("Exists.2.j.{0}", offset)));
            }

            String path = Defines.PackageVersionDirectory(Defines.PackagesDirectory.FullName, "Exists");

            Assert.AreEqual(latest, path);
        }
    }
}
