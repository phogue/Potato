using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using NUnit.Framework;
using Procon.Core.Repositories;
using Procon.Core.Repositories.Serialization;
using Procon.Net.Utils;

namespace Procon.Core.Test.Repositories {
    [TestFixture]
    public class TestPackageVersion {

        /// <summary>
        /// Tests that a package version can be deserialized alone with no sub files.
        /// </summary>
        [Test]
        [Ignore]
        public void TestPackageVersionDeserialization() {
            XElement element = XElement.Parse(@"<package_version>
    <version>
        <major>1</major>
        <minor>2</minor>
        <build>3</build>
        <revision>4</revision>
    </version>
    <files>
        <file>
            <name>ThisIsInASubDirectory.txt</name>
            <size>89</size>
            <date>1341275726</date>
            <relative_path>plugins\ThisIsInASubDirectory.txt</relative_path>
            <md5>cbee0eff10c65e6bc4369fbea92df09e</md5>
            <last_modified>2012-07-03T10:05:26+09:30</last_modified>
        </file>
        <file>
            <name>ThisFileIsIdentical.txt</name>
            <size>43</size>
            <date>1341275647</date>
            <relative_path>ThisFileIsIdentical.txt</relative_path>
            <md5>01952484abdda0158c827a8848528e90</md5>
            <last_modified>2012-07-03T10:04:07+09:30</last_modified>
        </file>
    </files>
</package_version>");

            PackageVersion packageVersion = element.FromXElement<PackageVersion>();

            Assert.AreEqual(new Version(1, 2, 3, 4), packageVersion.Version.SystemVersion);

            Assert.AreEqual("ThisIsInASubDirectory.txt", packageVersion.Files[0].Name);
            Assert.AreEqual(89, packageVersion.Files[0].Size);
            Assert.AreEqual(new DateTime(2012, 7, 3, 10, 5, 26), packageVersion.Files[0].LastModified);
            Assert.AreEqual(@"plugins\ThisIsInASubDirectory.txt", packageVersion.Files[0].RelativePath);
            Assert.AreEqual("cbee0eff10c65e6bc4369fbea92df09e", packageVersion.Files[0].Md5);

            Assert.AreEqual("ThisFileIsIdentical.txt", packageVersion.Files[1].Name);
            Assert.AreEqual(43, packageVersion.Files[1].Size);
            Assert.AreEqual(new DateTime(2012, 7, 3, 10, 4, 7), packageVersion.Files[1].LastModified);
            Assert.AreEqual(@"ThisFileIsIdentical.txt", packageVersion.Files[1].RelativePath);
            Assert.AreEqual("01952484abdda0158c827a8848528e90", packageVersion.Files[1].Md5);
        }

        /// <summary>
        /// Test that a stored md5 hash of a file will be picked up when scanning a directory for changes.
        /// </summary>
        [Test]
        public void TestPackageVersionModifiedFilesAtModified() {
            String repositoryModifiedFilesAtValidPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Repositories\ModifiedFilesAt\Valid");

            PackageVersion packageVersion = new PackageVersion() {
                Version = new SerializableVersion() {
                    Build = 1,
                    Minor = 2,
                    Major = 3,
                    Revision = 4
                },
                Files = new List<PackageFile>() {
                    new PackageFile() {
                        Name = "EmptyFile.txt",
                        RelativePath = "EmptyFile.txt",
                        Size = 0,
                        LastModified = DateTime.Now,
                        Md5 = MD5.String("modified data")
                    }
                }
            };

            List<PackageFile> modifiedFiles = packageVersion.ModifiedFilesAt(repositoryModifiedFilesAtValidPath).ToList();

            Assert.AreEqual(1, modifiedFiles.Count);
        }

        /// <summary>
        /// Tests that no modifications are found if the hash matches the file hash.
        /// </summary>
        [Test]
        public void TestPackageVersionModifiedFilesAtNoModifications() {
            String repositoryModifiedFilesAtValidPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Repositories\ModifiedFilesAt\Valid");

            PackageVersion packageVersion = new PackageVersion() {
                Version = new SerializableVersion() {
                    Build = 1,
                    Minor = 2,
                    Major = 3,
                    Revision = 4
                },
                Files = new List<PackageFile>() {
                    new PackageFile() {
                        Name = "EmptyFile.txt",
                        RelativePath = "EmptyFile.txt",
                        Size = 0,
                        LastModified = DateTime.Now,
                        Md5 = "ecaa88f7fa0bf610a5a26cf545dcd3aa"
                    }
                }
            };

            List<PackageFile> modifiedFiles = packageVersion.ModifiedFilesAt(repositoryModifiedFilesAtValidPath).ToList();

            Assert.AreEqual(0, modifiedFiles.Count);
        }
    }
}
