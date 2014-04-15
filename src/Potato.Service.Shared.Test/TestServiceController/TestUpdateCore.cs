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
    public class TestUpdateCore {
        /// <summary>
        /// Tests that a stopped service controller will issue the call to update the core
        /// </summary>
        [Test]
        public void TestPackageCoreUpdateDispatched() {
            var merged = false;

            var service = new ServiceController() {
                Packages = new MockServicePackageManager() {
                    PackageInstalled = (sender, packageId, version) => { merged = true; }
                },
                Settings = {
                    ServiceUpdateCore = true
                },
                ServiceLoaderProxyType = typeof(MockServiceLoaderProxy)
            };

            service.UpdateCore();

            Assert.IsTrue(merged);

            service.Dispose();
        }

        /// <summary>
        /// Tests that no update call will occur if the ServiceUpdateCore setting is set to false.
        /// </summary>
        [Test]
        public void TestUpdateNotCalledWhenSettingIsFalse() {
            var merged = false;

            var service = new ServiceController() {
                Packages = new MockServicePackageManager() {
                    PackageInstalled = (sender, packageId, version) => { merged = true; }
                },
                Settings = {
                    ServiceUpdateCore = false
                },
                ServiceLoaderProxyType = typeof(MockServiceLoaderProxy)
            };

            service.UpdateCore();

            Assert.IsFalse(merged);

            service.Dispose();
        }

        /// <summary>
        /// Tests that no update call will occur if the service state is anything other than Stopped
        /// </summary>
        [Test]
        public void TestUpdateNotCalledWhenStatusIsStarted() {
            var merged = false;

            var service = new ServiceController() {
                Packages = new MockServicePackageManager() {
                    PackageInstalled = (sender, packageId, version) => { merged = true; }
                },
                Settings = {
                    ServiceUpdateCore = true
                },
                Observer = {
                    Status = ServiceStatusType.Started
                },
                ServiceLoaderProxyType = typeof(MockServiceLoaderProxy)
            };

            service.UpdateCore();

            Assert.IsFalse(merged);

            service.Dispose();
        }
    }
}
