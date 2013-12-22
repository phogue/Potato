#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using NUnit.Framework;
using Procon.Core.Repositories;
using Procon.Core.Repositories.Serialization;
using Procon.Core.Utils;
using Procon.Net.Shared.Utils;
using Procon.Net.Utils;

#endregion

namespace Procon.Core.Test.Repositories {
    [TestFixture]
    public class TestRepository {
        public static readonly string TestRepositoryUrl = "http://local.repo.myrcon.com/";
        public static readonly string TestRepositoryUsername = "admin";
        public static readonly string TestRepositoryPassword = "1234";

        /// <summary>
        ///     Tests that a package is simply added when it does not exist (same uid)
        /// </summary>
        [Test]
        public void TestAddOrCopyPackageNonExistant() {
            var repository = new Repository();

            var package = new Package() {
                Name = "PackageName",
                Uid = "PackageUid",
                PackageType = PackageType.Language,
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
                                LastModified = DateTime.Now,
                                Name = "FileName.txt",
                                Md5 = MD5.String(""),
                                RelativePath = @"relative/FileName.txt",
                                Size = 100
                            }
                        }
                    }
                }
            };

            Assert.AreEqual(0, repository.Packages.Count(p => p.Uid == package.Uid));

            repository.AddOrUpdatePackage(package);

            Assert.AreEqual(1, repository.Packages.Count);
        }

        [Test]
        public void TestAddOrCopyPackageUpdateExisting() {
            var originalPackage = new Package() {
                Name = "Existing Package Name",
                Uid = "PackageUid"
            };

            var repository = new Repository() {
                Packages = new List<Package>() {
                    originalPackage
                }
            };

            var package = new Package() {
                Name = "Modified Package Name",
                Uid = "PackageUid",
                PackageType = PackageType.Language
            };

            Assert.IsNotNull(originalPackage);
            Assert.AreEqual("Existing Package Name", originalPackage.Name);

            // Now update the existing package.
            repository.AddOrUpdatePackage(package);

            Assert.AreEqual(1, repository.Packages.Count);
            Assert.AreEqual("Modified Package Name", originalPackage.Name);
            Assert.AreEqual(PackageType.Language, originalPackage.PackageType);

            // Make sure the original object was kept in tact with data copied over it.
            Assert.IsTrue(ReferenceEquals(originalPackage, repository.Packages.First()));
        }

        /// <summary>
        ///     Tests that providing an incorrect password will result in bad authentication.
        /// </summary>
        [Test]
        public void TestBeginAuthenticationTestFailed() {
            var requestWait = new AutoResetEvent(false);

            bool successful = false;

            var repository = new Repository() {
                Url = TestRepositoryUrl,
                Username = TestRepositoryUsername,
                Password = "incorrect password"
            };

            repository.AuthenticationSuccess += (sender) => {
                successful = true;
                requestWait.Set();
            };

            repository.AuthenticationFailed += (sender) => {
                successful = false;
                requestWait.Set();
            };

            repository.BeginAuthenticationTest();

            Assert.IsTrue(requestWait.WaitOne(60000));

            Assert.IsFalse(successful);
        }

        /// <summary>
        ///     Tests that the repository can be authenticated against.
        /// </summary>
        [Test, Ignore]
        
        public void TestBeginAuthenticationTestSuccess() {
            var requestWait = new AutoResetEvent(false);

            bool successful = false;

            var repository = new Repository() {
                Url = TestRepositoryUrl,
                Username = TestRepositoryUsername,
                Password = TestRepositoryPassword
            };

            repository.AuthenticationSuccess += (sender) => {
                successful = true;
                requestWait.Set();
            };

            repository.AuthenticationFailed += (sender) => {
                successful = false;
                requestWait.Set();
            };

            repository.BeginAuthenticationTest();

            Assert.IsTrue(requestWait.WaitOne(60000));

            Assert.IsTrue(successful);
        }

        /// <summary>
        ///     Tests that a file will be published
        /// </summary>
        [Test, Ignore]
        
        public void TestBeginPublishSuccess() {
            String repositoryPublishValidPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Repositories\Publish\Valid");
            String randomlyGeneratedContentPath = Path.Combine(repositoryPublishValidPath, "RandomlyGeneratedContent.txt");

            // Build = Days since new years 2000. Revision = Seconds since midnight tonight.
            var version = new Version(0, 0, (int) (DateTime.Now - new DateTime(2000, 1, 1)).TotalDays, (int) (DateTime.Now - new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day)).TotalSeconds);

            // Randomly generate content for the random file so it's hash will always be different.
            File.WriteAllText(randomlyGeneratedContentPath, StringExtensions.RandomString(100));

            String randomlyGeneratedContentMd5 = MD5.File(randomlyGeneratedContentPath);

            var requestWait = new AutoResetEvent(false);

            bool successful = false;

            var repository = new Repository() {
                Url = TestRepositoryUrl,
                Username = TestRepositoryUsername,
                Password = TestRepositoryPassword
            };

            repository.PublishSuccess += (sender) => {
                successful = true;
                requestWait.Set();
            };

            var package = new Package() {
                Name = "Package Name",
                Uid = "PackageUid",
            };

            repository.BeginPublish(package, version, new DirectoryInfo(repositoryPublishValidPath).Zip());

            Assert.IsTrue(requestWait.WaitOne(60000));
            Assert.IsTrue(successful);

            // Now query the repository
            requestWait.Reset();
            successful = false;

            repository.RepositoryLoaded += (sender) => {
                successful = true;
                requestWait.Set();
            };

            repository.BeginQueryRequest();

            Assert.IsTrue(requestWait.WaitOne(60000));
            Assert.IsTrue(successful);

            // Now validate the respository has been updated properly.
            Package queriedPackage = repository.Packages.Find(p => p.Uid == "PackageUid");

            Assert.IsNotNull(queriedPackage);
            Assert.AreEqual(version, queriedPackage.LatestVersion.Version.SystemVersion);
            Assert.AreEqual(randomlyGeneratedContentMd5, queriedPackage.LatestVersion.Files.First().Md5);
        }

        /// <summary>
        ///     Tests that the repository can be queried.
        /// </summary>
        [Test, Ignore]
        
        public void TestBeginQueryRequest() {
            var requestWait = new AutoResetEvent(false);

            var repository = new Repository() {
                Url = TestRepositoryUrl
            };

            repository.RepositoryLoaded += (sender) => requestWait.Set();

            repository.BeginQueryRequest();

            Assert.IsTrue(requestWait.WaitOne(60000));

            Package package = repository.Packages.FirstOrDefault(p => p.Uid == "DownloadCacheTest");

            Assert.IsNotNull(package);
        }

        /// <summary>
        ///     Tests that the repository can have its cache rebuilt
        /// </summary>
        [Test, Ignore]
        
        public void TestBeginRebuildCacheSuccess() {
            var requestWait = new AutoResetEvent(false);

            bool successful = false;

            var repository = new Repository() {
                Url = TestRepositoryUrl,
                Username = TestRepositoryUsername,
                Password = TestRepositoryPassword
            };

            repository.RebuildCacheSuccess += (sender) => {
                successful = true;
                requestWait.Set();
            };

            repository.BeginRebuildCache();

            Assert.IsTrue(requestWait.WaitOne(60000));

            Assert.IsTrue(successful);
        }

        /// <summary>
        ///     Tests that loading a directory with only a single invalid xml file
        ///     will result in no packages being loaded.
        /// </summary>
        [Test]
        public void TestReadDirectoryInvalidRepository() {
            var repository = new Repository();

            List<Package> loadedPackages = repository.ReadDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Repositories\ReadDirectory\Invalid"));

            Assert.AreEqual(0, loadedPackages.Count);
            Assert.AreEqual(0, repository.Packages.Count);
        }

        /// <summary>
        ///     Tests that loading a directory with a valid package will validate
        /// </summary>
        [Test, Ignore]
        
        public void TestReadDirectoryValidRepository() {
            var repository = new Repository();

            List<Package> loadedPackages = repository.ReadDirectory(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Repositories\ReadDirectory\Valid"));

            Package package = repository.Packages.First();

            Assert.AreEqual(1, repository.Packages.Count);
            Assert.IsTrue(ReferenceEquals(loadedPackages.First(), package));

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
    }
}