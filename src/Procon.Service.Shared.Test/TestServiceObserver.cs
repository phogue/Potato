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

namespace Procon.Service.Shared.Test {
    [TestFixture]
    public class TestServiceObserver {
        /// <summary>
        /// Tests the initial values are set when instantiating a new observer.
        /// </summary>
        [Test]
        public void TestInitalValues() {
            var observer = new ServiceObserver();

            Assert.AreEqual(ServiceStatusType.Stopped, observer.Status);

            // Test that we are "down"
            Assert.GreaterOrEqual(observer.StopTime, DateTime.Now.AddSeconds(-5));
            Assert.GreaterOrEqual(observer.Downtime(), TimeSpan.FromSeconds(0));

            // Test that we are not "up"
            Assert.IsNull(observer.StartTime);
            Assert.AreEqual(new TimeSpan(0), observer.Uptime());
        }

        /// <summary>
        /// Test that setting the status to started will show correct uptime/downtime.
        /// </summary>
        [Test]
        public void TestStatusStarted() {
            var observer = new ServiceObserver {
                Status = ServiceStatusType.Started
            };

            Assert.AreEqual(ServiceStatusType.Started, observer.Status);

            // Test that we are not "down"
            Assert.IsNotNull(observer.StopTime);
            Assert.AreEqual(new TimeSpan(0), observer.Downtime());

            // Test that we are "up"
            Assert.GreaterOrEqual(observer.StartTime, DateTime.Now.AddSeconds(-5));
            Assert.GreaterOrEqual(observer.Uptime(), TimeSpan.FromSeconds(0));
        }

        /// <summary>
        /// Tests that if the downtime has not exceeded fifteen minutes the panic callback will not be called.
        /// </summary>
        [Test]
        public void TestPanicNotInitiatedUnderFifteenMinutes() {
            var paniced = false;
            var observer = new ServiceObserver() {
                Panic = () => { paniced = true; }
            };

            // We've only been down for less than a second so it shouldn't fire.
            observer.PanicTask_Tick(null);

            Assert.IsFalse(paniced);
        }

        /// <summary>
        /// Tests that if the downtime has exceeded fifteen minutes the panic callback will be called.
        /// </summary>
        [Test]
        public void TestPanicDowntimeOverFifteenMinutes() {
            var paniced = false;
            var observer = new ServiceObserver() {
                Panic = () => { paniced = true; },
                StopTime = DateTime.Now.AddMinutes(-16)
            };

            // We've only been down for less than a second so it shouldn't fire.
            observer.PanicTask_Tick(null);

            Assert.IsTrue(paniced);
        }

        /// <summary>
        /// Test that setting the status to started will show correct uptime/downtime.
        /// </summary>
        [Test]
        public void TestStatusModifiedCallback() {
            var modified = false;

            var observer = new ServiceObserver {
                StatusChange = (sender, type) => { modified = true; },
                Status = ServiceStatusType.Started
            };

            Assert.IsTrue(modified);
            Assert.AreEqual(ServiceStatusType.Started, observer.Status);
        }

        /// <summary>
        /// Test that the callback will not be called when the status is unmodified
        /// </summary>
        [Test]
        public void TestStatusUnmodifiedNoCallback() {
            var modified = false;

            var observer = new ServiceObserver {
                StatusChange = (sender, type) => { modified = true; },
                Status = ServiceStatusType.Stopped
            };

            Assert.IsFalse(modified);
            Assert.AreEqual(ServiceStatusType.Stopped, observer.Status);
        }
    }
}
