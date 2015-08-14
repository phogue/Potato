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
using NUnit.Framework;
using NuGet;
using Potato.Core.Packages;
using Potato.Core.Shared;
using Potato.Core.Shared.Models;
using Potato.Core.Test.Packages.Mocks;

namespace Potato.Core.Test.Packages.TestPackagesController {
    [TestFixture]
    public class TestPackagesMergePackage {
        [SetUp]
        public void Initialize() {
            SharedReferences.Setup();
        }

        /// <summary>
        /// Tests that attempting the command without any users in the security controller will
        /// result in insufficient permissions
        /// </summary>
        [Test]
        public void TestResultInsufficientPermissions() {
            var packages = new PackagesController();

            var result = packages.Tunnel(CommandBuilder.PackagesMergePackage("id").SetOrigin(CommandOrigin.Remote).SetAuthentication(new CommandAuthenticationModel() {
                Username = "Phogue"
            }));

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.InsufficientPermissions, result.CommandResultType);
        }

        /// <summary>
        /// Tests that supplying an empty package id will result in an invalid parameter
        /// </summary>
        [Test]
        public void TestResultInvalidParameter() {
            var packages = new PackagesController();

            var result = packages.Tunnel(CommandBuilder.PackagesMergePackage("").SetOrigin(CommandOrigin.Local));

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.InvalidParameter, result.CommandResultType);
        }

        /// <summary>
        /// Tests that supplying a package id we have no knowledge of will result in a DoesNotExists error.
        /// </summary>
        [Test]
        public void TestResultDoesNotExists() {
            var packages = new PackagesController();

            var result = packages.Tunnel(CommandBuilder.PackagesMergePackage("this-does-not-exist").SetOrigin(CommandOrigin.Local));

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.DoesNotExists, result.CommandResultType);
        }

        /// <summary>
        /// Tests that updating/installing a package with an identical version will result in an AlreadyExists error.
        /// </summary>
        [Test]
        public void TestResultAlreadyExists() {
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
                    Version = "1.0.0",
                    Tags = "Procon Potato Tag2"
                }
            }));

            cache.Build(localRepository);

            var packages = new PackagesController() {
                Cache = cache
            };

            var result = packages.Tunnel(CommandBuilder.PackagesMergePackage("A").SetOrigin(CommandOrigin.Local));

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.AlreadyExists, result.CommandResultType);
        }

        /// <summary>
        /// Tests the merge command will succeed if the package is not currently installed.
        /// </summary>
        [Test]
        public void TestResultUninstalledSuccess() {
            var cache = new RepositoryCache();
            var localRepository = new MockPackageRepository();

            cache.Add("localhost");

            cache.SourceRepositories.TryAdd("localhost", new MockPackageRepository(new List<IPackage>() {
                new DataServicePackage() {
                    Id = "A",
                    Version = "1.0.0",
                    Tags = "Procon Potato Tag2",
                    IsLatestVersion = true
                }
            }));

            cache.Build(localRepository);

            var packages = new PackagesController() {
                Cache = cache
            };

            var result = packages.Tunnel(CommandBuilder.PackagesMergePackage("A").SetOrigin(CommandOrigin.Local));

            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Success, result.CommandResultType);
        }

        /// <summary>
        /// Tests the merge command will succeed if an update is available for the package.
        /// </summary>
        [Test]
        public void TestResultUpdateAvailableSuccess() {
            var cache = new RepositoryCache();
            var localRepository = new MockPackageRepository(new List<IPackage>() {
                new DataServicePackage() {
                    Id = "A",
                    Version = "1.0.0",
                    Tags = "Potato Tag2"
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

            cache.Build(localRepository);

            var packages = new PackagesController() {
                Cache = cache
            };

            var result = packages.Tunnel(CommandBuilder.PackagesMergePackage("A").SetOrigin(CommandOrigin.Local));

            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Success, result.CommandResultType);
        }
    }
}
