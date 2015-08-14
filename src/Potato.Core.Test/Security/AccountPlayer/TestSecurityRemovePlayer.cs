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
using System;
using System.Linq;
using NUnit.Framework;
using Potato.Core.Security;
using Potato.Core.Shared;
using Potato.Core.Shared.Models;
using Potato.Net.Shared.Protocols;

namespace Potato.Core.Test.Security.AccountPlayer {
    [TestFixture]
    public class TestSecurityRemovePlayer {
        [SetUp]
        public void Initialize() {
            SharedReferences.Setup();
        }

        /// <summary>
        ///     Tests that a player can be removed if the player exists.
        /// </summary>
        [Test]
        public void TestRemoveSuccess() {
            var security = new SecurityController();
            security.Tunnel(CommandBuilder.SecurityAddGroup("GroupName").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupAddAccount("GroupName", "Phogue").SetOrigin(CommandOrigin.Local));

            // Now add a player to the "Phogue" account.
            security.Tunnel(CommandBuilder.SecurityAccountAddPlayer("Phogue", CommonProtocolType.DiceBattlefield3, "ABCDEF").SetOrigin(CommandOrigin.Local));

            // Now remove the player.
            var result = security.Tunnel(CommandBuilder.SecurityRemovePlayer(CommonProtocolType.DiceBattlefield3, "ABCDEF").SetOrigin(CommandOrigin.Local));

            // Validate the command was a success and the player is not attached to any accounts.
            // and no longer attached to the "Phogue" account.
            Assert.IsTrue(result.Success);
            Assert.AreEqual(result.CommandResultType, CommandResultType.Success);
            Assert.IsNull(security.Groups.SelectMany(group => group.Accounts).Where(account => account.Username == "Phogue").SelectMany(account => account.Players).FirstOrDefault());
        }

        /// <summary>
        ///     Tests that a player will not be removed as well as returning the valid errors if the player does not exist.
        /// </summary>
        [Test]
        public void TestDoesNotExist() {
            var security = new SecurityController();

            // Remove a player, though no accounts/players/groups exist.
            var result = security.Tunnel(CommandBuilder.SecurityRemovePlayer(CommonProtocolType.DiceBattlefield3, "ABCDEF").SetOrigin(CommandOrigin.Local));

            // Validate the command failed and returned the correct error status.
            Assert.IsFalse(result.Success);
            Assert.AreEqual(result.CommandResultType, CommandResultType.DoesNotExists);
            Assert.IsNull(security.Groups.SelectMany(group => group.Accounts).SelectMany(account => account.Players).FirstOrDefault());
        }

        /// <summary>
        ///     Tests that a player cannot be removed if it has a zero length UID.
        /// </summary>
        [Test]
        public void TestEmptyUid() {
            var security = new SecurityController();

            // Remove a player, though no accounts/players/groups exist.
            var result = security.Tunnel(CommandBuilder.SecurityRemovePlayer(CommonProtocolType.DiceBattlefield3, string.Empty).SetOrigin(CommandOrigin.Local));

            // Validate the command failed and returned the correct error status.
            Assert.IsFalse(result.Success);
            Assert.AreEqual(result.CommandResultType, CommandResultType.InvalidParameter);
            Assert.IsNull(security.Groups.SelectMany(group => group.Accounts).SelectMany(account => account.Players).FirstOrDefault());
        }

        /// <summary>
        ///     Ensures a player cannot be removed if the account does not have permission to do so.
        /// </summary>
        [Test]
        public void TestInsufficientPermission() {
            var security = new SecurityController();
            security.Tunnel(CommandBuilder.SecurityAddGroup("GroupName").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupAddAccount("GroupName", "Phogue").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityAccountAddPlayer("GroupName", CommonProtocolType.DiceBattlefield3, "ABCDEF").SetOrigin(CommandOrigin.Local));

            // Now remove the player.
            var result = security.Tunnel(CommandBuilder.SecurityRemovePlayer(CommonProtocolType.DiceBattlefield3, "ABCDEF")
                .SetOrigin(CommandOrigin.Remote)
                .SetAuthentication(new CommandAuthenticationModel() {
                    Username = "Phogue"
                })
            );

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.InsufficientPermissions, result.CommandResultType);
        }
    }
}
