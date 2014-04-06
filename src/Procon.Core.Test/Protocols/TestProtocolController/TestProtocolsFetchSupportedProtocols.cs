using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Procon.Core.Protocols;
using Procon.Core.Shared;
using Procon.Core.Shared.Models;
using Procon.Net.Shared;

namespace Procon.Core.Test.Protocols.TestProtocolController {
    [TestFixture]
    public class TestProtocolsFetchSupportedProtocols {
        /// <summary>
        /// Tests that a remote call with no permissions will result in an InsufficientPermissions status
        /// </summary>
        [Test]
        public void TestInsufficientPermissions() {
            var protocols = new ProtocolController();

            ICommandResult result = protocols.Tunnel(CommandBuilder.ProtocolsFetchSupportedProtocols().SetOrigin(CommandOrigin.Remote).SetAuthentication(new CommandAuthenticationModel() {
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
            var protocols = new ProtocolController();

            ICommandResult result = protocols.Tunnel(CommandBuilder.ProtocolsFetchSupportedProtocols().SetOrigin(CommandOrigin.Local));

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
                Type = "DiceBattlefield4"
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

            ICommandResult result = protocols.Tunnel(CommandBuilder.ProtocolsFetchSupportedProtocols().SetOrigin(CommandOrigin.Local));

            Assert.AreEqual(item, result.Now.ProtocolTypes.First());
        }
    }
}
