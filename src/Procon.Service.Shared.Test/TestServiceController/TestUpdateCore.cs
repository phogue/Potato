using NUnit.Framework;
using Procon.Service.Shared.Test.TestServiceController.Mocks;

namespace Procon.Service.Shared.Test.TestServiceController {
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
