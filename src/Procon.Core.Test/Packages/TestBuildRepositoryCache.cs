using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using NuGet;
using Procon.Core.Events;
using Procon.Core.Packages;
using Procon.Core.Shared.Models;
using Procon.Core.Test.Packages.Mocks;

namespace Procon.Core.Test.Packages {
    [TestFixture]
    public class TestBuildRepositoryCache {
        /// <summary>
        /// Tests building the cache will be completed successfully
        /// </summary>
        [Test]
        public void TestRepositoryCacheBuilt() {
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

            PackagesController packages = new PackagesController() {
                LocalRepository = localRepository,
                Cache = cache
            };

            packages.BuildRepositoryCache();

            Assert.IsNotNull(packages.Cache.Repositories.First(repository => repository.Uri == "localhost"));
            Assert.AreEqual("A", packages.Cache.Repositories.First(repository => repository.Uri == "localhost").Packages.First().Id);
            Assert.AreEqual(PackageState.UpdateAvailable, packages.Cache.Repositories.First(repository => repository.Uri == "localhost").Packages.First().State);
        }

        /// <summary>
        /// Tests that building the cache will log an event with the cached repository details.
        /// </summary>
        [Test]
        public void TestRepositoryCacheBuiltEventLogged() {
            EventsController events = new EventsController();

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

            PackagesController packages = new PackagesController() {
                LocalRepository = localRepository,
                Cache = cache
            };

            packages.Shared.Events = events;

            packages.BuildRepositoryCache();

            Assert.IsNotEmpty(events.LoggedEvents);
            Assert.AreEqual("PackagesCacheRebuilt", events.LoggedEvents.First().Name);
            Assert.AreEqual("A", events.LoggedEvents.First().Now.Repositories.First(repository => repository.Uri == "localhost").Packages.First().Id);
            Assert.AreEqual(PackageState.UpdateAvailable, events.LoggedEvents.First().Now.Repositories.First(repository => repository.Uri == "localhost").Packages.First().State);
        }
    }
}
