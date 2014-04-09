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
using System.Security.Cryptography.X509Certificates;
using NUnit.Framework;
using Procon.Config.Core;
using Procon.Service.Shared;

namespace Procon.Config.Test.TestConfigController {
    [TestFixture]
    public class TestCommandServerGenerateCertificate {
        /// <summary>
        /// Tests passing in nulled arguments is as good as passing in empty arguments
        /// </summary>
        [Test]
        public void TestNulledArguments() {
            var result = ConfigController.Dispatch("CommandServerGenerateCertificate", null);

            var password = result.Arguments["Password"];

            // Loads the certificates
            var loadedCertificate = new X509Certificate2(Defines.CertificatesDirectoryCommandServerPfx.FullName, password);

            // Certificate can be loaded with supplied password.
            Assert.IsNotNull(loadedCertificate);
            Assert.IsNotNull(loadedCertificate.PrivateKey);
        }

        /// <summary>
        /// Tests that pasing in empty arguments will generate a new password and the resulting certificate
        /// can be loaded with the password.
        /// </summary>
        [Test]
        public void TestEmptyArguments() {
            var result = ConfigController.Dispatch("CommandServerGenerateCertificate", new Dictionary<String, String>());

            var password = result.Arguments["Password"];

            // Loads the certificates
            var loadedCertificate = new X509Certificate2(Defines.CertificatesDirectoryCommandServerPfx.FullName, password);

            // Certificate can be loaded with supplied password.
            Assert.IsNotNull(loadedCertificate);
            Assert.IsNotNull(loadedCertificate.PrivateKey);
        }

        /// <summary>
        /// Tests that a password can be supplied and this password will be used to generate the certificate
        /// </summary>
        [Test]
        public void TestSuppliedPassword() {
            const string password = "TestSuppliedPassword";

            ConfigController.Dispatch("CommandServerGenerateCertificate", new Dictionary<String, String>() {
                { "password", password }
            });

            // Loads the certificates
            var loadedCertificate = new X509Certificate2(Defines.CertificatesDirectoryCommandServerPfx.FullName, password);

            // Certificate can be loaded with supplied password.
            Assert.IsNotNull(loadedCertificate);
            Assert.IsNotNull(loadedCertificate.PrivateKey);
        }

        /// <summary>
        /// Tests a supplied password will be returned in the message from the command.
        /// </summary>
        [Test]
        public void TestSuppliedPasswordReturnedInMessage() {
            const string password = "TestSuppliedPassword";

            var result = ConfigController.Dispatch("CommandServerGenerateCertificate", new Dictionary<String, String>() {
                { "password", password }
            });

            Assert.AreEqual(password, result.Arguments["Password"]);
        }
    }
}
