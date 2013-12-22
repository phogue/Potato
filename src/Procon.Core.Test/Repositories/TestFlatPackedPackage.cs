#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Xml.Linq;
using NUnit.Framework;
using Procon.Core.Repositories;
using Procon.Core.Repositories.Serialization;
using Procon.Net.Utils;

#endregion

namespace Procon.Core.Test.Repositories {
    [TestFixture]
    public class TestFlatPackedPackage {
        protected static String FlatPackedPackageCoveredPackageValidInstalledPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Repositories\FlatPackedPackage\CoveredPackage\Valid\Installed");
        protected static String FlatPackedPackageCoveredPackageValidUpdatesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Repositories\FlatPackedPackage\CoveredPackage\Valid\Updates");
        // Note this is only here so we can build MD5 hashes. The files should 
        // instead be located in a repo install on another server with the md5
        // hashes supplied to us via a query.
        protected static String FlatPackedPackageCoveredPackageValidRepoPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Repositories\FlatPackedPackage\CoveredPackage\Valid\Repo");

        protected static String FlatPackedPackageSaveValidUpdatesTemporaryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Repositories\FlatPackedPackage\Save\Valid\Updates\Temporary");
        protected static String FlatPackedPackageSaveValidUpdatesPackagesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Repositories\FlatPackedPackage\Save\Valid\Updates\Packages");

        protected static String FlatPackedPackageMigrateTemporaryFilesValidTemporaryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Repositories\FlatPackedPackage\MigrateTemporaryFiles\Temporary");
        protected static String FlatPackedPackageMigrateTemporaryFilesValidUpdatesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Repositories\FlatPackedPackage\MigrateTemporaryFiles\Updates");

        /// <summary>
        ///     Sets up a package with files installed, an update pending install and new updates in the repository.
        /// </summary>
        /// <returns></returns>
        protected FlatPackedPackage SetupGetModifiedFilesFlatPackage(bool setInstalledVersion = true, bool setUpdatedVersion = true, bool setAvailableVersion = true) {
            var package = new FlatPackedPackage() {
                UpdatesPath = FlatPackedPackageCoveredPackageValidUpdatesPath,
                InstallPath = FlatPackedPackageCoveredPackageValidInstalledPath,
                Name = "DownloadCacheTest",
                Uid = "DownloadCacheTest",
                Versions = new List<PackageVersion>() {
                    new PackageVersion() {
                        Version = new SerializableVersion() {
                            Major = 9,
                            Minor = 9,
                            Build = 9,
                            Revision = 9
                        },
                        Files = new List<PackageFile>() {
                            new PackageFile() {
                                Name = "New.txt",
                                Size = 89,
                                LastModified = new DateTime(2012, 7, 3, 10, 5, 26),
                                RelativePath = @"New.txt",
                                Md5 = MD5.File(Path.Combine(FlatPackedPackageCoveredPackageValidRepoPath, "New.txt"))
                            },
                            new PackageFile() {
                                Name = "File.txt",
                                Size = 89,
                                LastModified = new DateTime(2012, 7, 3, 10, 5, 26),
                                RelativePath = @"File.txt",
                                Md5 = MD5.File(Path.Combine(FlatPackedPackageCoveredPackageValidRepoPath, "File.txt"))
                            },
                            new PackageFile() {
                                Name = "Identical.txt",
                                Size = 43,
                                LastModified = new DateTime(2012, 7, 3, 10, 4, 7),
                                RelativePath = @"Identical.txt",
                                Md5 = MD5.File(Path.Combine(FlatPackedPackageCoveredPackageValidRepoPath, "Identical.txt"))
                            }
                        }
                    },
                    new PackageVersion() {
                        Version = new SerializableVersion() {
                            Major = 1,
                            Minor = 2,
                            Build = 3,
                            Revision = 4
                        },
                        Files = new List<PackageFile>() {
                            new PackageFile() {
                                Name = "File.txt",
                                Size = 89,
                                LastModified = new DateTime(2012, 7, 3, 10, 5, 26),
                                RelativePath = @"File.txt",
                                Md5 = MD5.File(Path.Combine(FlatPackedPackageCoveredPackageValidUpdatesPath, "File.txt"))
                            },
                            new PackageFile() {
                                Name = "Identical.txt",
                                Size = 43,
                                LastModified = new DateTime(2012, 7, 3, 10, 4, 7),
                                RelativePath = @"Identical.txt",
                                Md5 = MD5.File(Path.Combine(FlatPackedPackageCoveredPackageValidUpdatesPath, "Identical.txt"))
                            }
                        }
                    },
                    new PackageVersion() {
                        Version = new SerializableVersion() {
                            Major = 0,
                            Minor = 0,
                            Build = 0,
                            Revision = 1
                        },
                        Files = new List<PackageFile>() {
                            new PackageFile() {
                                Name = "File.txt",
                                Size = 89,
                                LastModified = new DateTime(2012, 7, 3, 10, 5, 26),
                                RelativePath = @"File.txt",
                                Md5 = MD5.File(Path.Combine(FlatPackedPackageCoveredPackageValidInstalledPath, "File.txt"))
                            },
                            new PackageFile() {
                                Name = "Identical.txt",
                                Size = 43,
                                LastModified = new DateTime(2012, 7, 3, 10, 4, 7),
                                RelativePath = @"Identical.txt",
                                Md5 = MD5.File(Path.Combine(FlatPackedPackageCoveredPackageValidInstalledPath, "Identical.txt"))
                            }
                        }
                    }
                }
            };

            if (setInstalledVersion == true)
                package.SetInstalledVersion(package.Versions.First(p => p.Version.SystemVersion == new Version(0, 0, 0, 1)));
            if (setUpdatedVersion == true)
                package.SetUpdatedVersion(package.Versions.First(p => p.Version.SystemVersion == new Version(1, 2, 3, 4)));
            if (setAvailableVersion == true)
                package.SetAvailableVersion(package.Versions.First(p => p.Version.SystemVersion == new Version(9, 9, 9, 9)));

            return package;
        }

        /// <summary>
        ///     Tests that the modified directory is not flagged as a valid install
        ///     when compared against the repo version, which has a modified file
        ///     and a new file that won't exit in the installed directory.
        /// </summary>
        [Test]
        public void TestCoveredPackageIsValidInstallFail() {
            FlatPackedPackage package = SetupGetModifiedFilesFlatPackage();

            Assert.IsFalse(FlatPackedPackage.IsValidInstall(FlatPackedPackageCoveredPackageValidUpdatesPath, package.AvailableVersion.Files));
        }

        /// <summary>
        ///     Tests that a install matches the packages files
        /// </summary>
        [Test]
        public void TestCoveredPackageIsValidInstallSuccess() {
            FlatPackedPackage package = SetupGetModifiedFilesFlatPackage();

            Assert.IsTrue(FlatPackedPackage.IsValidInstall(FlatPackedPackageCoveredPackageValidUpdatesPath, package.InstalledVersion.Files));
        }

        /// <summary>
        ///     Tests that the check for modified file listing is successful for modified or new files.
        /// </summary>
        [Test]
        public void TestCoveredPackageModifiedFiles() {
            FlatPackedPackage package = SetupGetModifiedFilesFlatPackage();

            List<PackageFile> files = package.GetModifiedFiles();

            Assert.AreEqual(2, files.Count);
            Assert.IsNotNull(files.Find(f => f.Name == "File.txt"));
            Assert.IsNotNull(files.Find(f => f.Name == "New.txt"));
            Assert.IsNull(files.Find(f => f.Name == "Identical.txt"));
        }

        /// <summary>
        ///     Tests the output of the modified relative paths helper method. Output should be
        ///     identical to the modified files output, but just return the relative paths of each
        ///     file, not the file object itself.
        /// </summary>
        [Test]
        public void TestCoveredPackageModifiedRelativePaths() {
            FlatPackedPackage package = SetupGetModifiedFilesFlatPackage();

            List<String> relativePaths = package.GetModifiedRelativePaths();

            Assert.AreEqual(2, relativePaths.Count);
            Assert.IsTrue(relativePaths.Contains("File.txt"));
            Assert.IsTrue(relativePaths.Contains("New.txt"));
            Assert.IsFalse(relativePaths.Contains("Identical.txt"));
        }

        /// <summary>
        ///     Tests that if no package is installed the helper will return all the files
        ///     in the latest version (new install)
        /// </summary>
        [Test]
        public void TestCoveredPackageRelevantPackageFilesNotInstalled() {
            FlatPackedPackage package = SetupGetModifiedFilesFlatPackage(false, false);

            List<PackageFile> files = package.RelevantPackageFiles();

            Assert.AreEqual(3, files.Count);
            Assert.IsNotNull(files.Find(f => f.Name == "File.txt"));
            Assert.IsNotNull(files.Find(f => f.Name == "New.txt"));
            Assert.IsNotNull(files.Find(f => f.Name == "Identical.txt"));
        }

        /// <summary>
        ///     Tests a simple helper method to fetch all the files that are required to bring
        ///     the latest installed version to date with the version in the repo.
        /// </summary>
        [Test]
        public void TestCoveredPackageRelevantPackageFilesWithUpdateAvailable() {
            FlatPackedPackage package = SetupGetModifiedFilesFlatPackage();

            List<PackageFile> files = package.RelevantPackageFiles();

            Assert.AreEqual(2, files.Count);
            Assert.IsNotNull(files.Find(f => f.Name == "File.txt"));
            Assert.IsNotNull(files.Find(f => f.Name == "New.txt"));
            Assert.IsNull(files.Find(f => f.Name == "Identical.txt"));
        }

        /// <summary>
        ///     Tests that the helper method returns no required files for download from
        ///     the repo when no file is available, therefore we already have all of the
        ///     updated files.
        /// </summary>
        [Test]
        public void TestCoveredPackageRelevantPackageFilesWithoutUpdateAvailable() {
            FlatPackedPackage package = SetupGetModifiedFilesFlatPackage(true, true, false);

            List<PackageFile> files = package.RelevantPackageFiles();

            Assert.AreEqual(0, files.Count);
            Assert.IsNull(files.Find(f => f.Name == "File.txt"));
            Assert.IsNull(files.Find(f => f.Name == "New.txt"));
            Assert.IsNull(files.Find(f => f.Name == "Identical.txt"));
        }

        /// <summary>
        ///     Tests copying for the flat packed package (overrides the Copy method)
        /// </summary>
        [Test]
        public void TestFlatPackedPackageCopying() {
            var original = new FlatPackedPackage();

            var package = new FlatPackedPackage() {
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
                },
                State = PackageState.UpdateAvailable
            };

            var originalCopiedTo = (FlatPackedPackage) original.Copy(package);

            Assert.IsTrue(ReferenceEquals(original, originalCopiedTo));

            Assert.AreEqual("DownloadCacheTest", original.Uid);
            Assert.AreEqual("DownloadCacheTest", original.Name);
            Assert.AreEqual(PackageState.UpdateAvailable, original.State);

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
        ///     Tests that a package can be downloaded and installed fresh. No existing version of the package exist.
        ///     @todo We shouldn't piggy back off an existing package and instead publish one ourselves so we know it is there for the test.
        /// </summary>
        [Test, Ignore]
        
        public void TestFlatPackedPackageInstall() {
            var requestWait = new AutoResetEvent(false);

            XElement element = XElement.Parse(@"<package>
    <uid>DownloadCacheTest</uid>
    <name>DownloadCacheTest</name>
    <package_versions>
        <package_version>
            <version>
                <major>0</major>
                <minor>0</minor>
                <build>0</build>
                <revision>1</revision>
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

            var package = element.FromXElement<FlatPackedPackage>();
            package.InstallPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Repositories\FlatPackedPackage\InstallOrUpdate\Install\Installed");
            package.UpdatesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Repositories\FlatPackedPackage\InstallOrUpdate\Install\Updates");
            package.TemporaryUpdatesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Repositories\FlatPackedPackage\InstallOrUpdate\Install\Updates\Temporary");
            package.PackagesUpdatesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Repositories\FlatPackedPackage\InstallOrUpdate\Install\Updates\Packages");

            package.Repository = new Repository() {
                Url = TestRepository.TestRepositoryUrl,
                UrlSlug = TestRepository.TestRepositoryUrl.UrlSlug()
            };

            package.SetAvailableVersion(package.Versions.First());

            package.StateChanged += (sender, args) => {
                if (package.State == PackageState.UpdateInstalled) {
                    requestWait.Set();
                }
            };

            package.InstallOrUpdate();

            Assert.IsTrue(requestWait.WaitOne(60000));

            // Test that the downloaded zip file was removed.
            Assert.IsFalse(Directory.Exists(package.TemporaryUpdatesPath));

            // Test that the package xml file was saved.
            Assert.IsTrue(File.Exists(Path.Combine(package.PackagesUpdatesPath, package.Repository.UrlSlug, package.Uid + "_" + package.Versions.First().Version + ".xml")));

            foreach (PackageFile packageFile in package.Versions.First().Files) {
                // Test that the file exist
                Assert.IsTrue(File.Exists(Path.Combine(package.UpdatesPath, packageFile.RelativePath)));

                // Test that the files md5 match what we were expecting
                Assert.AreEqual(packageFile.Md5, MD5.File(Path.Combine(package.UpdatesPath, packageFile.RelativePath)));
            }
        }

        /// <summary>
        ///     Tests that the method simply moves the files we have specified from one location to another.
        /// </summary>
        [Test]
        public void TestFlatPackedPackageMigrateTemporaryFiles() {
            if (Directory.Exists(FlatPackedPackageMigrateTemporaryFilesValidUpdatesPath) == true) {
                Directory.Delete(FlatPackedPackageMigrateTemporaryFilesValidUpdatesPath, true);
            }

            Directory.CreateDirectory(Path.Combine(FlatPackedPackageMigrateTemporaryFilesValidTemporaryPath, "SubDirectory"));

            File.WriteAllText(Path.Combine(FlatPackedPackageMigrateTemporaryFilesValidTemporaryPath, "File.txt"), "This is a file in the base directory.");
            File.WriteAllText(Path.Combine(FlatPackedPackageMigrateTemporaryFilesValidTemporaryPath, @"SubDirectory\AnotherFile.txt"), "This is a file in the sub directory.");

            var package = new FlatPackedPackage() {
                TemporaryUpdatesPath = FlatPackedPackageMigrateTemporaryFilesValidTemporaryPath,
                UpdatesPath = FlatPackedPackageMigrateTemporaryFilesValidUpdatesPath,
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
                                Name = "File.txt",
                                Size = 89,
                                LastModified = new DateTime(2012, 7, 3, 10, 5, 26),
                                RelativePath = @"File.txt",
                                Md5 = MD5.File(Path.Combine(FlatPackedPackageMigrateTemporaryFilesValidTemporaryPath, "File.txt"))
                            },
                            new PackageFile() {
                                Name = "AnotherFile.txt",
                                Size = 43,
                                LastModified = new DateTime(2012, 7, 3, 10, 4, 7),
                                RelativePath = @"SubDirectory\AnotherFile.txt",
                                Md5 = MD5.File(Path.Combine(FlatPackedPackageMigrateTemporaryFilesValidTemporaryPath, @"SubDirectory\AnotherFile.txt"))
                            }
                        }
                    }
                },
                State = PackageState.UpdateAvailable
            };

            package.MigrateTemporaryFiles(FlatPackedPackageMigrateTemporaryFilesValidTemporaryPath, package.Versions.First().Files);

            Assert.IsTrue(File.Exists(Path.Combine(FlatPackedPackageMigrateTemporaryFilesValidUpdatesPath, "File.txt")));
            Assert.IsTrue(File.Exists(Path.Combine(FlatPackedPackageMigrateTemporaryFilesValidUpdatesPath, @"SubDirectory\AnotherFile.txt")));
        }

        [Test]
        public void TestFlatPackedPackageSave() {
            var package = new FlatPackedPackage() {
                PackagesUpdatesPath = FlatPackedPackageSaveValidUpdatesPackagesPath,
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
                },
                State = PackageState.UpdateAvailable,
                Repository = new Repository() {
                    UrlSlug = "http://repo.myrcon.com/procon2/".UrlSlug()
                }
            };

            package.Save();

            XElement element = XElement.Load(Path.Combine(FlatPackedPackageSaveValidUpdatesPackagesPath, "http://repo.myrcon.com/procon2/".UrlSlug(), package.Uid + "_" + package.LatestVersion.Version + ".xml"));

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
        ///     Tests that we can serialize an object to a xml copy of what we see from the repository.
        /// </summary>
        [Test]
        public void TestFlatPackedPackageSerialization() {
            var package = new FlatPackedPackage() {
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


        /// <summary>
        ///     Tests that a package can be downloaded and installed when it needs to send a list of modified files to the server.
        ///     @todo We shouldn't piggy back off an existing package and instead publish one ourselves so we know it is there for the test.
        /// </summary>
        [Test, Ignore]
        
        public void TestFlatPackedPackageUpdate() {
            var requestWait = new AutoResetEvent(false);

            XElement element = XElement.Parse(@"<package>
    <uid>NewFile</uid>
    <name>NewFile</name>
    <package_versions>
        <package_version>
            <version>
                <major>0</major>
                <minor>0</minor>
                <build>0</build>
                <revision>1</revision>
            </version>
            <files/>
        </package_version>
        <package_version>
            <version>
                <major>0</major>
                <minor>0</minor>
                <build>0</build>
                <revision>2</revision>
            </version>
            <files>
                <file>
                    <name>BrandNewFile.txt</name>
                    <size>60</size>
                    <date>1341200430</date>
                    <relative_path>BrandNewFile.txt</relative_path>
                    <md5>8d28f9516ec9e9862793c0a12805f44b</md5>
                    <last_modified>2012-07-02T13:10:30+09:30</last_modified>
                </file>
                <file>
                    <name>ThisFileIsIdentical.txt</name>
                    <size>43</size>
                    <date>1341200398</date>
                    <relative_path>ThisFileIsIdentical.txt</relative_path>
                    <md5>01952484abdda0158c827a8848528e90</md5>
                    <last_modified>2012-07-02T13:09:58+09:30</last_modified>
                </file>
            </files>
        </package_version>
    </package_versions>
</package>");

            var package = element.FromXElement<FlatPackedPackage>();
            package.InstallPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Repositories\FlatPackedPackage\InstallOrUpdate\Update\Installed");
            package.UpdatesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Repositories\FlatPackedPackage\InstallOrUpdate\Update\Updates");
            package.TemporaryUpdatesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Repositories\FlatPackedPackage\InstallOrUpdate\Update\Updates\Temporary");
            package.PackagesUpdatesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Repositories\FlatPackedPackage\InstallOrUpdate\Update\Updates\Packages");

            package.Repository = new Repository() {
                Url = TestRepository.TestRepositoryUrl,
                UrlSlug = TestRepository.TestRepositoryUrl.UrlSlug()
            };

            package.SetInstalledVersion(package.Versions.First(pv => pv.Version.SystemVersion == new Version(0, 0, 0, 1)));
            package.SetAvailableVersion(package.Versions.First(pv => pv.Version.SystemVersion == new Version(0, 0, 0, 2)));

            package.StateChanged += (sender, args) => {
                if (package.State == PackageState.UpdateInstalled) {
                    requestWait.Set();
                }
            };

            package.InstallOrUpdate();

            Assert.IsTrue(requestWait.WaitOne(60000));

            // Test that the downloaded zip file was removed.
            Assert.IsFalse(Directory.Exists(package.TemporaryUpdatesPath));

            // Test that the package xml file was saved.
            Assert.IsTrue(File.Exists(Path.Combine(package.PackagesUpdatesPath, package.Repository.UrlSlug, package.Uid + "_" + package.Versions.Last().Version + ".xml")));

            foreach (PackageFile packageFile in package.Versions.Last().Files) {
                // Test that the file exist
                Assert.IsTrue(File.Exists(Path.Combine(package.UpdatesPath, packageFile.RelativePath)));

                // Test that the files md5 match what we were expecting
                Assert.AreEqual(packageFile.Md5, MD5.File(Path.Combine(package.UpdatesPath, packageFile.RelativePath)));
            }
        }
    }
}