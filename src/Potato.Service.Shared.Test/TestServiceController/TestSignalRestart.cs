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
using NUnit.Framework;
using Potato.Service.Shared.Test.TestServiceController.Mocks;

namespace Potato.Service.Shared.Test.TestServiceController {
    /// <summary>
    /// These tests are duplicated, but as a whole show that whatever state Potato is currently in
    /// calling Restart will eventually start a healthy instance.
    /// </summary>
    [TestFixture]
    public class TestSignalRestart {
        /// <summary>
        /// Tests the started is the final status from None status
        /// </summary>
        [Test]
        public void TestStartedFromNone() {
            var service = new ServiceController() {
                Packages = new MockServicePackageManager(),
                Settings = {
                    ServiceUpdateCore = false
                },
                ServiceLoaderProxyType = typeof(MockServiceLoaderProxy),
                Observer = {
                    Status = ServiceStatusType.None
                }
            };

            service.Restart();

            Assert.AreEqual(ServiceStatusType.Started, service.Observer.Status);

            service.Dispose();
        }

        /// <summary>
        /// Tests the started is the final status from Started status
        /// </summary>
        [Test]
        public void TestStartedFromStarted() {
            var service = new ServiceController() {
                Packages = new MockServicePackageManager(),
                Settings = {
                    ServiceUpdateCore = false
                },
                ServiceLoaderProxyType = typeof(MockServiceLoaderProxy),
                Observer = {
                    Status = ServiceStatusType.Started
                }
            };

            service.Restart();

            Assert.AreEqual(ServiceStatusType.Started, service.Observer.Status);

            service.Dispose();
        }

        /// <summary>
        /// Tests the started is the final status from Starting status
        /// </summary>
        [Test]
        public void TestStartedFromStarting() {
            var service = new ServiceController() {
                Packages = new MockServicePackageManager(),
                Settings = {
                    ServiceUpdateCore = false
                },
                ServiceLoaderProxyType = typeof(MockServiceLoaderProxy),
                Observer = {
                    Status = ServiceStatusType.Starting
                }
            };

            service.Restart();

            Assert.AreEqual(ServiceStatusType.Started, service.Observer.Status);

            service.Dispose();
        }

        /// <summary>
        /// Tests the started is the final status from Stopped status
        /// </summary>
        [Test]
        public void TestStartedFromStopped() {
            var service = new ServiceController() {
                Packages = new MockServicePackageManager(),
                Settings = {
                    ServiceUpdateCore = false
                },
                ServiceLoaderProxyType = typeof(MockServiceLoaderProxy),
                Observer = {
                    Status = ServiceStatusType.Stopped
                }
            };

            service.Restart();

            Assert.AreEqual(ServiceStatusType.Started, service.Observer.Status);

            service.Dispose();
        }

        /// <summary>
        /// Tests the started is the final status from Stopping status
        /// </summary>
        [Test]
        public void TestStartedFromStopping() {
            var service = new ServiceController() {
                Packages = new MockServicePackageManager(),
                Settings = {
                    ServiceUpdateCore = false
                },
                ServiceLoaderProxyType = typeof(MockServiceLoaderProxy),
                Observer = {
                    Status = ServiceStatusType.Stopping
                }
            };

            service.Restart();

            Assert.AreEqual(ServiceStatusType.Started, service.Observer.Status);

            service.Dispose();
        }
    }
}
