using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using NuGet;
using Procon.Core.Packages;
using Procon.Core.Shared.Models;

namespace Procon.Core.Test.Packages {
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
            RepositoryModel repository = new RepositoryModel();

            new AvailableCacheBuilder() {
                Repository = repository,
                Packages = new List<IPackage>() {
                    new DataServicePackage() {
                        Id = "TestAvailableBuilderEmptyRepository",
                        Version = "1.0.0"
                    }
                }
            }.Build();

            Assert.IsNotEmpty(repository.Packages);
            Assert.AreEqual(PackageState.NotInstalled, repository.Packages.First().State);
        }

        /// <summary>
        /// Tests that the available builder will update existing packages maintaining an installed status if versions match
        /// </summary>
        [Test]
        public void TestAvailableBuilderInstalledRepositoryIdenticalVersions() {
            RepositoryModel repository = new RepositoryModel() {
                Packages = new List<PackageWrapperModel>() {
                    new PackageWrapperModel() {
                        State = PackageState.Installed,
                        Installed = new PackageModel() {
                            Id = "TestAvailableBuilderInstalledRepositoryIdenticalVersions",
                            Version = "1.0.0"
                        }
                    }
                }
            };

            new AvailableCacheBuilder() {
                Repository = repository,
                Packages = new List<IPackage>() {
                    new DataServicePackage() {
                        Id = "TestAvailableBuilderInstalledRepositoryIdenticalVersions",
                        Version = "1.0.0"
                    }
                }
            }.Build();

            Assert.IsNotEmpty(repository.Packages);
            Assert.AreEqual(PackageState.Installed, repository.Packages.First().State);
        }

        /// <summary>
        /// Tests that the available builder will update existing packages maintaining an installed status if versions match
        /// </summary>
        [Test]
        public void TestAvailableBuilderInstalledRepositoryNewerVersions() {
            RepositoryModel repository = new RepositoryModel() {
                Packages = new List<PackageWrapperModel>() {
                    new PackageWrapperModel() {
                        State = PackageState.Installed,
                        Installed = new PackageModel() {
                            Id = "TestAvailableBuilderInstalledRepositoryNewerVersions",
                            Version = "1.0.0"
                        }
                    }
                }
            };

            new AvailableCacheBuilder() {
                Repository = repository,
                Packages = new List<IPackage>() {
                    new DataServicePackage() {
                        Id = "TestAvailableBuilderInstalledRepositoryNewerVersions",
                        Version = "2.0.0"
                    }
                }
            }.Build();

            Assert.IsNotEmpty(repository.Packages);
            Assert.AreEqual(PackageState.UpdateAvailable, repository.Packages.First().State);
        }

        /// <summary>
        /// Tests that the repository will remain empty as it cannot validate the installed packages belong to it.
        /// </summary>
        [Test]
        public void TestInstalledBuilderEmptyRepository() {
            RepositoryModel repository = new RepositoryModel();

            new InstalledCacheBuilder() {
                Repository = repository,
                Packages = new List<IPackage>() {
                    new DataServicePackage() {
                        Id = "TestInstalledBuilderEmptyRepository",
                        Version = "1.0.0"
                    }
                }
            }.Build();

            Assert.IsEmpty(repository.Packages);
        }

        /// <summary>
        /// Tests that the repository will be set to installed (not update available) if the available package
        /// exists and has an identical version.
        /// </summary>
        [Test]
        public void TestInstalledBuilderAvailableRepositoryIdenticalVersions() {
            RepositoryModel repository = new RepositoryModel() {
                Packages = new List<PackageWrapperModel>() {
                    new PackageWrapperModel() {
                        State = PackageState.NotInstalled,
                        Available = new PackageModel() {
                            Id = "TestInstalledBuilderAvailableRepositoryIdenticalVersions",
                            Version = "1.0.0"
                        }
                    }
                }
            };

            new InstalledCacheBuilder() {
                Repository = repository,
                Packages = new List<IPackage>() {
                    new DataServicePackage() {
                        Id = "TestInstalledBuilderAvailableRepositoryIdenticalVersions",
                        Version = "1.0.0"
                    }
                }
            }.Build();

            Assert.IsNotEmpty(repository.Packages);
            Assert.AreEqual(PackageState.Installed, repository.Packages.First().State);
        }

        /// <summary>
        /// Tests that the repository will be set to UpdateAvailable if the available package
        /// exists and has a newer version.
        /// </summary>
        [Test]
        public void TestInstalledBuilderAvailableRepositoryNewerVersions() {
            RepositoryModel repository = new RepositoryModel() {
                Packages = new List<PackageWrapperModel>() {
                    new PackageWrapperModel() {
                        State = PackageState.NotInstalled,
                        Available = new PackageModel() {
                            Id = "TestInstalledBuilderAvailableRepositoryNewerVersions",
                            Version = "2.0.0"
                        }
                    }
                }
            };

            new InstalledCacheBuilder() {
                Repository = repository,
                Packages = new List<IPackage>() {
                    new DataServicePackage() {
                        Id = "TestInstalledBuilderAvailableRepositoryNewerVersions",
                        Version = "1.0.0"
                    }
                }
            }.Build();

            Assert.IsNotEmpty(repository.Packages);
            Assert.AreEqual(PackageState.UpdateAvailable, repository.Packages.First().State);
        }

        /// <summary>
        /// Tests that an orphaned package will be added to the repository
        /// </summary>
        [Test]
        public void TestOrphanedBuilderEmptyRepository() {
            RepositoryModel repository = new RepositoryModel();

            new OrphanedCacheBuilder() {
                Repository = repository,
                Packages = new List<IPackage>() {
                    new DataServicePackage() {
                        Id = "TestOrphanedBuilderEmptyRepository",
                        Version = "1.0.0"
                    }
                }
            }.Build();

            Assert.IsNotEmpty(repository.Packages);
            Assert.AreEqual(PackageState.Installed, repository.Packages.First().State);
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
            RepositoryModel repository = new RepositoryModel() {
                Packages = new List<PackageWrapperModel>() {
                    new PackageWrapperModel() {
                        State = PackageState.NotInstalled,
                        Available = new PackageModel() {
                            Id = "TestOrphanedBuilderAvailableRepositoryNewerVersion",
                            // Even though it's newer it will be ignored as we've told the orphan builder
                            // that this package is unknown to us.
                            Version = "2.0.0"
                        }
                    }
                }
            };

            new OrphanedCacheBuilder() {
                Repository = repository,
                Packages = new List<IPackage>() {
                    new DataServicePackage() {
                        Id = "TestOrphanedBuilderAvailableRepositoryNewerVersion",
                        Version = "1.0.0"
                    }
                }
            }.Build();

            Assert.IsNotEmpty(repository.Packages);
            Assert.AreEqual(PackageState.Installed, repository.Packages.First().State);
        }
    }
}
