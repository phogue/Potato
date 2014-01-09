﻿using System.Collections.Generic;
using NUnit.Framework;
using NuGet;
using Procon.Core.Packages;
using Procon.Core.Shared;
using Procon.Core.Test.Packages.Mocks;

namespace Procon.Core.Test.Packages {
    [TestFixture]
    public class TestCommandPackagesUninstallPackage {
        /// <summary>
        /// Tests that attempting the command without any users in the security controller will
        /// result in insufficient permissions
        /// </summary>
        [Test]
        public void TestResultInsufficientPermissions() {
            PackagesController packages = new PackagesController();

            CommandResultArgs result = packages.Tunnel(CommandBuilder.PackagesUninstallPackage("id").SetOrigin(CommandOrigin.Remote).SetUsername("Phogue"));

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.InsufficientPermissions, result.Status);
        }

        /// <summary>
        /// Tests that supplying an empty package id will result in an invalid parameter
        /// </summary>
        [Test]
        public void TestResultInvalidParameter() {
            PackagesController packages = new PackagesController();

            CommandResultArgs result = packages.Tunnel(CommandBuilder.PackagesUninstallPackage("").SetOrigin(CommandOrigin.Local));

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.InvalidParameter, result.Status);
        }

        /// <summary>
        /// Tests that supplying a package id we have no knowledge of will result in a DoesNotExists error.
        /// </summary>
        [Test]
        public void TestResultDoesNotExists() {
            PackagesController packages = new PackagesController();

            CommandResultArgs result = packages.Tunnel(CommandBuilder.PackagesUninstallPackage("this-does-not-exist").SetOrigin(CommandOrigin.Local));

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.DoesNotExists, result.Status);
        }

        /// <summary>
        /// Tests uninstalling a package that is available but not currently installed will result in a AlreadyExists error.
        /// </summary>
        [Test]
        public void TestResultAlreadyExists() {
            var cache = new RepositoryCache();
            var localRepository = new MockPackageRepository();

            cache.Add("localhost");

            cache.SourceRepositories.TryAdd("localhost", new MockPackageRepository(new List<IPackage>() {
                new DataServicePackage() {
                    Id = "A",
                    Version = "1.0.0"
                }
            }));

            cache.Build(localRepository);

            PackagesController packages = new PackagesController() {
                Cache = cache
            };

            CommandResultArgs result = packages.Tunnel(CommandBuilder.PackagesUninstallPackage("A").SetOrigin(CommandOrigin.Local));

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.AlreadyExists, result.Status);
        }

        /// <summary>
        /// Tests the merge command will succeed if the package is currently installed.
        /// </summary>
        [Test]
        public void TestResultInstalledSuccess() {
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
                    Version = "1.0.0"
                }
            }));

            cache.Build(localRepository);

            PackagesController packages = new PackagesController() {
                Cache = cache
            };

            CommandResultArgs result = packages.Tunnel(CommandBuilder.PackagesUninstallPackage("A").SetOrigin(CommandOrigin.Local));

            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Success, result.Status);
        }

        /// <summary>
        /// Tests the uninstall command will succeed if an update is available for the package.
        /// </summary>
        [Test]
        public void TestResultUpdateAvailableSuccess() {
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

            cache.Build(localRepository);

            PackagesController packages = new PackagesController() {
                Cache = cache
            };

            CommandResultArgs result = packages.Tunnel(CommandBuilder.PackagesUninstallPackage("A").SetOrigin(CommandOrigin.Local));

            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Success, result.Status);
        }
    }
}