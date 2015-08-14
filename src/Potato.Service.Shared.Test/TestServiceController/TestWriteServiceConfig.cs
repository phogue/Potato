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
using System.Linq;
using NUnit.Framework;
using Potato.Service.Shared.Test.TestServiceController.Mocks;

namespace Potato.Service.Shared.Test.TestServiceController {
    [TestFixture]
    public class TestWriteServiceConfig {
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
        public void TestConfigWriteDispatchedSuccess() {
            var service = new ServiceController() {
                ServiceLoaderProxy = new MockServiceLoaderProxy()
            };

            service.WriteServiceConfig();

            Assert.IsTrue(((MockServiceLoaderProxy)service.ServiceLoaderProxy).OnWriteConfig);

            service.Dispose();
        }

        /// <summary>
        /// Tests that the WriteServiceConfigBegin delegate is called prior to
        /// writing the config.
        /// </summary>
        [Test]
        public void TestConfigWriteDispatchedBeginDelegateCalled() {
            var begin = false;

            var service = new ServiceController() {
                ServiceLoaderProxy = new MockServiceLoaderProxy(),
                WriteServiceConfigBegin = controller => begin = true
            };

            service.WriteServiceConfig();

            Assert.IsTrue(begin);

            service.Dispose();
        }

        /// <summary>
        /// Tests that the WriteServiceConfigBegin delegate is called after
        /// writing the config.
        /// </summary>
        [Test]
        public void TestConfigWriteDispatchedEndDelegateCalled() {
            var end = false;

            var service = new ServiceController() {
                ServiceLoaderProxy = new MockServiceLoaderProxy(),
                WriteServiceConfigEnd = controller => end = true
            };

            service.WriteServiceConfig();

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

            service.WriteServiceConfig();

            Assert.IsNotEmpty(Defines.ErrorsLogsDirectory.GetFiles());
            Assert.Greater(Defines.ErrorsLogsDirectory.GetFiles().First().Length, 0);

            service.Dispose();
        }

        /// <summary>
        /// Tests that an exception will be logged when the maxmimum number of milliseconds 
        /// for the task has exceeded.
        /// </summary>
        [Test]
        public void TestExceptionLoggedOnTimeout() {
            var service = new ServiceController() {
                ServiceLoaderProxy = new MockSlowServiceLoaderProxy() {
                    WriteConfigSleep = 500
                },
                Settings = {
                    WriteServiceConfigTimeout = 10
                }
            };

            service.WriteServiceConfig();

            Assert.IsNotEmpty(Defines.ErrorsLogsDirectory.GetFiles());
            Assert.Greater(Defines.ErrorsLogsDirectory.GetFiles().First().Length, 0);

            service.Dispose();
        }
    }
}
