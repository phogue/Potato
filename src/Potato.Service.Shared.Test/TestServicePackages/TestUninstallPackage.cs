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
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using NUnit.Framework;
using NuGet;
using Potato.Service.Shared.Packages;
using Potato.Service.Shared.Test.TestServicePackages.Mocks;

namespace Potato.Service.Shared.Test.TestServicePackages {
    [TestFixture]
    public class TestUninstallPackage {
        /// <summary>
        /// Tests that a package will be updated if a newer version is available
        /// </summary>
        [Test]
        public void TestUninstallDispatched() {
            var dispatcher = new MockPackageManagerDispatch();

            var sources = new ConcurrentDictionary<string, IPackageRepository>();

            sources.TryAdd("localhost", new MockPackageRepository());

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

            packages.UninstallPackage("A");

            Assert.IsTrue(dispatcher.DispatchedUninstallPackage);
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

            var sources = new ConcurrentDictionary<string, IPackageRepository>();

            sources.TryAdd("localhost", new MockPackageRepository());

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

            packages.UninstallPackage("A");

            Assert.AreEqual("ServicePackages.UninstallPackage.UninstallPackage", hint);
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

            packages.UninstallPackage("A");

            Assert.IsTrue(before);
        }
    }
}
