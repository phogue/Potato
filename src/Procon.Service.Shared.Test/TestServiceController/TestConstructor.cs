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
using NUnit.Framework;

namespace Procon.Service.Shared.Test.TestServiceController {
    [TestFixture]
    public class TestConstructor {
        /// <summary>
        /// Tests that when creating a new service controllerit will enable the appdomain monitoring
        /// </summary>
        [Test]
        public void TestMonitoringIsEnabled() {
            var service = new ServiceController();

            Assert.IsTrue(AppDomain.MonitoringIsEnabled);

            service.Dispose();
        }

        /// <summary>
        /// Tests the constructor initalizes the public properties of the controller
        /// </summary>
        [Test]
        public void TestNonNullProperties() {
            var service = new ServiceController();

            Assert.IsNotNull(service.Observer);
            Assert.IsNotNull(service.Polling);
            Assert.IsNotNull(service.WriteConfig);
            Assert.IsNotNull(service.Arguments);
            Assert.IsNotNull(service.Settings);
            Assert.IsNotNull(service.Packages);

            service.Dispose();
        }

        /// <summary>
        /// Ensures the initial service loader proxy type 
        /// </summary>
        [Test]
        public void TestServiceProxyType() {
            var service = new ServiceController();

            Assert.AreEqual(typeof(ServiceLoaderProxy), service.ServiceLoaderProxyType);

            service.Dispose();
        }

        /// <summary>
        /// Ensures the initial service state is stopped
        /// </summary>
        [Test]
        public void TestInitalServiceStateIsStopped() {
            var service = new ServiceController();

            Assert.AreEqual(ServiceStatusType.Stopped, service.Observer.Status);

            service.Dispose();
        }
    }
}
