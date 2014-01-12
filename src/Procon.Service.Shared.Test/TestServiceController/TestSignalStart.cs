using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Procon.Service.Shared.Test.TestServiceController.Mocks;

namespace Procon.Service.Shared.Test.TestServiceController {
    [TestFixture]
    public class TestSignalStart {
        /// <summary>
        /// Deletes the errors logs directory if it exists.
        /// </summary>
        [SetUp]
        public void DeleteErrorsLogsDirectory() {
            Defines.ErrorsLogsDirectory.Refresh();
            if (Defines.ErrorsLogsDirectory.Exists == true) Defines.ErrorsLogsDirectory.Delete(true);
        }

        /// <summary>
        /// Tests the app domain is created during start
        /// </summary>
        [Test]
        public void TestServiceDomainCreated() {
            var service = new ServiceController() {
                Packages = new MockServicePackageManager(),
                Settings = {
                    ServiceUpdateCore = false
                },
                ServiceLoaderProxyType = typeof(MockServiceLoaderProxy)
            };

            service.Start();

            Assert.IsNotNull(service.ServiceDomain);

            service.Dispose();
        }

        /// <summary>
        /// Tests that if the status is anything other that Stopped then nothing will
        /// occur when starting the appdomain.
        /// </summary>
        [Test]
        public void TestStartIgnoredOnNonStoppedStatus() {
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

            service.Start();

            Assert.IsNull(service.ServiceDomain);

            service.Dispose();
        }

        /// <summary>
        /// Tests the service Create method is called shortly after instantiation
        /// </summary>
        [Test]
        public void TestMethodCalledCreate() {
            var service = new ServiceController() {
                Packages = new MockServicePackageManager(),
                Settings = {
                    ServiceUpdateCore = false
                },
                ServiceLoaderProxyType = typeof(MockServiceLoaderProxy)
            };

            service.Start();

            Assert.IsTrue(((MockServiceLoaderProxy)service.ServiceLoaderProxy).OnCreate);

            service.Dispose();
        }

        /// <summary>
        /// Tests the service arguments are passed in shortly after instantiation
        /// </summary>
        [Test]
        public void TestMethodCalledArguments() {
            var service = new ServiceController() {
                Arguments = new List<String>() {
                    "A",
                    "B"
                },
                Packages = new MockServicePackageManager(),
                Settings = {
                    ServiceUpdateCore = false
                },
                ServiceLoaderProxyType = typeof(MockServiceLoaderProxy)
            };

            service.Start();

            Assert.AreEqual(service.Arguments, ((MockServiceLoaderProxy)service.ServiceLoaderProxy).OnParseCommandLineArguments);

            service.Dispose();
        }

        /// <summary>
        /// Tests the service Start method is called shortly after instantiation
        /// </summary>
        [Test]
        public void TestMethodCalledStarted() {
            var service = new ServiceController() {
                Packages = new MockServicePackageManager(),
                Settings = {
                    ServiceUpdateCore = false
                },
                ServiceLoaderProxyType = typeof(MockServiceLoaderProxy)
            };

            service.Start();

            Assert.IsTrue(((MockServiceLoaderProxy)service.ServiceLoaderProxy).OnStart);

            service.Dispose();
        }

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

            service.Start();

            Assert.AreEqual(ServiceStatusType.Started, service.Observer.Status);

            service.Dispose();
        }

        /// <summary>
        /// Tests that if an exception occurs during Start() it will be logged to file.
        /// </summary>
        [Test]
        public void TestExceptionLogged() {
            var service = new ServiceController() {
                Packages = new MockServicePackageManager(),
                Settings = {
                    ServiceUpdateCore = false
                },
                ServiceLoaderProxyType = typeof(MockNonSerializableServiceLoaderProxy)
            };

            service.Start();

            Assert.IsNotEmpty(Defines.ErrorsLogsDirectory.GetFiles());
            Assert.Greater(Defines.ErrorsLogsDirectory.GetFiles().First().Length, 0);

            service.Dispose();
        }
    }
}
