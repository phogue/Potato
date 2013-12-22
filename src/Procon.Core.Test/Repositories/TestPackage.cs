#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using NUnit.Framework;
using Procon.Core.Repositories;
using Procon.Core.Repositories.Serialization;
using Procon.Net.Shared.Utils;
using Procon.Net.Utils;

#endregion

namespace Procon.Core.Test.Repositories {
    [TestFixture]
    public class TestPackage {
        [Test]
        public void TestPackageCopying() {
            var original = new Package();

            var package = new Package() {
                Name = "DownloadCacheTest",
                Uid = "DownloadCacheTest",
                Versions = new List<PackageVersion>() {
                    new PackageVersion() {
                        Version = new SerializableVersion() {
                            Major = 1,
                            Minor = 2,
                            Build = 3,
                            Revision = 4
                        },
                        Files = new List<PackageFile>() {
                            new PackageFile() {
                                Name = "ThisIsInASubDirectory.txt",
                                Size = 89,
                                LastModified = new DateTime(2012, 7, 3, 10, 5, 26),
                                RelativePath = @"plugins\ThisIsInASubDirectory.txt",
                                Md5 = "cbee0eff10c65e6bc4369fbea92df09e"
                            },
                            new PackageFile() {
                                Name = "ThisFileIsIdentical.txt",
                                Size = 43,
                                LastModified = new DateTime(2012, 7, 3, 10, 4, 7),
                                RelativePath = @"ThisFileIsIdentical.txt",
                                Md5 = "01952484abdda0158c827a8848528e90"
                            }
                        }
                    }
                }
            };

            Package originalCopiedTo = original.Copy(package);

            Assert.IsTrue(ReferenceEquals(original, originalCopiedTo));

            Assert.AreEqual("DownloadCacheTest", original.Uid);
            Assert.AreEqual("DownloadCacheTest", original.Name);

            Assert.AreEqual(new Version(1, 2, 3, 4), original.Versions[0].Version.SystemVersion);

            Assert.AreEqual("ThisIsInASubDirectory.txt", original.Versions[0].Files[0].Name);
            Assert.AreEqual(89, original.Versions[0].Files[0].Size);
            Assert.AreEqual(new DateTime(2012, 7, 3, 10, 5, 26), original.Versions[0].Files[0].LastModified);
            Assert.AreEqual(@"plugins\ThisIsInASubDirectory.txt", original.Versions[0].Files[0].RelativePath);
            Assert.AreEqual("cbee0eff10c65e6bc4369fbea92df09e", original.Versions[0].Files[0].Md5);

            Assert.AreEqual("ThisFileIsIdentical.txt", original.Versions[0].Files[1].Name);
            Assert.AreEqual(43, original.Versions[0].Files[1].Size);
            Assert.AreEqual(new DateTime(2012, 7, 3, 10, 4, 7), original.Versions[0].Files[1].LastModified);
            Assert.AreEqual(@"ThisFileIsIdentical.txt", original.Versions[0].Files[1].RelativePath);
            Assert.AreEqual("01952484abdda0158c827a8848528e90", original.Versions[0].Files[1].Md5);
        }

        /// <summary>
        ///     Tests that we can deserialize from a static copy of the output of the repository.
        /// </summary>
        [Test, Ignore]
        
        public void TestPackageDeserialization() {
            XElement element = XElement.Parse(@"<package>
    <uid>DownloadCacheTest</uid>
    <name>DownloadCacheTest</name>
    <package_versions>
        <package_version>
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
        </package_version>
    </package_versions>
</package>");

            var package = element.FromXElement<Package>();

            Assert.AreEqual("DownloadCacheTest", package.Uid);
            Assert.AreEqual("DownloadCacheTest", package.Name);

            Assert.AreEqual(new Version(1, 2, 3, 4), package.Versions[0].Version.SystemVersion);

            Assert.AreEqual("ThisIsInASubDirectory.txt", package.Versions[0].Files[0].Name);
            Assert.AreEqual(89, package.Versions[0].Files[0].Size);
            Assert.AreEqual(new DateTime(2012, 7, 3, 10, 5, 26), package.Versions[0].Files[0].LastModified);
            Assert.AreEqual(@"plugins\ThisIsInASubDirectory.txt", package.Versions[0].Files[0].RelativePath);
            Assert.AreEqual("cbee0eff10c65e6bc4369fbea92df09e", package.Versions[0].Files[0].Md5);

            Assert.AreEqual("ThisFileIsIdentical.txt", package.Versions[0].Files[1].Name);
            Assert.AreEqual(43, package.Versions[0].Files[1].Size);
            Assert.AreEqual(new DateTime(2012, 7, 3, 10, 4, 7), package.Versions[0].Files[1].LastModified);
            Assert.AreEqual(@"ThisFileIsIdentical.txt", package.Versions[0].Files[1].RelativePath);
            Assert.AreEqual("01952484abdda0158c827a8848528e90", package.Versions[0].Files[1].Md5);
        }

        /// <summary>
        ///     Tests that the latest version of a package is populated.
        /// </summary>
        [Test]
        public void TestPackageDeserializationWithLatestVersion() {
            XElement element = XElement.Parse(@"<package>
    <uid>DownloadCacheTest</uid>
    <name>DownloadCacheTest</name>
    <package_versions>
        <package_version>
            <version>
                <major>1</major>
                <minor>2</minor>
                <build>3</build>
                <revision>4</revision>
            </version>
            <files />
        </package_version>
        <package_version>
            <version>
                <major>0</major>
                <minor>0</minor>
                <build>0</build>
                <revision>1</revision>
            </version>
            <files />
        </package_version>
    </package_versions>
</package>");

            var package = element.FromXElement<Package>();

            Assert.IsNotNull(package.LatestVersion);
            Assert.AreEqual(new Version(1, 2, 3, 4), package.LatestVersion.Version.SystemVersion);
        }

        /// <summary>
        ///     Tests that we can serialize an object to a xml copy of what we see from the repository.
        /// </summary>
        [Test]
        public void TestPackageSerialization() {
            var package = new Package() {
                Name = "DownloadCacheTest",
                Uid = "DownloadCacheTest",
                Versions = new List<PackageVersion>() {
                    new PackageVersion() {
                        Version = new SerializableVersion() {
                            Major = 1,
                            Minor = 2,
                            Build = 3,
                            Revision = 4
                        },
                        Files = new List<PackageFile>() {
                            new PackageFile() {
                                Name = "ThisIsInASubDirectory.txt",
                                Size = 89,
                                LastModified = new DateTime(2012, 7, 3, 10, 5, 26),
                                RelativePath = @"plugins\ThisIsInASubDirectory.txt",
                                Md5 = "cbee0eff10c65e6bc4369fbea92df09e"
                            },
                            new PackageFile() {
                                Name = "ThisFileIsIdentical.txt",
                                Size = 43,
                                LastModified = new DateTime(2012, 7, 3, 10, 4, 7),
                                RelativePath = @"ThisFileIsIdentical.txt",
                                Md5 = "01952484abdda0158c827a8848528e90"
                            }
                        }
                    }
                }
            };

            XElement element = package.ToXElement();

            Assert.AreEqual("DownloadCacheTest", element.Element("uid").Value);
            Assert.AreEqual("DownloadCacheTest", element.Element("name").Value);

            Assert.AreEqual("1", element.Descendants("version").First().Element("major").Value);
            Assert.AreEqual("2", element.Descendants("version").First().Element("minor").Value);
            Assert.AreEqual("3", element.Descendants("version").First().Element("build").Value);
            Assert.AreEqual("4", element.Descendants("version").First().Element("revision").Value);

            Assert.AreEqual("ThisIsInASubDirectory.txt", element.Descendants("file").First().Element("name").Value);
            Assert.AreEqual("89", element.Descendants("file").First().Element("size").Value);
            Assert.AreEqual("2012-07-03T10:05:26", element.Descendants("file").First().Element("last_modified").Value);
            Assert.AreEqual(@"plugins\ThisIsInASubDirectory.txt", element.Descendants("file").First().Element("relative_path").Value);
            Assert.AreEqual("cbee0eff10c65e6bc4369fbea92df09e", element.Descendants("file").First().Element("md5").Value);

            Assert.AreEqual("ThisFileIsIdentical.txt", element.Descendants("file").Last().Element("name").Value);
            Assert.AreEqual("43", element.Descendants("file").Last().Element("size").Value);
            Assert.AreEqual("2012-07-03T10:04:07", element.Descendants("file").Last().Element("last_modified").Value);
            Assert.AreEqual(@"ThisFileIsIdentical.txt", element.Descendants("file").Last().Element("relative_path").Value);
            Assert.AreEqual("01952484abdda0158c827a8848528e90", element.Descendants("file").Last().Element("md5").Value);
        }
    }
}