using System;
using NUnit.Framework;

namespace Potato.Core.Shared.Test.TestConfigCommand {
    [TestFixture]
    public class TestEncrypt {

        protected const String Password = "password";

        /// <summary>
        /// Tests that after encrypting the data the command will be nulled out.
        /// </summary>
        [Test]
        public void TestCommandNulledAfterEncryption() {
            IConfigCommand command = new ConfigCommand() {
                Command = new Command() {
                    CommandType = CommandType.ConnectionQuery
                }
            };

            command.Encrypt(TestEncrypt.Password);

            Assert.IsNull(command.Command);
        }

        /// <summary>
        /// Tests a nulled output password will raise an exception
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestNullPasswordParameterException() {
            IConfigCommand command = new ConfigCommand() {
                Command = new Command() {
                    CommandType = CommandType.ConnectionQuery
                }
            };

            command.Encrypt(null);
        }

        /// <summary>
        /// Tests an empty password will raise an exception
        /// </summary>
        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void TestEmptyPasswordParameterException() {
            IConfigCommand command = new ConfigCommand() {
                Command = new Command() {
                    CommandType = CommandType.ConnectionQuery
                }
            };

            command.Encrypt("");
        }

        /// <summary>
        /// Simple full test showing a command can be encrypted, then decrypted when done so in memory.
        /// </summary>
        [Test]
        public void TestEncryptedCanBeDecryptedInMemory() {
            IConfigCommand command = new ConfigCommand() {
                Command = new Command() {
                    CommandType = CommandType.ConnectionQuery
                }
            };

            command.Encrypt(TestEncrypt.Password);

            Assert.IsNull(command.Command);

            command.Decrypt(TestEncrypt.Password);

            Assert.IsNotNull(command.Command);
        }

        /// <summary>
        /// Tests the integrity of the decrypted data
        /// </summary>
        [Test]
        public void TestEncryptedCanBeDecryptedInMemoryIntegrity() {
            IConfigCommand command = new ConfigCommand() {
                Command = new Command() {
                    CommandType = CommandType.ConnectionQuery
                }
            };

            command.Encrypt(TestEncrypt.Password);

            Assert.IsNull(command.Command);

            command.Decrypt(TestEncrypt.Password);

            Assert.AreEqual(CommandType.ConnectionQuery.ToString(), command.Command.Name);
        }

        /// <summary>
        /// Tests the integrity of the decrypted data
        /// </summary>
        [Test]
        public void TestEncryptedCanBeDecryptedInMemoryIntegrityWithComplexPassword() {
            const string password = "G1cK6yn3muCor0kL1g";

            IConfigCommand command = new ConfigCommand() {
                Command = new Command() {
                    CommandType = CommandType.ConnectionQuery
                }
            };

            command.Encrypt(password);

            Assert.IsNull(command.Command);

            command.Decrypt(password);

            Assert.AreEqual(CommandType.ConnectionQuery.ToString(), command.Command.Name);
        }
    }
}
