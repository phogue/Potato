using System.IO;
using NUnit.Framework;
using Procon.Core.Remote;
using Procon.Core.Shared.Models;
using Procon.Core.Variables;
using Procon.Service.Shared;

namespace Procon.Core.Test.Remote.TestCertificateController {
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
