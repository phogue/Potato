#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using NUnit.Framework;
using Procon.Core.Events;
using Procon.Core.Repositories;
using Procon.Core.Security;
using Procon.Core.Shared;
using Procon.Core.Shared.Events;
using Procon.Net.Shared.Utils;
using Procon.Net.Utils;

#endregion

namespace Procon.Core.Test.Repositories {
    [TestFixture]
    public class TestRepositoryControllerCommands {
        [SetUp]
        public void Initialize() {
            Directory.CreateDirectory(ExecuteInstalledPath);
            Directory.CreateDirectory(ExecutePackagesInstalledPath);
            Directory.CreateDirectory(ExecuteUpdatesPath);
            Directory.CreateDirectory(ExecutePackagesUpdatesPath);
            Directory.CreateDirectory(ExecuteTemporaryUpdatesPath);
        }

        protected static String ExecuteInstalledPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Repositories\RepositoryController\Commands\Installed");
        protected static String ExecutePackagesInstalledPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Repositories\RepositoryController\Commands\Installed\Packages");
        protected static String ExecuteUpdatesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Repositories\RepositoryController\Commands\Installed\Updates");
        protected static String ExecutePackagesUpdatesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Repositories\RepositoryController\Commands\Installed\Updates\Packages");
        protected static String ExecuteTemporaryUpdatesPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"Repositories\RepositoryController\Commands\Installed\Updates\Temporary");

        protected RepositoryController SetupRepositoryController() {
            var repository = new RepositoryController() {
                PackagesPath = ExecutePackagesInstalledPath,
                PackagesUpdatesPath = ExecutePackagesUpdatesPath,
                UpdatesPath = ExecuteUpdatesPath,
                InstallPath = ExecuteInstalledPath,
                TemporaryUpdatesPath = ExecuteTemporaryUpdatesPath,
                Security = new SecurityController(),
                Events = new EventsController()
            };

            repository.Execute();

            return repository;
        }


        /// <summary>
        ///     Tests that a package can be downloaded and installed.
        /// </summary>
        [Test]
        public void TestRepositoryControllerAddRemoteRepositoryAlreadyExists() {
            RepositoryController repository = SetupRepositoryController();

            repository.Tunnel(new Command() {
                CommandType = CommandType.PackagesAddRemoteRepository,
                Origin = CommandOrigin.Local,
                Parameters = new List<CommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            Content = new List<String>() {
                                TestRepository.TestRepositoryUrl
                            }
                        }
                    }
                }
            });

            CommandResultArgs result = repository.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.PackagesAddRemoteRepository,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    TestRepository.TestRepositoryUrl
                })
            });

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.AlreadyExists, result.Status);
        }

        /// <summary>
        ///     Tests that a non existant user will be denied from insufficient permission.
        /// </summary>
        [Test]
        public void TestRepositoryControllerAddRemoteRepositoryInsufficientPermission() {
            RepositoryController repository = SetupRepositoryController();

            CommandResultArgs result = repository.Tunnel(new Command() {
                CommandType = CommandType.PackagesAddRemoteRepository,
                Username = "Phogue",
                Origin = CommandOrigin.Remote,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "http://localhost/"
                })
            });

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.InsufficientPermissions, result.Status);
        }

        /// <summary>
        ///     Tests that a remote repository can be added to the repository controller
        /// </summary>
        [Test]
        public void TestRepositoryControllerAddRemoteRepositorySuccess() {
            RepositoryController repository = SetupRepositoryController();

            CommandResultArgs result = repository.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.PackagesAddRemoteRepository,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    TestRepository.TestRepositoryUrl
                })
            });

            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Success, result.Status);
        }

        /// <summary>
        ///     Tests that a non existant user will be denied from insufficient permission.
        /// </summary>
        [Test]
        public void TestRepositoryControllerIngoreAutomaticUpdatePackageInsufficientPermission() {
            RepositoryController repository = SetupRepositoryController();

            CommandResultArgs result = repository.Tunnel(new Command() {
                CommandType = CommandType.PackagesIngoreAutomaticUpdateOnPackage,
                Username = "Phogue",
                Origin = CommandOrigin.Remote,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "http://localhost/",
                    "PackageUid"
                })
            });

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.InsufficientPermissions, result.Status);
        }

        [Test, Ignore]
        
        public void TestRepositoryControllerIngoreAutomaticUpdatePackageNoDuplicationSuccess() {
            var requestWait = new AutoResetEvent(false);

            RepositoryController repository = SetupRepositoryController();

            repository.Events.EventLogged += (sender, args) => {
                if (args.GenericEventType == GenericEventType.RepositoriesPackagesRebuilt) {
                    requestWait.Set();
                }
            };

            repository.Tunnel(new Command() {
                CommandType = CommandType.PackagesAddRemoteRepository,
                Origin = CommandOrigin.Local,
                Parameters = new List<CommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            Content = new List<String>() {
                                TestRepository.TestRepositoryUrl
                            }
                        }
                    }
                }
            });

            Assert.IsTrue(requestWait.WaitOne(60000));

            // Add it first
            repository.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.PackagesIngoreAutomaticUpdateOnPackage,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    TestRepository.TestRepositoryUrl,
                    "PackageUid"
                })
            });

            // Now re add it. It's a success, but we only want a single reference kept (no duplications)
            CommandResultArgs result = repository.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.PackagesIngoreAutomaticUpdateOnPackage,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    TestRepository.TestRepositoryUrl,
                    "PackageUid"
                })
            });

            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Success, result.Status);
            Assert.AreEqual(1, repository.CachedAutoUpdateReferences.Count);
        }

        /// <summary>
        ///     Tests that a remote repository can be added to the repository controller
        /// </summary>
        [Test, Ignore]
        
        public void TestRepositoryControllerIngoreAutomaticUpdatePackageSuccess() {
            var requestWait = new AutoResetEvent(false);

            RepositoryController repository = SetupRepositoryController();

            repository.Events.EventLogged += (sender, args) => {
                if (args.GenericEventType == GenericEventType.RepositoriesPackagesRebuilt) {
                    requestWait.Set();
                }
            };

            repository.Tunnel(new Command() {
                CommandType = CommandType.PackagesAddRemoteRepository,
                Origin = CommandOrigin.Local,
                Parameters = new List<CommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            Content = new List<String>() {
                                TestRepository.TestRepositoryUrl
                            }
                        }
                    }
                }
            });

            Assert.IsTrue(requestWait.WaitOne(60000));

            CommandResultArgs result = repository.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.PackagesIngoreAutomaticUpdateOnPackage,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    TestRepository.TestRepositoryUrl,
                    "PackageUid"
                })
            });

            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Success, result.Status);
        }

        /// <summary>
        ///     Tests that a package can be downloaded and installed.
        /// </summary>
        [Test, Ignore]
        
        public void TestRepositoryControllerInstallPackageDoesNotExist() {
            var requestWait = new AutoResetEvent(false);

            RepositoryController repository = SetupRepositoryController();

            repository.Events.EventLogged += (sender, args) => {
                if (args.GenericEventType == GenericEventType.RepositoriesPackagesRebuilt) {
                    requestWait.Set();
                }
            };

            repository.Tunnel(new Command() {
                CommandType = CommandType.PackagesAddRemoteRepository,
                Origin = CommandOrigin.Local,
                Parameters = new List<CommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            Content = new List<String>() {
                                TestRepository.TestRepositoryUrl
                            }
                        }
                    }
                }
            });

            Assert.IsTrue(requestWait.WaitOne(60000));

            requestWait.Reset();

            FlatPackedPackage package = repository.Packages.Find(p => p.Repository.UrlSlug == TestRepository.TestRepositoryUrl.UrlSlug() && p.Uid == "PackageUid");

            package.StateChanged += (sender, args) => {
                if (package.State == PackageState.NotInstalled) {
                    requestWait.Set();
                }
            };

            CommandResultArgs result = repository.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.PackagesInstallPackage,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    TestRepository.TestRepositoryUrl.UrlSlug(),
                    "DoesNotExistPackageUid"
                })
            });

            Assert.IsTrue(requestWait.WaitOne(60000));

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.DoesNotExists, result.Status);
        }

        /// <summary>
        ///     Tests that a non existant user will be denied from insufficient permission.
        /// </summary>
        [Test]
        public void TestRepositoryControllerInstallPackageInsufficientPermission() {
            RepositoryController repository = SetupRepositoryController();

            CommandResultArgs result = repository.Tunnel(new Command() {
                CommandType = CommandType.PackagesInstallPackage,
                Username = "Phogue",
                Origin = CommandOrigin.Remote,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "repo_uid",
                    "package_uid"
                })
            });

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.InsufficientPermissions, result.Status);
        }

        /// <summary>
        ///     Tests that a package can be downloaded and installed.
        /// </summary>
        [Test, Ignore]
        
        public void TestRepositoryControllerInstallPackageSuccess() {
            var requestWait = new AutoResetEvent(false);

            RepositoryController repository = SetupRepositoryController();

            repository.Events.EventLogged += (sender, args) => {
                if (args.GenericEventType == GenericEventType.RepositoriesPackagesRebuilt) {
                    requestWait.Set();
                }
            };

            repository.Tunnel(new Command() {
                CommandType = CommandType.PackagesAddRemoteRepository,
                Origin = CommandOrigin.Local,
                Parameters = new List<CommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            Content = new List<String>() {
                                TestRepository.TestRepositoryUrl
                            }
                        }
                    }
                }
            });

            Assert.IsTrue(requestWait.WaitOne(60000));

            requestWait.Reset();

            FlatPackedPackage package = repository.Packages.Find(p => p.Repository.UrlSlug == TestRepository.TestRepositoryUrl.UrlSlug() && p.Uid == "PackageUid");

            package.StateChanged += (sender, args) => {
                if (package.State == PackageState.UpdateInstalled) {
                    requestWait.Set();
                }
            };

            CommandResultArgs result = repository.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.PackagesInstallPackage,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    TestRepository.TestRepositoryUrl.UrlSlug(),
                    "PackageUid"
                })
            });

            Assert.IsTrue(requestWait.WaitOne(60000));
        }

        /// <summary>
        ///     Tests that a non existant user will be denied from insufficient permission.
        /// </summary>
        [Test]
        public void TestRepositoryControllerPackagesAutomaticUpdateOnPackageInsufficientPermission() {
            RepositoryController repository = SetupRepositoryController();

            CommandResultArgs result = repository.Tunnel(new Command() {
                CommandType = CommandType.PackagesAutomaticUpdateOnPackage,
                Username = "Phogue",
                Origin = CommandOrigin.Remote,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "http://localhost/",
                    "PackageUid"
                })
            });

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.InsufficientPermissions, result.Status);
        }

        /// <summary>
        ///     Tests that a remote repository can be added to the repository controller
        /// </summary>
        [Test]
        public void TestRepositoryControllerPackagesAutomaticUpdateOnPackageSuccess() {
            var requestWait = new AutoResetEvent(false);

            RepositoryController repository = SetupRepositoryController();

            repository.Events.EventLogged += (sender, args) => {
                if (args.GenericEventType == GenericEventType.RepositoriesPackagesRebuilt) {
                    requestWait.Set();
                }
            };

            repository.Tunnel(new Command() {
                CommandType = CommandType.PackagesAddRemoteRepository,
                Origin = CommandOrigin.Local,
                Parameters = new List<CommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            Content = new List<String>() {
                                TestRepository.TestRepositoryUrl
                            }
                        }
                    }
                }
            });

            Assert.IsTrue(requestWait.WaitOne(60000));

            repository.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.PackagesIngoreAutomaticUpdateOnPackage,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    TestRepository.TestRepositoryUrl,
                    "PackageUid"
                })
            });

            CommandResultArgs result = repository.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.PackagesAutomaticUpdateOnPackage,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    TestRepository.TestRepositoryUrl,
                    "PackageUid"
                })
            });

            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Success, result.Status);
            Assert.AreEqual(0, repository.CachedAutoUpdateReferences.Count);
        }

        /// <summary>
        ///     Tests that a package can be downloaded and installed.
        /// </summary>
        [Test]
        public void TestRepositoryControllerRemoveRemoteRepositoryDoesNotExist() {
            RepositoryController repository = SetupRepositoryController();

            // This won't exist, nothing has been added yet.
            CommandResultArgs result = repository.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.PackagesRemoveRemoteRepository,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    TestRepository.TestRepositoryUrl
                })
            });

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.DoesNotExists, result.Status);
        }

        /// <summary>
        ///     Tests that a non existant user will be denied from insufficient permission.
        /// </summary>
        [Test]
        public void TestRepositoryControllerRemoveRemoteRepositoryInsufficientPermission() {
            RepositoryController repository = SetupRepositoryController();

            CommandResultArgs result = repository.Tunnel(new Command() {
                CommandType = CommandType.PackagesRemoveRemoteRepository,
                Username = "Phogue",
                Origin = CommandOrigin.Remote,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "http://localhost/"
                })
            });

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.InsufficientPermissions, result.Status);
        }

        /// <summary>
        ///     Tests that a remote repository can be added to the repository controller
        /// </summary>
        [Test]
        public void TestRepositoryControllerRemoveRemoteRepositorySuccess() {
            RepositoryController repository = SetupRepositoryController();

            repository.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.PackagesAddRemoteRepository,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    TestRepository.TestRepositoryUrl
                })
            });

            CommandResultArgs result = repository.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.PackagesRemoveRemoteRepository,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    TestRepository.TestRepositoryUrl
                })
            });

            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Success, result.Status);
        }
    }
}