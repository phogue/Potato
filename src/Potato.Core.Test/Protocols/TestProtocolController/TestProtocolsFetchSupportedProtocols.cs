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
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Potato.Core.Protocols;
using Potato.Core.Shared;
using Potato.Core.Shared.Models;
using Potato.Net.Shared;

namespace Potato.Core.Test.Protocols.TestProtocolController {
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
