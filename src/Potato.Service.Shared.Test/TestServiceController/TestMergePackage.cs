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
    [TestFixture]
    public class TestMergePackage {
        /// <summary>
        /// Tests the package dispatched for install if the entry status is set to Stopped
        /// </summary>
        [Test]
        public void TestInstallDispatchedEntryStatusIsStopped() {
            var merged = false;

            var service = new ServiceController() {
                Packages = new MockServicePackageManager() {
                    PackageInstalled = (sender, uri, packageId) => { merged = true; }
                },
                Observer = {
                    Status = ServiceStatusType.Stopped
                },
                ServiceLoaderProxyType = typeof(MockServiceLoaderProxy)
            };

            service.MergePackage("localhost", "id");

            Assert.IsTrue(merged);

            service.Dispose();
        }

        /// <summary>
        /// Tests the package dispatched for install if the entry status is set to Started
        /// </summary>
        [Test]
        public void TestInstallDispatchedEntryStatusIsStarted() {
            var merged = false;

            var service = new ServiceController() {
                Packages = new MockServicePackageManager() {
                    PackageInstalled = (sender, uri, packageId) => { merged = true; }
                },
                Observer = {
                    Status = ServiceStatusType.Started
                },
                ServiceLoaderProxyType = typeof(MockServiceLoaderProxy)
            };

            service.MergePackage("localhost", "id");

            Assert.IsTrue(merged);

            service.Dispose();
        }

        /// <summary>
        /// Tests that from a stopped state the eventual state is Started
        /// </summary>
        [Test]
        public void TestFromStoppedEndsAsStarted() {
            var service = new ServiceController() {
                Packages = new MockServicePackageManager(),
                Observer = {
                    Status = ServiceStatusType.Stopped
                },
                ServiceLoaderProxyType = typeof(MockServiceLoaderProxy)
            };

            service.MergePackage("localhost", "id");

            Assert.AreEqual(ServiceStatusType.Started, service.Observer.Status);

            service.Dispose();
        }
    }
}
