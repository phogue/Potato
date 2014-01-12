using System.Linq;
using NUnit.Framework;
using Procon.Service.Shared.Test.TestServiceController.Mocks;

namespace Procon.Service.Shared.Test.TestServiceController {
    [TestFixture]
    public class TestDisposeService {
        /// <summary>
        /// Deletes the errors logs directory if it exists.
        /// </summary>
        [SetUp]
        public void DeleteErrorsLogsDirectory() {
            Defines.ErrorsLogsDirectory.Refresh();
            if (Defines.ErrorsLogsDirectory.Exists == true) Defines.ErrorsLogsDirectory.Delete(true);
        }

        /// <summary>
        /// Tests the writeconfig method is called on the service loader proxy
        /// </summary>
        [Test]
        public void TestDisposeServiceDispatchedSuccess() {
            var disposed = false;

            var service = new ServiceController() {
                ServiceLoaderProxy = new MockServiceLoaderProxy() {
                    OnDisposeHandler = () => disposed = true
                }
            };

            service.DisposeService();

            Assert.IsTrue(disposed);

            service.Dispose();
        }

        /// <summary>
        /// Tests the loader proxy is nulled after disposed
        /// </summary>
        [Test]
        public void TestServiceLoaderProxyNulled() {
            var service = new ServiceController() {
                ServiceLoaderProxy = new MockServiceLoaderProxy()
            };

            service.DisposeService();

            Assert.IsNull(service.ServiceLoaderProxy);

            service.Dispose();
        }

        /// <summary>
        /// Tests that the WriteServiceConfigBegin delegate is called prior to
        /// writing the config.
        /// </summary>
        [Test]
        public void TestDisposeServiceDispatchedBeginDelegateCalled() {
            var begin = false;

            var service = new ServiceController() {
                ServiceLoaderProxy = new MockServiceLoaderProxy(),
                DisposeServiceBegin = controller => begin = true
            };

            service.DisposeService();

            Assert.IsTrue(begin);

            service.Dispose();
        }

        /// <summary>
        /// Tests that the WriteServiceConfigBegin delegate is called after
        /// writing the config.
        /// </summary>
        [Test]
        public void TestDisposeServiceDispatchedEndDelegateCalled() {
            var end = false;

            var service = new ServiceController() {
                ServiceLoaderProxy = new MockServiceLoaderProxy(),
                DisposeServiceEnd = controller => end = true
            };

            service.DisposeService();

            Assert.IsTrue(end);

            service.Dispose();
        }

        /// <summary>
        /// Tests that exception that occur during config write will be captured and logged.
        /// </summary>
        [Test]
        public void TestExceptionLogged() {
            var service = new ServiceController() {
                ServiceLoaderProxy = new MockNonSerializableServiceLoaderProxy()
            };

            service.DisposeService();

            Assert.IsNotEmpty(Defines.ErrorsLogsDirectory.GetFiles());
            Assert.Greater(Defines.ErrorsLogsDirectory.GetFiles().First().Length, 0);

            service.Dispose();
        }
    }
}
