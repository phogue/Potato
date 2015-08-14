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
using System;
using System.Linq;
using NUnit.Framework;
using Potato.Service.Shared.Test.TestServiceController.Mocks;

namespace Potato.Service.Shared.Test.TestServiceController {
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
                ServiceDomain = AppDomain.CreateDomain("Potato.Instance"),
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
                ServiceDomain = AppDomain.CreateDomain("Potato.Instance"),
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
                ServiceDomain = AppDomain.CreateDomain("Potato.Instance"),
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
                ServiceDomain = AppDomain.CreateDomain("Potato.Instance"),
                UnloadServiceBegin = controller => {
                    throw new Exception("Empty");
                },
                Process = new MockProcess()
            };

            service.UnloadService();
            
            Assert.IsNotEmpty(Defines.ErrorsLogsDirectory.GetFiles());
            Assert.Greater(Defines.ErrorsLogsDirectory.GetFiles().First().Length, 0);

            service.Dispose();
        }

        /// <summary>
        /// Tests that exception that occur during config write will be captured and logged.
        /// </summary>
        [Test]
        public void TestExceptionKillsProcess() {
            var process = new MockProcess();

            var service = new ServiceController() {
                ServiceDomain = AppDomain.CreateDomain("Potato.Instance"),
                UnloadServiceBegin = controller => {
                    throw new Exception("Empty");
                },
                Process = process
            };

            service.UnloadService();

            Assert.IsTrue(process.OnKill);

            service.Dispose();
        }
    }
}
