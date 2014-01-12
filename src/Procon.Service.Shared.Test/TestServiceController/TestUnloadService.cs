using System;
using System.Linq;
using NUnit.Framework;
using Procon.Service.Shared.Test.TestServiceController.Mocks;

namespace Procon.Service.Shared.Test.TestServiceController {
    [TestFixture]
    public class TestUnloadService {
        /// <summary>
        /// Deletes the errors logs directory if it exists.
        /// </summary>
        [SetUp]
        public void DeleteErrorsLogsDirectory() {
            Defines.ErrorsLogsDirectory.Refresh();
            if (Defines.ErrorsLogsDirectory.Exists == true) Defines.ErrorsLogsDirectory.Delete(true);
        }

        /// <summary>
        /// Tests the loader proxy is nulled after unloading
        /// </summary>
        [Test]
        public void TestServiceDomainNulled() {
            var service = new ServiceController() {
                ServiceDomain = AppDomain.CreateDomain("Procon.Instance"),
            };

            service.UnloadService();

            Assert.IsNull(service.ServiceDomain);

            service.Dispose();
        }

        /// <summary>
        /// Tests that the WriteServiceConfigBegin delegate is called prior to
        /// writing the config.
        /// </summary>
        [Test]
        public void TestUnloadServiceDispatchedBeginDelegateCalled() {
            var begin = false;

            var service = new ServiceController() {
                ServiceDomain = AppDomain.CreateDomain("Procon.Instance"),
                UnloadServiceBegin = controller => begin = true
            };

            service.UnloadService();

            Assert.IsTrue(begin);

            service.Dispose();
        }

        /// <summary>
        /// Tests that the WriteServiceConfigBegin delegate is called after
        /// writing the config.
        /// </summary>
        [Test]
        public void TestUnloadServiceDispatchedEndDelegateCalled() {
            var end = false;

            var service = new ServiceController() {
                ServiceDomain = AppDomain.CreateDomain("Procon.Instance"),
                UnloadServiceEnd = controller => end = true
            };

            service.UnloadService();

            Assert.IsTrue(end);

            service.Dispose();
        }

        /// <summary>
        /// Tests that exception that occur during config write will be captured and logged.
        /// </summary>
        [Test]
        public void TestExceptionLogged() {
            var service = new ServiceController() {
                ServiceDomain = AppDomain.CreateDomain("Procon.Instance"),
                UnloadServiceBegin = controller => {
                    throw new Exception("Empty");
                }
            };

            service.UnloadService();
            
            Assert.IsNotEmpty(Defines.ErrorsLogsDirectory.GetFiles());
            Assert.Greater(Defines.ErrorsLogsDirectory.GetFiles().First().Length, 0);

            service.Dispose();
        }
    }
}
