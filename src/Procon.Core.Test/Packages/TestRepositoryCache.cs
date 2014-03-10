using System;
using System.Collections.Generic;
using System.Linq;
using NuGet;
using NUnit.Framework;
using Procon.Core.Packages;
using Procon.Core.Shared.Models;
using Procon.Core.Test.Packages.Mocks;

namespace Procon.Core.Test.Packages {
    [TestFixture]
    public class TestRepositoryCache {
        [SetUp]
        public void Initialize() {
            SharedReferences.Setup();
        }

        /// <summary>
        /// Tests that building with no repository and an empty local repository will result
        /// in zero package wrappers.
        /// </summary>
        [Test]
        public void TestCacheBuildEmptyLocalRepository() {
            var cache = new RepositoryCache();
            var localRepository = new MockPackageRepository();

            cache.Build(localRepository);

            Assert.IsEmpty(cache.Repositories.SelectMany(repository => repository.Packages));
        }

        /// <summary>
        /// Tests that building with a local repository with packages will orphan these packages
        /// because no remote repositories have been included.
        /// </summary>
        [Test]
        public void TestCacheBuildOrphanedLocalRepository() {
            var cache = new RepositoryCache();
            var localRepository = new MockPackageRepository(
                new List<IPackage>() {
                    new DataServicePackage() {
                        Id = "A",
                        Version = "1.0.0",
                        Tags = "Procon Tag2"
                    }
                }
            );

            cache.Build(localRepository);

            Assert.IsNotEmpty(cache.Repositories.First(repository => repository.IsOrphanage == true).Packages);
            Assert.AreEqual("A", cache.Repositories.First(repository => repository.IsOrphanage == true).Packages.First().Id);
            Assert.AreEqual(PackageState.Installed, cache.Repositories.First(repository => repository.IsOrphanage == true).Packages.First().State);
        }

        /// <summary>
        /// Tests that an exception thrown during the cache rebuild source repository fetch will be attached to the model
        /// </summary>
        [Test]
        public void TestExceptionOnCacheBuildAttachedToRepositoryModel() {
            var cache = new RepositoryCache();
            var localRepository = new MockPackageRepository();

            cache.Add("localhost");

            cache.SourceRepositories.TryAdd("localhost", new MockExceptionPackageRepository());

            // Now successfully build the repository..
            cache.Build(localRepository);

            Assert.AreEqual("GetPackages Exception", cache.Repositories.First(repository => repository.IsOrphanage == false).CacheError);
        }

        /// <summary>
        /// Tests the last cache build error is nulled when the repository is successfully built.
        /// </summary>
        [Test]
        public void TestLastCacheBuildErrorNulled() {
            var cache = new RepositoryCache();
            var localRepository = new MockPackageRepository();

            cache.Add("localhost");

            cache.SourceRepositories.TryAdd("localhost", new MockPackageRepository(new List<IPackage>() {
                new DataServicePackage() {
                    Id = "A",
                    Version = "1.0.0",
                    Tags = "Procon Tag2",
                    IsLatestVersion = true
                }
            }));

            cache.Repositories.First(repository => repository.IsOrphanage == false).CacheError = "Error!!";

            // Now successfully build the repository..
            cache.Build(localRepository);

            Assert.IsNull(cache.Repositories.First(repository => repository.IsOrphanage == false).CacheError);
        }

        /// <summary>
        /// Tests the stamp on the repository model is set on building the cache.
        /// </summary>
        [Test]
        public void TestLastCacheBuildStampSet() {
            var cache = new RepositoryCache();
            var localRepository = new MockPackageRepository();

            cache.Add("localhost");

            cache.SourceRepositories.TryAdd("localhost", new MockPackageRepository(new List<IPackage>() {
                new DataServicePackage() {
                    Id = "A",
                    Version = "1.0.0",
                    Tags = "Procon Tag2",
                    IsLatestVersion = true
                }
            }));

            // Now successfully build the repository..
            cache.Build(localRepository);

            Assert.Greater(cache.Repositories.First(repository => repository.IsOrphanage == false).CacheStamp, DateTime.Now.AddSeconds(-5));
        }

        /// <summary>
        /// Tests that a single package is available for installing from a remote source
        /// with nothing installed locally.
        /// </summary>
        [Test]
        public void TestCacheBuildSingleRemoteRepositoryEmptyLocalRepository() {
            var cache = new RepositoryCache();
            var localRepository = new MockPackageRepository();

            cache.Add("localhost");

            cache.SourceRepositories.TryAdd("localhost", new MockPackageRepository(new List<IPackage>() {
                new DataServicePackage() {
                    Id = "A",
                    Version = "1.0.0",
                    Tags = "Procon Tag2",
                    IsLatestVersion = true
                }
            }));

            cache.Build(localRepository);

            Assert.IsNotEmpty(cache.Repositories.SelectMany(repository => repository.Packages));
            Assert.AreEqual("A", cache.Repositories.SelectMany(repository => repository.Packages).First().Id);
            Assert.AreEqual(PackageState.NotInstalled, cache.Repositories.SelectMany(repository => repository.Packages).First().State);
        }

        /// <summary>
        /// Tests that we ignore packages that do not have a Procon tag.
        /// </summary>
        [Test]
        public void TestIgnoreMissingProconTag() {
            var cache = new RepositoryCache();
            var localRepository = new MockPackageRepository();

            cache.Add("localhost");

            cache.SourceRepositories.TryAdd("localhost", new MockPackageRepository(new List<IPackage>() {
                new DataServicePackage() {
                    Id = "A",
                    Version = "1.0.0",
                    Tags = "Tag1 Tag2",
                    IsLatestVersion = true
                },
                new DataServicePackage() {
                    Id = "B",
                    Version = "1.0.0",
                    Tags = "Procon Tag2",
                    IsLatestVersion = true
                },
                new DataServicePackage() {
                    Id = "C",
                    Version = "1.0.0",
                    Tags = "Tag1 Tag2",
                    IsLatestVersion = true
                }
            }));

            cache.Build(localRepository);

            Assert.AreEqual(1, cache.Repositories.SelectMany(repository => repository.Packages).Count());
            Assert.AreEqual("B", cache.Repositories.SelectMany(repository => repository.Packages).First().Id);
        }

        /// <summary>
        /// Tests that a single package is installed from a remote source
        /// with the package installed locally.
        /// </summary>
        [Test]
        public void TestCacheBuildSingleRemoteRepositoryInstalledLocalRepository() {
            var cache = new RepositoryCache();
            var localRepository = new MockPackageRepository(new List<IPackage>() {
                new DataServicePackage() {
                    Id = "A",
                    Version = "1.0.0",
                    Tags = "Procon Tag2"
                }
            });

            cache.Add("localhost");

            cache.SourceRepositories.TryAdd("localhost", new MockPackageRepository(new List<IPackage>() {
                new DataServicePackage() {
                    Id = "A",
                    Version = "1.0.0",
                    Tags = "Procon Tag2",
                    IsLatestVersion = true
                }
            }));

            cache.Build(localRepository);

            Assert.IsNotEmpty(cache.Repositories.SelectMany(repository => repository.Packages));
            Assert.AreEqual("A", cache.Repositories.SelectMany(repository => repository.Packages).First().Id);
            Assert.AreEqual(PackageState.Installed, cache.Repositories.SelectMany(repository => repository.Packages).First().State);
        }

        /// <summary>
        /// Tests that a single package is installed locally with a newer version available on
        /// the remote repository will show as update available.
        /// </summary>
        [Test]
        public void TestCacheBuildSingleRemoteRepositoryUpdateAvailableLocalRepository() {
            var cache = new RepositoryCache();
            var localRepository = new MockPackageRepository(new List<IPackage>() {
                new DataServicePackage() {
                    Id = "A",
                    Version = "1.0.0",
                    Tags = "Procon Tag2"
                }
            });

            cache.Add("localhost");

            cache.SourceRepositories.TryAdd("localhost", new MockPackageRepository(new List<IPackage>() {
                new DataServicePackage() {
                    Id = "A",
                    Version = "2.0.0",
                    Tags = "Procon Tag2",
                    IsLatestVersion = true
                }
            }));

            cache.Build(localRepository);

            Assert.IsNotEmpty(cache.Repositories.SelectMany(repository => repository.Packages));
            Assert.AreEqual("A", cache.Repositories.SelectMany(repository => repository.Packages).First().Id);
            Assert.AreEqual(PackageState.UpdateAvailable, cache.Repositories.SelectMany(repository => repository.Packages).First().State);
        }

        /// <summary>
        /// Tests clearing repositories will maintain the package orphanage when the repository list is empty.
        /// </summary>
        [Test]
        public void TestCacheClearEmptyRepositoryListMaintainOrphanedRepository() {
            var cache = new RepositoryCache();

            cache.Clear();

            Assert.IsNotEmpty(cache.Repositories);
            Assert.IsNotNull(cache.Repositories.First(repository => repository.IsOrphanage == true));
        }

        /// <summary>
        /// Tests clearing repositories will maintain the package orphanage when a repository exists in the list.
        /// </summary>
        [Test]
        public void TestCacheClearSingleRepositoryListMaintainOrphanedRepository() {
            var cache = new RepositoryCache();

            cache.Add("localhost");

            cache.Clear();

            Assert.IsNotEmpty(cache.Repositories);
            Assert.IsNotNull(cache.Repositories.First(repository => repository.IsOrphanage == true));
        }
    }
}
