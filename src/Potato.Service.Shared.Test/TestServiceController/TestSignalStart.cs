#region Copyright
// Copyright 2014 Myrcon Pty. Ltd.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Potato.Service.Shared.Test.TestServiceController.Mocks;

namespace Potato.Service.Shared.Test.TestServiceController {
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
