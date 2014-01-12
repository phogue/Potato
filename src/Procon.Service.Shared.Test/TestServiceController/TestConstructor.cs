using System;
using NUnit.Framework;

namespace Procon.Service.Shared.Test.TestServiceController {
    [TestFixture]
    public class TestConstructor {
        /// <summary>
        /// Tests that when creating a new service controllerit will enable the appdomain monitoring
        /// </summary>
        [Test]
        public void TestMonitoringIsEnabled() {
            var service = new ServiceController();

            Assert.IsTrue(AppDomain.MonitoringIsEnabled);

            service.Dispose();
        }

        /// <summary>
        /// Tests the constructor initalizes the public properties of the controller
        /// </summary>
        [Test]
        public void TestNonNullProperties() {
            var service = new ServiceController();

            Assert.IsNotNull(service.Observer);
            Assert.IsNotNull(service.Polling);
            Assert.IsNotNull(service.Arguments);
            Assert.IsNotNull(service.Settings);
            Assert.IsNotNull(service.Packages);

            service.Dispose();
        }

        /// <summary>
        /// Ensures the initial service loader proxy type 
        /// </summary>
        [Test]
        public void TestServiceProxyType() {
            var service = new ServiceController();

            Assert.AreEqual(typeof(ServiceLoaderProxy), service.ServiceLoaderProxyType);

            service.Dispose();
        }

        /// <summary>
        /// Ensures the initial service state is stopped
        /// </summary>
        [Test]
        public void TestInitalServiceStateIsStopped() {
            var service = new ServiceController();

            Assert.AreEqual(ServiceStatusType.Stopped, service.Observer.Status);

            service.Dispose();
        }
    }
}
