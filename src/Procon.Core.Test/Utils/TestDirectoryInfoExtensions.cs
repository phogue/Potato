#region

using System;
using System.IO;
using System.Linq;
using Ionic.Zip;
using NUnit.Framework;
using Procon.Core.Utils;

#endregion

namespace Procon.Core.Test.Utils {
    [TestFixture]
    public class TestDirectoryInfoExtensions {
        /// <summary>
        ///     Tests that files and sub directories of a directory will be deleted.
        /// </summary>
        [Test]
        public void TestDirectoryInfoExtensionsClean() {
            String cleanPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Utils\DirectoryInfoExtensions\Clean");
            var fileA = new FileInfo(Path.Combine(cleanPath, @"a.txt"));
            var fileB = new FileInfo(Path.Combine(cleanPath, @"sub\b.txt"));

            // Make sure the directories exist
            fileA.Directory.Create();
            fileB.Directory.Create();

            // Create two files.
            File.WriteAllText(fileA.FullName, "Output content");
            File.WriteAllText(fileB.FullName, "Output content");

            // Ensure the files initially exist.
            Assert.IsTrue(fileA.Exists);
            Assert.IsTrue(fileB.Exists);
            Assert.IsTrue(fileA.Directory.Exists);
            Assert.IsTrue(fileB.Directory.Exists);

            new DirectoryInfo(cleanPath).Clean();

            // Now make sure the files have been removed.
            Assert.IsFalse(new FileInfo(fileA.FullName).Exists);
            Assert.IsFalse(new FileInfo(fileB.FullName).Exists);
            Assert.IsFalse(fileB.Directory.Exists);

            // The root directory should still exist.
            Assert.IsTrue(fileA.Directory.Exists);
        }

        /// <summary>
        ///     Checks that a directory can be zipped up. Validates the files are in the resulting zip file in memory.
        /// </summary>
        [Test]
        public void TestDirectoryInfoExtensionsZip() {
            String cleanPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Utils\DirectoryInfoExtensions\Zip");
            var fileA = new FileInfo(Path.Combine(cleanPath, @"a.txt"));
            var fileB = new FileInfo(Path.Combine(cleanPath, @"sub\b.txt"));

            // Make sure the directories exist
            fileA.Directory.Create();
            fileB.Directory.Create();

            // Create two files.
            File.WriteAllText(fileA.FullName, "Output content");
            File.WriteAllText(fileB.FullName, "Output content");

            MemoryStream stream = new DirectoryInfo(cleanPath).Zip();

            ZipFile zip = ZipFile.Read(stream);

            Assert.IsTrue(zip.Select(entry => entry.FileName == "a.txt").Any());
            Assert.IsTrue(zip.Select(entry => entry.FileName == "b.txt").Any());
        }
    }
}