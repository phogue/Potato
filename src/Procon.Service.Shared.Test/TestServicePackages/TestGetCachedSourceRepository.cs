using System;
using System.Collections.Concurrent;
using NUnit.Framework;
using NuGet;
using Procon.Service.Shared.Packages;

namespace Procon.Service.Shared.Test.TestServicePackages {
    [TestFixture]
    public class TestGetCachedSourceRepository {
        /// <summary>
        /// Tests that an empty repository source cache will have a new repository made
        /// and added to the cache.
        /// </summary>
        [Test]
        public void TestAddNewCachedSourceRepository() {
            var packages = new ServicePackageManager();

            Assert.IsEmpty(packages.SourceRepositories);

            IPackageRepository repository = packages.GetCachedSourceRepository(Defines.PackagesDirectory.FullName);

            Assert.AreEqual(Defines.PackagesDirectory.FullName, repository.Source);
        }

        /// <summary>
        /// Tests that a source repository will be pulled from the source cache
        /// </summary>
        [Test]
        public void TestRepositoryPulledFromCache() {
            var sources = new ConcurrentDictionary<String, IPackageRepository>();

            sources.TryAdd(Defines.PackagesDirectory.FullName, PackageRepositoryFactory.Default.CreateRepository(Defines.PackagesDirectory.FullName));

            var packages = new ServicePackageManager() {
                SourceRepositories = sources
            };

            Assert.IsNotEmpty(packages.SourceRepositories);
            Assert.AreEqual(1, packages.SourceRepositories.Count);

            IPackageRepository repository = packages.GetCachedSourceRepository(Defines.PackagesDirectory.FullName);

            Assert.AreEqual(Defines.PackagesDirectory.FullName, repository.Source);
            Assert.AreEqual(1, packages.SourceRepositories.Count);
        }
    }
}
