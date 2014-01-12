using NUnit.Framework;
using Procon.Service.Shared.Test.TestServiceController.Mocks;

namespace Procon.Service.Shared.Test.TestServiceController {
    [TestFixture]
    public class TestPanic {
        /// <summary>
        /// Tests that a healthy service that has been stopped for 20 minutes will attempt a startup and succeed
        /// </summary>
        [Test]
        public void TestStoppedHealthyServiceInPanicWillRestart() {
            var service = new ServiceController() {
                Packages = new MockServicePackageManager(),
                Settings = {
                    ServiceUpdateCore = true
                },
                ServiceLoaderProxyType = typeof(MockServiceLoaderProxy)
            };

            service.Panic();

            Assert.AreEqual(ServiceStatusType.Started, service.Observer.Status);

            service.Dispose();
        }

        /// <summary>
        /// Tests that the initial restart a panic will do will not update if updates are set to false. This
        /// is considered a healthy instance that was stopped for a long period of time, but is being
        /// started again.
        /// </summary>
        [Test]
        public void TestStoppedHealthyServiceInPanicWillNotUpdateWithUpdatesDisabled() {
            var merged = false;

            var service = new ServiceController() {
                Packages = new MockServicePackageManager() {
                    PackageInstalled = (sender, uri, packageId) => merged = true
                },
                Settings = {
                    ServiceUpdateCore = false
                },
                ServiceLoaderProxyType = typeof(MockServiceLoaderProxy),
                Observer = {
                    Status = ServiceStatusType.None
                }
            };

            service.Panic();

            Assert.IsFalse(merged);

            service.Dispose();
        }

        /// <summary>
        /// Tests that a corrupted service in panic that does not load initially
        /// will override the disabled core updates and check for a core update.
        /// </summary>
        [Test]
        public void TestCorruptedServiceInPanicWillOverrideDisabledUpdates() {
            var merged = false;

            var service = new ServiceController() {
                Packages = new MockServicePackageManager() {
                    PackageInstalled = (sender, uri, packageId) => merged = true
                },
                Settings = {
                    ServiceUpdateCore = false
                },
                ServiceLoaderProxyType = typeof(MockNonSerializableServiceLoaderProxy),
                Observer = {
                    Status = ServiceStatusType.None
                }
            };

            service.Panic();

            Assert.IsTrue(merged);

            service.Dispose();
        }

        /// <summary>
        /// Tests that a corrupted service in panic that does not load initially
        /// will override the disabled core updates and check for a core update, which
        /// resolves a problem and successfully loads the service. This is the ideal path
        /// for a panic to resolve itself.
        /// </summary>
        [Test]
        public void TestCorruptedServiceInPanicWillOverrideDisabledUpdatesAndRecover() {
            var merged = false;

            ServiceController service = null;

            service = new ServiceController() {
                Packages = new MockServicePackageManager() {
                    PackageInstalled = (sender, uri, packageId) => {
                        merged = true;
                        // ReSharper disable AccessToModifiedClosure
                        // We only do this to keep the test concise.
                        if (service != null) {
                            service.ServiceLoaderProxyType = typeof(MockServiceLoaderProxy);
                        }
                        // ReSharper restore AccessToModifiedClosure

                    }
                },
                Settings = {
                    ServiceUpdateCore = false
                },
                ServiceLoaderProxyType = typeof(MockNonSerializableServiceLoaderProxy),
                Observer = {
                    Status = ServiceStatusType.None
                }
            };

            service.Panic();

            Assert.IsTrue(merged);
            Assert.AreEqual(ServiceStatusType.Started, service.Observer.Status);

            service.Dispose();
        }
    }
}
