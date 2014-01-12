using System.Collections.Generic;
using NUnit.Framework;
using Procon.Service.Shared.Test.TestServiceController.Mocks;

namespace Procon.Service.Shared.Test.TestServiceController {
    [TestFixture]
    public class TestSignalMessage {
        /// <summary>
        /// Tests that passing in a null parameter will return false.
        /// </summary>
        [Test]
        public void TestNullMessageReturnsFalse() {
            var service = new ServiceController();

            Assert.IsFalse(service.SignalMessage(null));

            service.Dispose();
        }

        /// <summary>
        /// Tests that passing in a nop message will return true
        /// </summary>
        [Test]
        public void TestNopMessageReturnsTrue() {
            var service = new ServiceController();

            Assert.IsTrue(service.SignalMessage(new ServiceMessage() {
                Name = "nop"
            }));

            service.Dispose();
        }

        /// <summary>
        /// Tests that passing in a junk message will return false
        /// </summary>
        [Test]
        public void TestJunkMessageReturnsFalse() {
            var service = new ServiceController();

            Assert.IsFalse(service.SignalMessage(new ServiceMessage() {
                Name = "junk"
            }));

            service.Dispose();
        }

        /// <summary>
        /// Tests that passing in a start message will return true
        /// </summary>
        [Test]
        public void TestStartMessageReturnsTrue() {
            var service = new ServiceController() {
                Settings = {
                    ServiceUpdateCore = false
                }
            };

            Assert.IsTrue(service.SignalMessage(new ServiceMessage() {
                Name = "start"
            }));

            service.Dispose();
        }

        /// <summary>
        /// Tests that passing in a stop message will return true
        /// </summary>
        [Test]
        public void TestStopMessageReturnsTrue() {
            var service = new ServiceController() {
                Settings = {
                    ServiceUpdateCore = false
                }
            };

            Assert.IsTrue(service.SignalMessage(new ServiceMessage() {
                Name = "stop"
            }));

            service.Dispose();
        }

        /// <summary>
        /// Tests that passing in a restart message will return true
        /// </summary>
        [Test]
        public void TestRestartMessageReturnsTrue() {
            var service = new ServiceController() {
                Settings = {
                    ServiceUpdateCore = false
                }
            };

            Assert.IsTrue(service.SignalMessage(new ServiceMessage() {
                Name = "restart"
            }));

            service.Dispose();
        }

        /// <summary>
        /// Tests that passing in a merge message will return true
        /// </summary>
        [Test]
        public void TestMergeMessageReturnsTrue() {
            var service = new ServiceController() {
                Settings = {
                    ServiceUpdateCore = false
                }
            };

            Assert.IsTrue(service.SignalMessage(new ServiceMessage() {
                Name = "merge"
            }));

            service.Dispose();
        }

        /// <summary>
        /// Tests that passing in a merge message with the correct parameters will 
        /// call the packages merge
        /// </summary>
        [Test]
        public void TestMergeMessageWithParametersCallsMerge() {
            var merged = false;

            var service = new ServiceController() {
                Settings = {
                    ServiceUpdateCore = false
                },
                Packages = new MockServicePackageManager() {
                    PackageInstalled = (sender, uri, packageId) => { merged = true; }
                }
            };

            service.SignalMessage(new ServiceMessage() {
                Name = "merge",
                Arguments = new Dictionary<string, string>() {
                    { "uri", "localhost" },
                    { "packageid", "id" }
                }
            });

            Assert.IsTrue(merged);

            service.Dispose();
        }

        /// <summary>
        /// Tests that passing in a uninstall message will return true
        /// </summary>
        [Test]
        public void TestUninstallMessageReturnsTrue() {
            var service = new ServiceController() {
                Settings = {
                    ServiceUpdateCore = false
                }
            };

            Assert.IsTrue(service.SignalMessage(new ServiceMessage() {
                Name = "uninstall"
            }));

            service.Dispose();
        }

        /// <summary>
        /// Tests that passing in a merge message with the correct parameters will 
        /// call the packages merge
        /// </summary>
        [Test]
        public void TestUninstallMessageWithParametersCallsMerge() {
            var uninstall = false;

            var service = new ServiceController() {
                Settings = {
                    ServiceUpdateCore = false
                },
                Packages = new MockServicePackageManager() {
                    PackageUninstalled = (sender, uri, packageId) => { uninstall = true; }
                }
            };

            service.SignalMessage(new ServiceMessage() {
                Name = "uninstall",
                Arguments = new Dictionary<string, string>() {
                    { "packageid", "id" }
                }
            });

            Assert.IsTrue(uninstall);

            service.Dispose();
        }

        /// <summary>
        /// Tests that passing in a help message will return true
        /// </summary>
        [Test]
        public void TestHelpMessageReturnsTrue() {
            var service = new ServiceController();

            Assert.IsTrue(service.SignalMessage(new ServiceMessage() {
                Name = "help"
            }));

            service.Dispose();
        }

        /// <summary>
        /// Tests that passing in a stats message will return true
        /// </summary>
        [Test]
        public void TestStatsMessageReturnsTrue() {
            var service = new ServiceController();

            Assert.IsTrue(service.SignalMessage(new ServiceMessage() {
                Name = "stats"
            }));

            service.Dispose();
        }

        /// <summary>
        /// Tests that passing in a statistics message will return true
        /// </summary>
        [Test]
        public void TestStatisticsMessageReturnsTrue() {
            var service = new ServiceController();

            Assert.IsTrue(service.SignalMessage(new ServiceMessage() {
                Name = "statistics"
            }));

            service.Dispose();
        }
    }
}
