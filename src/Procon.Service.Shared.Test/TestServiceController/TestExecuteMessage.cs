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
