using System;
using NUnit.Framework;

namespace Potato.Core.Shared.Test.TestConfigCommand {
    [TestFixture]
    public class TestDecrypted {

        protected const string Password = "password";

        protected IConfigCommand Encrypted { get; set; }

        [SetUp]
        public void EncryptConfigCommand() {
            Encrypted = new ConfigCommand() {
                Command = new Command() {
                    CommandType = CommandType.ConnectionQuery
                }
            }.Encrypt(Password);
        }

        /// <summary>
        /// Tests a nulled output password will raise an exception
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestNullPasswordParameterException() {
            Encrypted.Decrypt(null);
        }

        /// <summary>
        /// Tests an empty password will raise an exception
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestEmptyPasswordParameterException() {
            Encrypted.Encrypt("");
        }

        /// <summary>
        /// Tests that decryption will fail with an invalid password
        /// </summary>
        [Test]
        public void TestDecryptionFailsWithIncorrectPassword() {
            Encrypted.Decrypt("password1");

            Assert.IsNull(Encrypted.Command);
        }
    }
}
