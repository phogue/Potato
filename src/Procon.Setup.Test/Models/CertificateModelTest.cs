using System.IO;
using System.Security.Cryptography.X509Certificates;
using NUnit.Framework;
using Procon.Service.Shared;
using Procon.Setup.Models;

namespace Procon.Setup.Test.Models {
    [TestFixture]
    public class CertificateModelTest {
        /// <summary>
        /// Tests a certificate will be generated and can be read by .NET
        /// </summary>
        [Test]
        public void TestGenerate() {
            CertificateModel model = new CertificateModel();

            // Delete the certificate if it exists.
            File.Delete(Defines.CertificatesDirectoryCommandServerPfx);

            // Create a new certificate
            model.Generate();

            // Certificate exists
            Assert.IsTrue(File.Exists(Defines.CertificatesDirectoryCommandServerPfx));

            // Loads the certificates
            var loadedCertificate = new X509Certificate2(Defines.CertificatesDirectoryCommandServerPfx, model.Password);
            
            // Certificate can be loaded.
            Assert.IsNotNull(loadedCertificate);
            Assert.IsNotNull(loadedCertificate.PrivateKey);
        }
    }
}
