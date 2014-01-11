using System.IO;
using System.Linq;
using NUnit.Framework;
using Procon.Core.Events;
using Procon.Core.Remote;
using Procon.Core.Shared.Events;
using Procon.Service.Shared;

namespace Procon.Core.Test.Remote.TestCertificateController {
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
