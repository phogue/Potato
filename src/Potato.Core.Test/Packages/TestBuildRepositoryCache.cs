#region Copyright
// Copyright 2015 Geoff Green.
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
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using NuGet;
using Potato.Core.Events;
using Potato.Core.Packages;
using Potato.Core.Shared.Models;
using Potato.Core.Test.Packages.Mocks;

namespace Potato.Core.Test.Packages {
    [TestFixture]
    public class TestBuildRepositoryCache {
        [SetUp]
        public void Initialize() {
            SharedReferences.Setup();
        }

        /// <summary>
        /// Tests building the cache will be completed successfully
        /// </summary>
        [Test]
        public void TestRepositoryCacheBuilt() {
            var cache = new RepositoryCache();
            var localRepository = new MockPackageRepository(new List<IPackage>() {
                new DataServicePackage() {
                    Id = "A",
                    Version = "1.0.0",
                    Tags = "Procon Potato Tag2"
                }
            });

            cache.Add("localhost");

            cache.SourceRepositories.TryAdd("localhost", new MockPackageRepository(new List<IPackage>() {
                new DataServicePackage() {
                    Id = "A",
                    Version = "2.0.0",
                    Tags = "Procon Potato Tag2",
                    IsLatestVersion = true
                }
            }));

            var packages = new PackagesController() {
                LocalRepository = localRepository,
                Cache = cache
            };

            packages.Poke();

            Assert.IsNotNull(packages.Cache.Repositories.First(repository => repository.Uri == "localhost"));
            Assert.AreEqual("A", packages.Cache.Repositories.First(repository => repository.Uri == "localhost").Packages.First().Id);
            Assert.AreEqual(PackageState.UpdateAvailable, packages.Cache.Repositories.First(repository => repository.Uri == "localhost").Packages.First().State);
        }

        /// <summary>
        /// Tests that building the cache will log an event with the cached repository details.
        /// </summary>
        [Test]
        public void TestRepositoryCacheBuiltEventLogged() {
            var events = new EventsController();

            var cache = new RepositoryCache();
            var localRepository = new MockPackageRepository(new List<IPackage>() {
                new DataServicePackage() {
                    Id = "A",
                    Version = "1.0.0",
                    Tags = "Procon Potato Tag2"
                }
            });

            cache.Add("localhost");

            cache.SourceRepositories.TryAdd("localhost", new MockPackageRepository(new List<IPackage>() {
                new DataServicePackage() {
                    Id = "A",
                    Version = "2.0.0",
                    Tags = "Procon Potato Tag2",
                    IsLatestVersion = true
                }
            }));

            var packages = new PackagesController() {
                LocalRepository = localRepository,
                Cache = cache
            };

            packages.Shared.Events = events;

            packages.Poke();

            Assert.IsNotEmpty(events.LoggedEvents);
            Assert.AreEqual("PackagesCacheRebuilt", events.LoggedEvents.First().Name);
            Assert.AreEqual("A", events.LoggedEvents.First().Now.Repositories.First(repository => repository.Uri == "localhost").Packages.First().Id);
            Assert.AreEqual(PackageState.UpdateAvailable, events.LoggedEvents.First().Now.Repositories.First(repository => repository.Uri == "localhost").Packages.First().State);
        }
    }
}
