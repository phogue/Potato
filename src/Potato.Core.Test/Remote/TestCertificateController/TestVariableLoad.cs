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
using System.Linq;
using NUnit.Framework;
using Potato.Core.Events;
using Potato.Core.Remote;
using Potato.Core.Shared.Events;
using Potato.Service.Shared;

namespace Potato.Core.Test.Remote.TestCertificateController {
    [TestFixture]
    public class TestExplicitLoad {
        /// <summary>
        /// Tests that a certificate will not load if the certificate does not exist.
        /// </summary>
        [Test]
        public void TestFileDoesNotExist() {
            var certificate = new CertificateController();

            Assert.IsFalse(certificate.Load(Path.Combine(Defines.BaseDirectory.FullName, "Remote", "Certificates", "DoesNotExist.pfx")));
            Assert.IsNull(certificate.Certificate);
        }

        /// <summary>
        /// Tests that a certificate can be loaded when it requires no password.
        /// </summary>
        [Test]
        public void TestNoPassword() {
            var certificate = new CertificateController();

            Assert.IsTrue(certificate.Load(Path.Combine(Defines.BaseDirectory.FullName, "Remote", "Certificates", "NoPassword.pfx")));
            Assert.IsNotNull(certificate.Certificate.PrivateKey);
        }

        /// <summary>
        /// Tests that a certificate will not be loaded if the certificate requires no password, but a password is supplied.
        /// </summary>
        [Test]
        public void TestNoPasswordButPasswordSupplied() {
            var certificate = new CertificateController();
            
            Assert.IsFalse(certificate.Load(Path.Combine(Defines.BaseDirectory.FullName, "Remote", "Certificates", "NoPassword.pfx"), "This is a password that isn't needed"));
            Assert.IsNull(certificate.Certificate);
        }

        /// <summary>
        /// Tests that a certificate can be loaded when when it requires a password.
        /// </summary>
        [Test]
        public void TestPassword() {
            var certificate = new CertificateController();

            Assert.IsTrue(certificate.Load(Path.Combine(Defines.BaseDirectory.FullName, "Remote", "Certificates", "Password.pfx"), "password1"));
            Assert.IsNotNull(certificate.Certificate.PrivateKey);
        }

        /// <summary>
        /// Tests that a certificate will not be loaded when when it requires a password, but the password supplied is incorrect.
        /// </summary>
        [Test]
        public void TestPasswordButNoPasswordSupplied() {
            var certificate = new CertificateController();

            Assert.IsFalse(certificate.Load(Path.Combine(Defines.BaseDirectory.FullName, "Remote", "Certificates", "Password.pfx")));
            Assert.IsNull(certificate.Certificate);
        }

        /// <summary>
        /// Tests that a certificate will not be loaded when when it requires a password, but the password supplied isn't supplied.
        /// </summary>
        [Test]
        public void TestPasswordButIncorrectPasswordSupplied() {
            var certificate = new CertificateController();

            Assert.IsFalse(certificate.Load(Path.Combine(Defines.BaseDirectory.FullName, "Remote", "Certificates", "Password.pfx"), "incorrect password"));
            Assert.IsNull(certificate.Certificate);
        }

        /// <summary>
        /// Tests that an event will be logged when attempting to load a certificate that does not exist.
        /// </summary>
        [Test]
        public void TestEventLoggedOnFileDoesNotExist() {
            var events = new EventsController();
            var certificate = new CertificateController() {
                Shared = {
                    Events = events
                }
            };

            Assert.IsFalse(certificate.Load(Path.Combine(Defines.BaseDirectory.FullName, "Remote", "Certificates", "DoesNotExist.pfx")));
            Assert.IsNotEmpty(events.LoggedEvents);
            Assert.IsFalse(events.LoggedEvents.First(e => e.Name == GenericEventType.CommandServerStarted.ToString()).Success);
        }

        /// <summary>
        /// Tests that an event is logged when a password is supplied to a certificate that does not require a password.
        /// </summary>
        [Test]
        public void TestEventLoggedOnNoPasswordButPasswordSupplied() {
            var events = new EventsController();
            var certificate = new CertificateController() {
                Shared = {
                    Events = events
                }
            };

            Assert.IsFalse(certificate.Load(Path.Combine(Defines.BaseDirectory.FullName, "Remote", "Certificates", "NoPassword.pfx"), "This is a password that isn't needed"));
            Assert.IsNotEmpty(events.LoggedEvents);
            Assert.IsFalse(events.LoggedEvents.First(e => e.Name == GenericEventType.CommandServerStarted.ToString()).Success);
        }

        /// <summary>
        /// Tests that an event is logged when no password is supplied for a certificate that requires a password.
        /// </summary>
        [Test]
        public void TestEventLoggedOnPasswordButNoPasswordSupplied() {
            var events = new EventsController();
            var certificate = new CertificateController() {
                Shared = {
                    Events = events
                }
            };

            Assert.IsFalse(certificate.Load(Path.Combine(Defines.BaseDirectory.FullName, "Remote", "Certificates", "Password.pfx")));
            Assert.IsNotEmpty(events.LoggedEvents);
            Assert.IsFalse(events.LoggedEvents.First(e => e.Name == GenericEventType.CommandServerStarted.ToString()).Success);
        }

        /// <summary>
        /// Tests that an event is logged when an incorrect password is supplied for a certificate
        /// </summary>
        [Test]
        public void TestEventLoggedOnPasswordButIncorrectPasswordSupplied() {
            var events = new EventsController();
            var certificate = new CertificateController() {
                Shared = {
                    Events = events
                }
            };

            Assert.IsFalse(certificate.Load(Path.Combine(Defines.BaseDirectory.FullName, "Remote", "Certificates", "Password.pfx"), "incorrect password"));
            Assert.IsNotEmpty(events.LoggedEvents);
            Assert.IsFalse(events.LoggedEvents.First(e => e.Name == GenericEventType.CommandServerStarted.ToString()).Success);
        }
    }
}
