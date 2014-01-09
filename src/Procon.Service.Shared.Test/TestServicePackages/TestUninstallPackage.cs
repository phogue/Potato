using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using NUnit.Framework;
using NuGet;
using Procon.Service.Shared.Packages;
using Procon.Service.Shared.Test.TestServicePackages.Mocks;

namespace Procon.Service.Shared.Test.TestServicePackages {
    [TestFixture]
    public class TestUninstallPackage {
        /// <summary>
        /// Tests that a package will be updated if a newer version is available
        /// </summary>
        [Test]
        public void TestUninstallDispatched() {
            var dispatcher = new MockPackageManagerDispatch();

            var sources = new ConcurrentDictionary<String, IPackageRepository>();

            sources.TryAdd("localhost", new MockPackageRepository());

            var packages = new ServicePackageManager() {
                LocalRepository = new MockPackageRepository(new List<IPackage>() {
                    new DataServicePackage() {
                        Id = "A",
                        Version = "1.0.0"
                    }
                }) {
                    Uri = Defines.PackagesDirectory
                },
                PackageManagerDispatch = dispatcher,
                SourceRepositories = sources
            };

            packages.UninstallPackage("A");

            Assert.IsTrue(dispatcher.DispatchedUninstallPackage);
        }

        /// <summary>
        /// Tests that a delegate will be called if the package id is missing from a source
        /// </summary>
        [Test]
        public void TestPackageMissing() {
            var missing = false;

            var sources = new ConcurrentDictionary<String, IPackageRepository>();

            sources.TryAdd("localhost", new MockPackageRepository(new List<IPackage>() {
                new DataServicePackage() {
                    Id = "A",
                    Version = "1.0.0"
                }
            }));

            var packages = new ServicePackageManager() {
                LocalRepository = new MockPackageRepository() {
                    Uri = Defines.PackagesDirectory
                },
                SourceRepositories = sources,
                PackageMissing = packageId => missing = true,
                PackageManagerDispatch = new MockPackageManagerDispatch()
            };

            packages.UninstallPackage("B");

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

            packages.UninstallPackage("A");

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

            packages.UninstallPackage("A");

            Assert.AreEqual("ServicePackages.UninstallPackage.GeneralCatch", hint);
        }


        /// <summary>
        /// Test an exception is fired if we actually dispatch to our erronous package manager. While mocked
        /// this tests that if an error occurs during a dispatch it is captured.
        /// </summary>
        [Test]
        public void TestOnRepositoryExceptionUninstallPackage() {
            var hint = "";

            var sources = new ConcurrentDictionary<String, IPackageRepository>();

            sources.TryAdd("localhost", new MockPackageRepository());

            var packages = new ServicePackageManager() {
                LocalRepository = new MockPackageRepository(new List<IPackage>() {
                    new DataServicePackage() {
                        Id = "A",
                        Version = "1.0.0"
                    }
                }) {
                    Uri = Defines.PackagesDirectory
                },
                SourceRepositories = sources,
                RepositoryException = (h, exception) => hint = h
            };

            packages.UninstallPackage("A");

            Assert.AreEqual("ServicePackages.UninstallPackage.UninstallPackage", hint);
        }

        /// <summary>
        /// Tests the before local package fetch delegate is called when merging a package.
        /// </summary>
        [Test]
        public void TestOnBeforeLocalPackageFetch() {
            var before = false;

            var sources = new ConcurrentDictionary<String, IPackageRepository>();

            sources.TryAdd("localhost", new MockPackageRepository(new List<IPackage>() {
                new DataServicePackage() {
                    Id = "A",
                    Version = "1.0.0"
                }
            }));

            var packages = new ServicePackageManager() {
                LocalRepository = new MockPackageRepository() {
                    Uri = Defines.PackagesDirectory
                },
                SourceRepositories = sources,
                BeforeLocalPackageFetch = () => before = true,
                PackageManagerDispatch = new MockPackageManagerDispatch()
            };

            packages.UninstallPackage("A");

            Assert.IsTrue(before);
        }
    }
}
