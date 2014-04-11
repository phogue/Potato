using System;
using NUnit.Framework;

namespace Procon.Core.Shared.Test.TestConfigCommand {
    [TestFixture]
    public class TestDecrypted {

        protected const String Password = "password";

        protected IConfigCommand Encrypted { get; set; }

        [SetUp]
        public void EncryptConfigCommand() {
            this.Encrypted = new ConfigCommand() {
                Command = new Command() {
                    CommandType = CommandType.ConnectionQuery
                }
            }.Encrypt(TestDecrypted.Password);
        }

        /// <summary>
        /// Tests a nulled output password will raise an exception
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestNullPasswordParameterException() {
            this.Encrypted.Decrypt(null);
        }

        /// <summary>
        /// Tests an empty password will raise an exception
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestEmptyPasswordParameterException() {
            this.Encrypted.Encrypt("");
        }

        /// <summary>
        /// Tests that decryption will fail with an invalid password
        /// </summary>
        [Test]
        public void TestDecryptionFailsWithIncorrectPassword() {
            this.Encrypted.Decrypt("password1");

            Assert.IsNull(this.Encrypted.Command);
        }
    }
}
