using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Procon.Core.Protocols;
using Procon.Core.Shared;
using Procon.Core.Shared.Models;
using Procon.Net.Shared;
using Procon.Net.Shared.Protocols;

namespace Procon.Core.Test.Protocols.TestProtocolController {
    [TestFixture]
    public class TestProtocolsCheckSupportedProtocol {
        /// <summary>
        /// Tests that a remote call with no permissions will result in an InsufficientPermissions status
        /// </summary>
        [Test]
        public void TestInsufficientPermissions() {
            var protocols = new ProtocolController();

            ICommandResult result = protocols.Tunnel(CommandBuilder.ProtocolsCheckSupportedProtocol("Myrcon", CommonProtocolType.DiceBattlefield4).SetOrigin(CommandOrigin.Remote).SetAuthentication(new CommandAuthenticationModel() {
                Username = "Phogue"
            }));

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.InsufficientPermissions, result.CommandResultType);
        }

        /// <summary>
        /// Tests the command is successful and the supported protocols are returned.
        /// </summary>
        [Test]
        public void TestSuccess() {
            var item = new ProtocolType() {
                Name = "Battlefield 4",
                Provider = "Myrcon",
                Type = CommonProtocolType.DiceBattlefield4
            };

            var protocols = new ProtocolController {
                Protocols = new List<IProtocolAssemblyMetadata>() {
                    new ProtocolAssemblyMetadata() {
                        ProtocolTypes = new List<IProtocolType>() {
                            item
                        }
                    }
                }
            };

            ICommandResult result = protocols.Tunnel(CommandBuilder.ProtocolsCheckSupportedProtocol("Myrcon", CommonProtocolType.DiceBattlefield4).SetOrigin(CommandOrigin.Local));

            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Success, result.CommandResultType);
        }

        /// <summary>
        /// Tests the command is successful and the supported protocols are returned.
        /// </summary>
        [Test]
        public void TestProtocolTypesReturned() {
            var item = new ProtocolType() {
                Name = "Battlefield 4",
                Provider = "Myrcon",
                Type = CommonProtocolType.DiceBattlefield4
            };

            var protocols = new ProtocolController {
                Protocols = new List<IProtocolAssemblyMetadata>() {
                    new ProtocolAssemblyMetadata() {
                        ProtocolTypes = new List<IProtocolType>() {
                            item
                        }
                    }
                }
            };

            ICommandResult result = protocols.Tunnel(CommandBuilder.ProtocolsCheckSupportedProtocol("Myrcon", CommonProtocolType.DiceBattlefield4).SetOrigin(CommandOrigin.Local));

            Assert.AreEqual(item, result.Now.ProtocolTypes.First());
        }

        /// <summary>
        /// Tests the command fails with DoesNotExist status when the protocol is not supported.
        /// </summary>
        [Test]
        public void TestFailureDoesNotExist() {
            var protocols = new ProtocolController();

            ICommandResult result = protocols.Tunnel(CommandBuilder.ProtocolsCheckSupportedProtocol("Myrcon", CommonProtocolType.DiceBattlefield4).SetOrigin(CommandOrigin.Local));

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.DoesNotExists, result.CommandResultType);
        }
    }
}
