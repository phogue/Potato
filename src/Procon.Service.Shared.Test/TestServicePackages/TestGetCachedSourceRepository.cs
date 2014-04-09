#region Copyright
// Copyright 2014 Myrcon Pty. Ltd.
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
