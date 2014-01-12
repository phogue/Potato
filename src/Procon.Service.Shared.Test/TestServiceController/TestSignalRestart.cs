using NUnit.Framework;
using Procon.Service.Shared.Test.TestServiceController.Mocks;

namespace Procon.Service.Shared.Test.TestServiceController {
    /// <summary>
    /// These tests are duplicated, but as a whole show that whatever state Procon is currently in
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
