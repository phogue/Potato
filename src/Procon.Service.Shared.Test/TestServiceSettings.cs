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
using NUnit.Framework;

namespace Procon.Service.Shared.Test {
    [TestFixture]
    public class TestServiceSettings {
        /// <summary>
        /// Tests that the initial values are setup for service settings
        /// </summary>
        [Test]
        public void TestInitialValues() {
            var settings = new ServiceSettings();

            Assert.IsTrue(settings.ServiceUpdateCore);
            Assert.AreEqual(Defines.PackagesDefaultSourceRepositoryUri, settings.PackagesDefaultSourceRepositoryUri);
        }

        /// <summary>
        /// Tests that passing in a ServiceUpdateCore attribute will override the settings value.
        /// </summary>
        [Test]
        public void TestServiceSettingsOverrideServiceUpdateCore() {
            var settings = new ServiceSettings(new List<String>() {
                "-ServiceUpdateCore",
                "false"
            });

            Assert.IsFalse(settings.ServiceUpdateCore);
            Assert.AreEqual(Defines.DefaultServicePollingTimeout, settings.ServicePollingTimeout);
            Assert.AreEqual(Defines.PackagesDefaultSourceRepositoryUri, settings.PackagesDefaultSourceRepositoryUri);
        }

        /// <summary>
        /// Tests that passing in a ServicePollingTimeout attribute will override the settings value.
        /// </summary>
        [Test]
        public void TestServiceSettingsOverrideServicePollingTimeout() {
            var settings = new ServiceSettings(new List<String>() {
                "-ServicePollingTimeout",
                "10"
            });

            Assert.IsTrue(settings.ServiceUpdateCore);
            Assert.AreEqual(10, settings.ServicePollingTimeout);
            Assert.AreEqual(Defines.PackagesDefaultSourceRepositoryUri, settings.PackagesDefaultSourceRepositoryUri);
        }

        /// <summary>
        /// Tests that passing in a PackagesDefaultSourceRepositoryUri attribute will override the settings value.
        /// </summary>
        [Test]
        public void TestServiceSettingsOverridePackagesDefaultSourceRepositoryUri() {
            var settings = new ServiceSettings(new List<String>() {
                "-PackagesDefaultSourceRepositoryUri",
                "localhost"
            });

            Assert.IsTrue(settings.ServiceUpdateCore);
            Assert.AreEqual(Defines.DefaultServicePollingTimeout, settings.ServicePollingTimeout);
            Assert.AreEqual("localhost", settings.PackagesDefaultSourceRepositoryUri);
        }
    }
}
