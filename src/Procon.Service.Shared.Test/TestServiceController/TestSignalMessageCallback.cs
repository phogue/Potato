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
using NUnit.Framework;
using Procon.Service.Shared.Test.TestServiceController.Mocks;

namespace Procon.Service.Shared.Test.TestServiceController {
    [TestFixture]
    public class TestSignalMessageCallback {
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
        /// Tests the signal begin callback is called
        /// </summary>
        [Test]
        public void TestSignalBeginCallback() {
            var signaled = false;

            var service = new ServiceController() {
                Settings = {
                    ServiceUpdateCore = false
                },
                SignalBegin = (controller, message) => signaled = true
            };

            service.SignalMessage(new ServiceMessage() {
                Name = "help"
            });

            Assert.IsTrue(signaled);

            service.Dispose();
        }

        /// <summary>
        /// Tests the signal end callback is called
        /// </summary>
        [Test]
        public void TestSignalEndCallback() {
            var signaled = false;

            var service = new ServiceController() {
                Settings = {
                    ServiceUpdateCore = false
                },
                SignalEnd = (controller, message, seconds) => signaled = true
            };

            service.SignalMessage(new ServiceMessage() {
                Name = "help"
            });

            Assert.IsTrue(signaled);

            service.Dispose();
        }

        /// <summary>
        /// Tests an error callback is called when a parameter is missing from a signal message
        /// that requires parameters
        /// </summary>
        [Test]
        public void TestSignalParameterError() {
            var error = false;

            var service = new ServiceController() {
                Settings = {
                    ServiceUpdateCore = false
                },
                SignalParameterError = (controller, list) => error = true
            };

            service.SignalMessage(new ServiceMessage() {
                Name = "merge",
                Arguments = new Dictionary<string, string>() {
                    { "uri", "localhost" }
                }
            });

            Assert.IsTrue(error);

            service.Dispose();
        }

        /// <summary>
        /// Tests the signal statistics callback is called
        /// </summary>
        [Test]
        public void TestSignalStatisticsCallback() {
            var signaled = false;

            var service = new ServiceController() {
                Settings = {
                    ServiceUpdateCore = false
                },
                SignalStatistics = (controller, message) => signaled = true
            };

            service.SignalMessage(new ServiceMessage() {
                Name = "statistics"
            });

            Assert.IsTrue(signaled);

            service.Dispose();
        }

        /// <summary>
        /// Tests the signal help callback is called
        /// </summary>
        [Test]
        public void TestSignalHelpCallback() {
            var signaled = false;

            var service = new ServiceController() {
                Settings = {
                    ServiceUpdateCore = false
                },
                SignalHelp = controller => signaled = true
            };

            service.SignalMessage(new ServiceMessage() {
                Name = "help"
            });

            Assert.IsTrue(signaled);

            service.Dispose();
        }

        /// <summary>
        /// Tests the signal result callback is called
        /// </summary>
        [Test]
        public void TestSignalResultCallback() {
            var signaled = false;

            var service = new ServiceController() {
                Settings = {
                    ServiceUpdateCore = false
                },
                SignalResult = (controller, message) => signaled = true
            };

            service.SignalMessage(new ServiceMessage() {
                Name = "result"
            });

            Assert.IsTrue(signaled);

            service.Dispose();
        }
    }
}
