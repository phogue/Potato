using NUnit.Framework;
using Procon.Service.Shared.Test.TestServiceController.Mocks;

namespace Procon.Service.Shared.Test.TestServiceController {
    [TestFixture]
    public class TestUninstallPackage {
        /// <summary>
        /// Tests the package dispatched for install if the entry status is set to Stopped
        /// </summary>
        [Test]
        public void TestInstallDispatchedEntryStatusIsStopped() {
            var uninstalled = false;

            var service = new ServiceController() {
                Packages = new MockServicePackageManager() {
                    PackageInstalled = (sender, uri, packageId) => { uninstalled = true; }
                },
                Observer = {
                    Status = ServiceStatusType.Stopped
                },
                ServiceLoaderProxyType = typeof(MockServiceLoaderProxy)
            };

            service.UninstallPackage("id");

            Assert.IsTrue(uninstalled);

            service.Dispose();
        }

        /// <summary>
        /// Tests the package dispatched for install if the entry status is set to Started
        /// </summary>
        [Test]
        public void TestInstallDispatchedEntryStatusIsStarted() {
            var uninstalled = false;

            var service = new ServiceController() {
                Packages = new MockServicePackageManager() {
                    PackageInstalled = (sender, uri, packageId) => { uninstalled = true; }
                },
                Observer = {
                    Status = ServiceStatusType.Started
                },
                ServiceLoaderProxyType = typeof(MockServiceLoaderProxy)
            };

            service.UninstallPackage("id");

            Assert.IsTrue(uninstalled);

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

            service.UninstallPackage("id");

            Assert.AreEqual(ServiceStatusType.Started, service.Observer.Status);

            service.Dispose();
        }
    }
}
