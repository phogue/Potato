#region Copyright
// Copyright 2015 Geoff Green.
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
using Potato.Service.Shared.Test.TestServiceController.Mocks;

namespace Potato.Service.Shared.Test.TestServiceController {
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
