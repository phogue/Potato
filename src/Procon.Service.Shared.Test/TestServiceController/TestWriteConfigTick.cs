using NUnit.Framework;
using Procon.Service.Shared.Test.TestServiceController.Mocks;

namespace Procon.Service.Shared.Test.TestServiceController {
    [TestFixture]
    public class TestWriteConfigTick {
        /// <summary>
        /// Deletes the errors logs directory if it exists.
        /// </summary>
        [SetUp]
        public void DeleteErrorsLogsDirectory() {
            Defines.ErrorsLogsDirectory.Refresh();
            if (Defines.ErrorsLogsDirectory.Exists == true) Defines.ErrorsLogsDirectory.Delete(true);
        }

        /// <summary>
        /// Tests that the write config message is dispatched from the write config tick when
        /// the service is started
        /// </summary>
        [Test]
        public void TestWriteConfigTickPassedTroughToMessage() {
            var signaled = false;

            var service = new ServiceController() {
                ServiceLoaderProxy = new MockServiceLoaderProxy(),
                WriteServiceConfigBegin = (controller) => { signaled = true; },
                Settings = {
                    ServiceUpdateCore = false
                },
                Observer = {
                    Status = ServiceStatusType.Started
                },
                ServiceLoaderProxyType = typeof(MockServiceLoaderProxy)
            };

            service.WriteConfig_Tick(null);

            Assert.IsTrue(signaled);

            service.Dispose();
        }

        /// <summary>
        /// Tests the proxy isn't nagged when the service is currently stopped
        /// </summary>
        [Test]
        public void TestNotWrittenWhenServiceIsStopped() {
            var signaled = false;

            var service = new ServiceController() {
                ServiceLoaderProxy = new MockServiceLoaderProxy(),
                SignalBegin = (controller, message) => { signaled = true; },
                Settings = {
                    ServiceUpdateCore = false
                },
                Observer = {
                    Status = ServiceStatusType.Stopped
                },
                ServiceLoaderProxyType = typeof(MockServiceLoaderProxy)
            };

            service.WriteConfig_Tick(null);

            Assert.IsFalse(signaled);

            service.Dispose();
        }

        /// <summary>
        /// Simple condition to make sure messages are not processed from an imaginary proxy.
        /// </summary>
        [Test]
        public void TestNotWrittenWhenServiceProxyIsNull() {
            var signaled = false;

            var service = new ServiceController() {
                SignalBegin = (controller, message) => { signaled = true; },
                Settings = {
                    ServiceUpdateCore = false
                },
                Observer = {
                    Status = ServiceStatusType.Started
                },
                ServiceLoaderProxyType = typeof(MockServiceLoaderProxy)
            };

            service.WriteConfig_Tick(null);

            Assert.IsFalse(signaled);

            service.Dispose();
        }
    }
}
