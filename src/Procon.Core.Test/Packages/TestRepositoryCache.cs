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
                        Version = "1.0.0"
                    }
                }
            );

            cache.Build(localRepository);

            Assert.IsNotEmpty(cache.Repositories.First(repository => repository.IsOrphanage == true).Packages);
            Assert.AreEqual("A", cache.Repositories.First(repository => repository.IsOrphanage == true).Packages.First().Id);
            Assert.AreEqual(PackageState.Installed, cache.Repositories.First(repository => repository.IsOrphanage == true).Packages.First().State);
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
                    Version = "1.0.0"
                }
            }));

            cache.Build(localRepository);

            Assert.IsNotEmpty(cache.Repositories.SelectMany(repository => repository.Packages));
            Assert.AreEqual("A", cache.Repositories.SelectMany(repository => repository.Packages).First().Id);
            Assert.AreEqual(PackageState.NotInstalled, cache.Repositories.SelectMany(repository => repository.Packages).First().State);
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
                    Version = "1.0.0"
                }
            });

            cache.Add("localhost");

            cache.SourceRepositories.TryAdd("localhost", new MockPackageRepository(new List<IPackage>() {
                new DataServicePackage() {
                    Id = "A",
                    Version = "1.0.0"
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
                    Version = "1.0.0"
                }
            });

            cache.Add("localhost");

            cache.SourceRepositories.TryAdd("localhost", new MockPackageRepository(new List<IPackage>() {
                new DataServicePackage() {
                    Id = "A",
                    Version = "2.0.0"
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
