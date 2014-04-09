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
using System.Security.Cryptography.X509Certificates;
using NUnit.Framework;
using Procon.Config.Core.Models;
using Procon.Service.Shared;

namespace Procon.Config.Test.Models {
    [TestFixture]
    public class TestCertificateModel {
        /// <summary>
        /// Tests a certificate will be generated and can be read by .NET
        /// </summary>
        [Test]
        public void TestGenerate() {
            CertificateModel model = new CertificateModel();

            // Delete the certificate if it exists.
            Defines.CertificatesDirectoryCommandServerPfx.Delete();

            // Create a new certificate
            model.Generate();

            // Certificate exists
            Defines.CertificatesDirectoryCommandServerPfx.Refresh();
            Assert.IsTrue(Defines.CertificatesDirectoryCommandServerPfx.Exists);

            // Loads the certificates
            var loadedCertificate = new X509Certificate2(Defines.CertificatesDirectoryCommandServerPfx.FullName, model.Password);
            
            // Certificate can be loaded.
            Assert.IsNotNull(loadedCertificate);
            Assert.IsNotNull(loadedCertificate.PrivateKey);
        }
    }
}
