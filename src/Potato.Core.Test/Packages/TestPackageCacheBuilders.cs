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
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using NuGet;
using Potato.Core.Packages;
using Potato.Core.Shared.Models;

namespace Potato.Core.Test.Packages {
    [TestFixture]
    public class TestPackageCacheBuilders {
        [SetUp]
        public void Initialize() {
            SharedReferences.Setup();
        }

        /// <summary>
        /// Tests that the available builder will add packages marking them as NotInstalled.
        /// </summary>
        [Test]
        public void TestAvailableBuilderEmptyRepository() {
            var packages = new List<PackageWrapperModel>();

            new AvailableCacheBuilder() {
                Cache = packages,
                Source = new List<IPackage>() {
                    new DataServicePackage() {
                        Id = "TestAvailableBuilderEmptyRepository",
                        Version = "1.0.0"
                    }
                }
            }.Build();

            Assert.IsNotEmpty(packages);
            Assert.AreEqual(PackageState.NotInstalled, packages.First().State);
        }

        /// <summary>
        /// Tests that the available builder will update existing packages maintaining an installed status if versions match
        /// </summary>
        [Test]
        public void TestAvailableBuilderInstalledRepositoryIdenticalVersions() {
            var packages = new List<PackageWrapperModel>() {
                new PackageWrapperModel() {
                    State = PackageState.Installed,
                    Installed = new PackageModel() {
                        Id = "TestAvailableBuilderInstalledRepositoryIdenticalVersions",
                        Version = "1.0.0"
                    }
                }
            };

            new AvailableCacheBuilder() {
                Cache = packages,
                Source = new List<IPackage>() {
                    new DataServicePackage() {
                        Id = "TestAvailableBuilderInstalledRepositoryIdenticalVersions",
                        Version = "1.0.0"
                    }
                }
            }.Build();

            Assert.IsNotEmpty(packages);
            Assert.AreEqual(PackageState.Installed, packages.First().State);
        }

        /// <summary>
        /// Tests that the available builder will update existing packages maintaining an installed status if versions match
        /// </summary>
        [Test]
        public void TestAvailableBuilderInstalledRepositoryNewerVersions() {
            var packages = new List<PackageWrapperModel>() {
                new PackageWrapperModel() {
                    State = PackageState.Installed,
                    Installed = new PackageModel() {
                        Id = "TestAvailableBuilderInstalledRepositoryNewerVersions",
                        Version = "1.0.0"
                    }
                }
            };

            new AvailableCacheBuilder() {
                Cache = packages,
                Source = new List<IPackage>() {
                    new DataServicePackage() {
                        Id = "TestAvailableBuilderInstalledRepositoryNewerVersions",
                        Version = "2.0.0"
                    }
                }
            }.Build();

            Assert.IsNotEmpty(packages);
            Assert.AreEqual(PackageState.UpdateAvailable, packages.First().State);
        }

        /// <summary>
        /// Tests that the repository will remain empty as it cannot validate the installed packages belong to it.
        /// </summary>
        [Test]
        public void TestInstalledBuilderEmptyRepository() {
            var packages = new List<PackageWrapperModel>();

            new InstalledCacheBuilder() {
                Cache = packages,
                Source = new List<IPackage>() {
                    new DataServicePackage() {
                        Id = "TestInstalledBuilderEmptyRepository",
                        Version = "1.0.0"
                    }
                }
            }.Build();

            Assert.IsEmpty(packages);
        }

        /// <summary>
        /// Tests that the repository will be set to installed (not update available) if the available package
        /// exists and has an identical version.
        /// </summary>
        [Test]
        public void TestInstalledBuilderAvailableRepositoryIdenticalVersions() {
            var packages = new List<PackageWrapperModel>() {
                new PackageWrapperModel() {
                    State = PackageState.NotInstalled,
                    Available = new PackageModel() {
                        Id = "TestInstalledBuilderAvailableRepositoryIdenticalVersions",
                        Version = "1.0.0"
                    }
                }
            };

            new InstalledCacheBuilder() {
                Cache = packages,
                Source = new List<IPackage>() {
                    new DataServicePackage() {
                        Id = "TestInstalledBuilderAvailableRepositoryIdenticalVersions",
                        Version = "1.0.0"
                    }
                }
            }.Build();

            Assert.IsNotEmpty(packages);
            Assert.AreEqual(PackageState.Installed, packages.First().State);
        }

        /// <summary>
        /// Tests that the repository will be set to UpdateAvailable if the available package
        /// exists and has a newer version.
        /// </summary>
        [Test]
        public void TestInstalledBuilderAvailableRepositoryNewerVersions() {
            var packages = new List<PackageWrapperModel>() {
                new PackageWrapperModel() {
                    State = PackageState.NotInstalled,
                    Available = new PackageModel() {
                        Id = "TestInstalledBuilderAvailableRepositoryNewerVersions",
                        Version = "2.0.0"
                    }
                }
            };

            new InstalledCacheBuilder() {
                Cache = packages,
                Source = new List<IPackage>() {
                    new DataServicePackage() {
                        Id = "TestInstalledBuilderAvailableRepositoryNewerVersions",
                        Version = "1.0.0"
                    }
                }
            }.Build();

            Assert.IsNotEmpty(packages);
            Assert.AreEqual(PackageState.UpdateAvailable, packages.First().State);
        }

        /// <summary>
        /// Tests that an orphaned package will be added to the repository
        /// </summary>
        [Test]
        public void TestOrphanedBuilderEmptyRepository() {
            var packages = new List<PackageWrapperModel>();

            new OrphanedCacheBuilder() {
                Cache = packages,
                Source = new List<IPackage>() {
                    new DataServicePackage() {
                        Id = "TestOrphanedBuilderEmptyRepository",
                        Version = "1.0.0"
                    }
                }
            }.Build();

            Assert.IsNotEmpty(packages);
            Assert.AreEqual(PackageState.Installed, packages.First().State);
        }

        /// <summary>
        /// Tests that an orphaned package will update an existing entry in the repostory.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         The initial state of the repository should never be hit during normal operation, but if for some reason it
        ///         will be corrected by the orphaned builder. The fact that we have the available package from the source means
        ///         that the package isn't orphaned.
        ///     </para>
        /// </remarks>
        [Test]
        public void TestOrphanedBuilderAvailableRepositoryNewerVersion() {
            var packages = new List<PackageWrapperModel>() {
                    new PackageWrapperModel() {
                        State = PackageState.NotInstalled,
                        Available = new PackageModel() {
                            Id = "TestOrphanedBuilderAvailableRepositoryNewerVersion",
                            // Even though it's newer it will be ignored as we've told the orphan builder
                            // that this package is unknown to us.
                            Version = "2.0.0"
                        }
                    }
            };

            new OrphanedCacheBuilder() {
                Cache = packages,
                Source = new List<IPackage>() {
                    new DataServicePackage() {
                        Id = "TestOrphanedBuilderAvailableRepositoryNewerVersion",
                        Version = "1.0.0"
                    }
                }
            }.Build();

            Assert.IsNotEmpty(packages);
            Assert.AreEqual(PackageState.Installed, packages.First().State);
        }
    }
}
