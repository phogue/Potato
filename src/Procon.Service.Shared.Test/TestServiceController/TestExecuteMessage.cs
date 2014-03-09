using NUnit.Framework;
using Procon.Service.Shared.Test.TestServiceController.Mocks;

namespace Procon.Service.Shared.Test.TestServiceController {
    [TestFixture]
    public class TestExecuteMessage {
        /// <summary>
        /// Tests the service controller obserable object is set to Started
        /// </summary>
        [Test]
        public void TestObservableStarted() {
            var service = new ServiceController() {
                Packages = new MockServicePackageManager(),
                Settings = {
                    ServiceUpdateCore = false
                },
                ServiceLoaderProxyType = typeof(MockServiceLoaderProxy)
            };

            service.ExecuteMessage(new ServiceMessage());

            Assert.AreEqual(ServiceStatusType.Started, service.Observer.Status);

            service.Dispose();
        }

        /// <summary>
        /// Tests the service ExecuteMessage method is called shortly after instantiation
        /// </summary>
        [Test]
        public void TestMethodCalledExecuteMessage() {
            var service = new ServiceController() {
                Packages = new MockServicePackageManager(),
                Settings = {
                    ServiceUpdateCore = false
                },
                ServiceLoaderProxyType = typeof(MockServiceLoaderProxy)
            };

            service.ExecuteMessage(new ServiceMessage());

            Assert.IsTrue(((MockServiceLoaderProxy)service.ServiceLoaderProxy).OnExecuteMessage);

            service.Dispose();
        }

        /// <summary>
        /// Tests result is processed after executing a message
        /// </summary>
        [Test]
        public void TestResultMessageProcessed() {
            var signaled = false;

            var service = new ServiceController() {
                Packages = new MockServicePackageManager(),
                Settings = {
                    ServiceUpdateCore = false
                },
                ServiceLoaderProxyType = typeof(MockServiceLoaderProxy),
                SignalResult = (controller, message) => signaled = true
            };

            service.Start();

            ((MockServiceLoaderProxy)service.ServiceLoaderProxy).ExecuteResultMessage = new ServiceMessage() {
                Name = "result"
            };

            service.ExecuteMessage(new ServiceMessage());

            Assert.IsTrue(signaled);

            service.Dispose();
        }
    }
}
