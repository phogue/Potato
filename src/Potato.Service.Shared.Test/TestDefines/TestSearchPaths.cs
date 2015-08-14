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
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;

namespace Potato.Service.Shared.Test.TestDefines {
    [TestFixture]
    public class TestSearchPaths {
        [SetUp]
        public void SetUp() {
            Cleanup();
        }

        [TearDown]
        public void Cleanup() {
            var directory = new DirectoryInfo(Path.Combine(Defines.PackagesDirectory.FullName, "Exists.1.0.0"));
            if (directory.Exists == true) directory.Delete(true);

            var file = new FileInfo(Path.Combine(Defines.BaseDirectory.FullName, "Potato.Core.txt"));
            if (file.Exists == true) file.Delete();
        }

        /// <summary>
        /// Tests a file can be found in the base directory
        /// </summary>
        [Test]
        public void TestFileExistsInBaseDirectory() {
            var paths = Defines.SearchPaths("Potato.Service.Shared.Test.dll", new List<string>() {
                Defines.BaseDirectory.FullName
            });

            Assert.IsNotEmpty(paths);
        }

        /// <summary>
        /// Tests a file can be found in the base directory
        /// </summary>
        [Test]
        public void TestFileDoesNotExist() {
            var paths = Defines.SearchPaths("lulz.dll", new List<string>() {
                Defines.BaseDirectory.FullName
            });

            Assert.IsEmpty(paths);
        }

        /// <summary>
        /// Tests that a file will be found in the packages folder
        /// </summary>
        [Test]
        public void TestFileExistsInPackagesDirectory() {
            var directory = new DirectoryInfo(Path.Combine(Defines.PackagesDirectory.FullName, "Exists.1.0.0", "lib", "net40"));
            var file = new FileInfo(Path.Combine(directory.FullName, "Potato.Core.txt"));
            directory.Create();

            File.WriteAllText(file.FullName, "Test Output");

            var paths = Defines.SearchPaths(file.Name, new List<string>() {
                directory.FullName
            });

            Assert.IsNotEmpty(paths);
        }

        /// <summary>
        /// Tests that a file will be found in the packages folder and base directory
        /// </summary>
        [Test]
        public void TestFileExistsInPackagesAndBaseDirectory() {
            var directory = new DirectoryInfo(Path.Combine(Defines.PackagesDirectory.FullName, "Exists.1.0.0", "lib", "net40"));
            var file = new FileInfo(Path.Combine(directory.FullName, "Potato.Core.txt"));
            directory.Create();

            File.WriteAllText(file.FullName, "Test Output");

            File.WriteAllText(Path.Combine(Defines.BaseDirectory.FullName, "Potato.Core.txt"), "Test Output");

            var paths = Defines.SearchPaths(file.Name, new List<string>() {
                Defines.BaseDirectory.FullName,
                directory.FullName
            });

            Assert.IsNotEmpty(paths);
            Assert.AreEqual(2, paths.Count);
        }
    }
}
