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
using System.Collections.Generic;
using NUnit.Framework;
using NuGet;
using Potato.Service.Shared.Packages;
using Potato.Service.Shared.Test.TestServicePackages.Mocks;

namespace Potato.Service.Shared.Test.TestServicePackages {
    [TestFixture]
    public class TestMergePackage {
        /// <summary>
        /// Tests that a package will be installed when a merge package request is sent
        /// and the package is not currently installed.
        /// </summary>
        [Test]
        public void TestInstallCleanDispatched() {
            var dispatcher = new MockPackageManagerDispatch();

            var sources = new ConcurrentDictionary<string, IPackageRepository>();

            sources.TryAdd("localhost", new MockPackageRepository(new List<IPackage>() {
                new DataServicePackage() {
                    Id = "A",
                    Version = "1.0.0",
                    IsLatestVersion = true
                }
            }));

            var packages = new ServicePackageManager() {
                LocalRepository = new MockPackageRepository() {
                    Uri = Defines.PackagesDirectory.FullName
                },
                PackageManagerDispatch = dispatcher,
                SourceRepositories = sources
            };

            packages.MergePackage("localhost", "A");

            Assert.IsTrue(dispatcher.DispatchedInstallPackage);
        }

        /// <summary>
        /// Tests that a package will be updated if a newer version is available
        /// </summary>
        [Test]
        public void TestUpdateDispatched() {
            var dispatcher = new MockPackageManagerDispatch();

            var sources = new ConcurrentDictionary<string, IPackageRepository>();

            sources.TryAdd("localhost", new MockPackageRepository(new List<IPackage>() {
                new DataServicePackage() {
                    Id = "A",
                    Version = "2.0.0",
                    IsLatestVersion = true
                }
            }));

            var packages = new ServicePackageManager() {
                LocalRepository = new MockPackageRepository(new List<IPackage>() {
                    new DataServicePackage() {
                        Id = "A",
                        Version = "1.0.0"
                    }
                }) {
                    Uri = Defines.PackagesDirectory.FullName
                },
                PackageManagerDispatch = dispatcher,
                SourceRepositories = sources
            };

            packages.MergePackage("localhost", "A");

            Assert.IsTrue(dispatcher.DispatchedUpdatePackage);
        }

        /// <summary>
        /// Tests that the action is canceled if the package with the same version is already installed.
        /// </summary>
        [Test]
        public void TestInstallAlreadyExistsCanceled() {
            var canceled = false;

            var sources = new ConcurrentDictionary<string, IPackageRepository>();

            sources.TryAdd("localhost", new MockPackageRepository(new List<IPackage>() {
                new DataServicePackage() {
                    Id = "A",
                    Version = "1.0.0",
                    IsLatestVersion = true
                }
            }));

            var packages = new ServicePackageManager() {
                LocalRepository = new MockPackageRepository(new List<IPackage>() {
                    new DataServicePackage() {
                        Id = "A",
                        Version = "1.0.0"
                    }
                }) {
                    Uri = Defines.PackagesDirectory.FullName
                },
                SourceRepositories = sources,
                PackageActionCanceled = packageId => canceled = true,
                PackageManagerDispatch = new MockPackageManagerDispatch()
            };

            packages.MergePackage("localhost", "A");

            Assert.IsTrue(canceled);
        }

        /// <summary>
        /// Tests that a delegate will be called if the package id is missing from a source
        /// </summary>
        [Test]
        public void TestPackageMissing() {
            var missing = false;

            var sources = new ConcurrentDictionary<string, IPackageRepository>();

            sources.TryAdd("localhost", new MockPackageRepository(new List<IPackage>() {
                new DataServicePackage() {
                    Id = "A",
                    Version = "1.0.0"
                }
            }));

            var packages = new ServicePackageManager() {
                LocalRepository = new MockPackageRepository() {
                    Uri = Defines.PackagesDirectory.FullName
                },
                SourceRepositories = sources,
                PackageMissing = packageId => missing = true,
                PackageManagerDispatch = new MockPackageManagerDispatch()
            };

            packages.MergePackage("localhost", "B");

            Assert.IsTrue(missing);
        }

        /// <summary>
        /// Tests the initialize delegate is called when merging a package
        /// </summary>
        [Test]
        public void TestBeforeRepositoryInitialize() {
            var before = false;

            var packages = new ServicePackageManager() {
                BeforeRepositoryInitialize = () => before = true,
                PackageManagerDispatch = new MockPackageManagerDispatch()
            };

            packages.MergePackage("localhost", "A");

            Assert.IsTrue(before);
        }

        /// <summary>
        /// Test general exception when the local repository is null (no assumptions about the location of the installed packages)
        /// </summary>
        [Test]
        public void TestOnRepositoryExceptionGeneral() {
            var hint = "";

            var packages = new ServicePackageManager() {
                RepositoryException = (h, exception) => hint = h,
                PackageManagerDispatch = new MockPackageManagerDispatch()
            };

            packages.MergePackage("localhost", "A");

            Assert.AreEqual("ServicePackages.MergePackage.GeneralCatch", hint);
        }

        /// <summary>
        /// Test an exception is fired if we actually dispatch to our erronous package manager. While mocked
        /// this tests that if an error occurs during a dispatch it is captured.
        /// </summary>
        [Test]
        public void TestOnRepositoryExceptionInstallPackage() {
            var hint = "";

            var sources = new ConcurrentDictionary<string, IPackageRepository>();

            sources.TryAdd("localhost", new MockPackageRepository(new List<IPackage>() {
                new DataServicePackage() {
                    Id = "A",
                    Version = "1.0.0",
                    IsLatestVersion = true
                }
            }));

            var packages = new ServicePackageManager() {
                LocalRepository = new MockPackageRepository() {
                    Uri = Defines.PackagesDirectory.FullName
                },
                SourceRepositories = sources,
                RepositoryException = (h, exception) => hint = h
            };

            packages.MergePackage("localhost", "A");

            Assert.AreEqual("ServicePackages.MergePackage.InstallPackage", hint);
        }

        /// <summary>
        /// Test an exception is fired if we actually dispatch to our erronous package manager. While mocked
        /// this tests that if an error occurs during a dispatch it is captured.
        /// </summary>
        [Test]
        public void TestOnRepositoryExceptionUpdatePackage() {
            var hint = "";

            var sources = new ConcurrentDictionary<string, IPackageRepository>();

            sources.TryAdd("localhost", new MockPackageRepository(new List<IPackage>() {
                new DataServicePackage() {
                    Id = "A",
                    Version = "2.0.0",
                    IsLatestVersion = true
                }
            }));

            var packages = new ServicePackageManager() {
                LocalRepository = new MockPackageRepository(new List<IPackage>() {
                    new DataServicePackage() {
                        Id = "A",
                        Version = "1.0.0"
                    }
                }) {
                    Uri = Defines.PackagesDirectory.FullName
                },
                SourceRepositories = sources,
                RepositoryException = (h, exception) => hint = h
            };

            packages.MergePackage("localhost", "A");

            Assert.AreEqual("ServicePackages.MergePackage.UpdatePackage", hint);
        }

        /// <summary>
        /// Tests the before source package fetch delegate is called when merging a package.
        /// </summary>
        [Test]
        public void TestBeforeSourcePackageFetch() {
            var before = false;

            var sources = new ConcurrentDictionary<string, IPackageRepository>();

            sources.TryAdd("localhost", new MockPackageRepository(new List<IPackage>() {
                new DataServicePackage() {
                    Id = "A",
                    Version = "1.0.0"
                }
            }));

            var packages = new ServicePackageManager() {
                LocalRepository = new MockPackageRepository() {
                    Uri = Defines.PackagesDirectory.FullName
                },
                SourceRepositories = sources,
                BeforeSourcePackageFetch = () => before = true,
                PackageManagerDispatch = new MockPackageManagerDispatch()
            };

            packages.MergePackage("localhost", "A");

            Assert.IsTrue(before);
        }

        /// <summary>
        /// Tests the before local package fetch delegate is called when merging a package.
        /// </summary>
        [Test]
        public void TestOnBeforeLocalPackageFetch() {
            var before = false;

            var sources = new ConcurrentDictionary<string, IPackageRepository>();

            sources.TryAdd("localhost", new MockPackageRepository(new List<IPackage>() {
                new DataServicePackage() {
                    Id = "A",
                    Version = "1.0.0"
                }
            }));

            var packages = new ServicePackageManager() {
                LocalRepository = new MockPackageRepository() {
                    Uri = Defines.PackagesDirectory.FullName
                },
                SourceRepositories = sources,
                BeforeLocalPackageFetch = () => before = true,
                PackageManagerDispatch = new MockPackageManagerDispatch()
            };

            packages.MergePackage("localhost", "A");

            Assert.IsTrue(before);
        }
    }
}
