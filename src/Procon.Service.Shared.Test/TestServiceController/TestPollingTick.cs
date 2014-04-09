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
using System.Linq;
using NUnit.Framework;
using Procon.Service.Shared.Test.TestServiceController.Mocks;

namespace Procon.Service.Shared.Test.TestServiceController {
    [TestFixture]
    public class TestPollingTick {
        /// <summary>
        /// Deletes the errors logs directory if it exists.
        /// </summary>
        [SetUp]
        public void DeleteErrorsLogsDirectory() {
            Defines.ErrorsLogsDirectory.Refresh();
            if (Defines.ErrorsLogsDirectory.Exists == true) Defines.ErrorsLogsDirectory.Delete(true);
        }

        // Test returning message fails, restart

        /// <summary>
        /// Tests that a waiting message from the instance will be fetched and dispatched to the message handler.
        /// </summary>
        [Test]
        public void TestPollingRecievedMessage() {
            var signaled = false;

            var service = new ServiceController() {
                ServiceLoaderProxy = new MockServiceLoaderProxy() {
                    WaitingMessage = new ServiceMessage() {
                        Name = "help"
                    }
                },
                SignalBegin = (controller, message) => { signaled = true; },
                Settings = {
                    ServiceUpdateCore = false
                },
                Observer = {
                    Status = ServiceStatusType.Started
                },
                ServiceLoaderProxyType = typeof(MockServiceLoaderProxy)
            };

            service.Polling_Tick(null);

            Assert.IsTrue(signaled);

            service.Dispose();
        }


        /// <summary>
        /// Tests that fetching a messae from the instance will not result in a restart
        /// </summary>
        [Test]
        public void TestPollingRecievedMessageDoesNotRestart() {
            var stopped = false;
            var started = false;

            var service = new ServiceController() {
                ServiceLoaderProxy = new MockServiceLoaderProxy() {
                    WaitingMessage = new ServiceMessage() {
                        Name = "help"
                    }
                },
                Settings = {
                    ServiceUpdateCore = false
                },
                Observer = {
                    Status = ServiceStatusType.Started,
                    StatusChange = (observer, type) => {
                        if (type == ServiceStatusType.Started) started = true;
                        if (type == ServiceStatusType.Stopped) stopped = true;
                    }
                },
                ServiceLoaderProxyType = typeof(MockServiceLoaderProxy)
            };

            service.Polling_Tick(null);

            Assert.IsFalse(stopped);
            Assert.IsFalse(started);

            service.Dispose();
        }

        /// <summary>
        /// Tests the proxy isn't nagged when the service is currently stopped
        /// </summary>
        [Test]
        public void TestNotPolledWhenServiceIsStopped() {
            var signaled = false;

            var service = new ServiceController() {
                ServiceLoaderProxy = new MockServiceLoaderProxy() {
                    WaitingMessage = new ServiceMessage() {
                        Name = "help"
                    }
                },
                SignalBegin = (controller, message) => { signaled = true; },
                Settings = {
                    ServiceUpdateCore = false
                },
                Observer = {
                    Status = ServiceStatusType.Stopped
                },
                ServiceLoaderProxyType = typeof(MockServiceLoaderProxy)
            };

            service.Polling_Tick(null);

            Assert.IsFalse(signaled);

            service.Dispose();
        }

        /// <summary>
        /// Simple condition to make sure messages are not processed from an imaginary proxy.
        /// </summary>
        [Test]
        public void TestNotPolledWhenServiceProxyIsNull() {
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

            service.Polling_Tick(null);

            Assert.IsFalse(signaled);

            service.Dispose();
        }

        /// <summary>
        /// Tests an exception is logged if an exception occurs when fetching the
        /// message from the instance.
        /// </summary>
        [Test]
        public void TestExceptionLoggedWhenPollingThrowsException() {
            var service = new ServiceController() {
                ServiceLoaderProxy = new MockNonSerializableServiceLoaderProxy(),
                Settings = {
                    ServiceUpdateCore = false
                },
                Observer = {
                    Status = ServiceStatusType.Started
                },
                ServiceLoaderProxyType = typeof(MockServiceLoaderProxy)
            };

            service.Polling_Tick(null);

            Assert.IsNotEmpty(Defines.ErrorsLogsDirectory.GetFiles());
            Assert.Greater(Defines.ErrorsLogsDirectory.GetFiles().First().Length, 0);

            service.Dispose();
        }

        /// <summary>
        /// Tests the service is restarted when an exception occurs while polling for a message.
        /// </summary>
        [Test]
        public void TestServiceRestartWhenPollingThrowsException() {
            var stopped = false;
            var started = false;

            var service = new ServiceController() {
                ServiceLoaderProxy = new MockNonSerializableServiceLoaderProxy(),
                Settings = {
                    ServiceUpdateCore = false
                },
                Observer = {
                    Status = ServiceStatusType.Started,
                    StatusChange = (observer, type) => {
                        if (type == ServiceStatusType.Started) started = true;
                        if (type == ServiceStatusType.Stopped) stopped = true;
                    }
                },
                ServiceLoaderProxyType = typeof(MockServiceLoaderProxy)
            };

            service.Polling_Tick(null);

            Assert.IsTrue(stopped);
            Assert.IsTrue(started);

            service.Dispose();
        }
    }
}
