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
            Assert.AreEqual("localhost", settings.PackagesDefaultSourceRepositoryUri);
        }
    }
}
