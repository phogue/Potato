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
using System.IO;
using NUnit.Framework;
using Potato.Core.Remote;
using Potato.Core.Shared.Models;
using Potato.Core.Variables;
using Potato.Service.Shared;

namespace Potato.Core.Test.Remote.TestCertificateController {
    [TestFixture]
    public class TestVariableLoad {
        /// <summary>
        /// Tests that a certificate can be loaded when it requires no password.
        /// </summary>
        [Test]
        public void TestNoPassword() {
            var variables = new VariableController();
            var certificate = new CertificateController() {
                Shared = {
                    Variables = variables
                }
            };

            variables.Variable(CommonVariableNames.CommandServerCertificatePath).Value = Path.Combine(Defines.BaseDirectory.FullName, "Remote", "Certificates", "NoPassword.pfx");

            certificate.Execute();

            Assert.IsNotNull(certificate.Certificate);
            Assert.IsNotNull(certificate.Certificate.PrivateKey);
        }

        /// <summary>
        /// Tests that a certificate can be loaded when when it requires a password.
        /// </summary>
        [Test]
        public void TestPassword() {
            var variables = new VariableController();
            var certificate = new CertificateController() {
                Shared = {
                    Variables = variables
                }
            };

            variables.Variable(CommonVariableNames.CommandServerCertificatePath).Value = Path.Combine(Defines.BaseDirectory.FullName, "Remote", "Certificates", "Password.pfx");
            variables.Variable(CommonVariableNames.CommandServerCertificatePassword).Value = "password1";

            certificate.Execute();

            Assert.IsNotNull(certificate.Certificate);
            Assert.IsNotNull(certificate.Certificate.PrivateKey);
        }
    }
}
