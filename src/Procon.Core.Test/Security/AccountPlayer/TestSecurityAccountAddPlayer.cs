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
using Procon.Core.Security;
using Procon.Core.Shared;
using Procon.Core.Shared.Models;
using Procon.Net.Shared.Protocols;

namespace Procon.Core.Test.Security.AccountPlayer {
    [TestFixture]
    public class TestSecurityAccountAddPlayer {
        [SetUp]
        public void Initialize() {
            SharedReferences.Setup();
        }

        /// <summary>
        ///     Tests that a player can be added to an account.
        /// </summary>
        [Test]
        public void TestAddSuccess() {
            var security = new SecurityController();
            security.Tunnel(CommandBuilder.SecurityAddGroup("GroupName").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupAddAccount("GroupName", "Phogue").SetOrigin(CommandOrigin.Local));

            // Now add a player to the "Phogue" account.
            ICommandResult result = security.Tunnel(CommandBuilder.SecurityAccountAddPlayer("Phogue", CommonProtocolType.DiceBattlefield3, "ABCDEF").SetOrigin(CommandOrigin.Local));

            // Validate the player was added successfully.
            Assert.IsTrue(result.Success);
            Assert.AreEqual(result.CommandResultType, CommandResultType.Success);
            Assert.AreEqual(security.Groups.SelectMany(group => group.Accounts).SelectMany(account => account.Players).First().ProtocolType, CommonProtocolType.DiceBattlefield3);
            Assert.AreEqual(security.Groups.SelectMany(group => group.Accounts).SelectMany(account => account.Players).First().Uid, "ABCDEF");
        }

        /// <summary>
        ///     Tests that a player cannot be added with a zero length UID.
        /// </summary>
        [Test]
        public void TestEmptyUidFailure() {
            var security = new SecurityController();
            security.Tunnel(CommandBuilder.SecurityAddGroup("GroupName").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupAddAccount("GroupName", "Phogue").SetOrigin(CommandOrigin.Local));

            // Now add a player to the "Phogue" account.
            ICommandResult result = security.Tunnel(CommandBuilder.SecurityAccountAddPlayer("Phogue", CommonProtocolType.DiceBattlefield3, String.Empty).SetOrigin(CommandOrigin.Local));

            // Validate the player was added successfully.
            Assert.IsFalse(result.Success);
            Assert.AreEqual(result.CommandResultType, CommandResultType.InvalidParameter);
        }

        /// <summary>
        ///     Tests that a player will be re-assigned to another account if the player already exists.
        /// </summary>
        [Test]
        public void TestExistingPlayerMoved() {
            var security = new SecurityController();
            security.Tunnel(CommandBuilder.SecurityAddGroup("GroupName").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupAddAccount("GroupName", "Phogue").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupAddAccount("GroupName", "PapaCharlie9").SetOrigin(CommandOrigin.Local));

            // Now add a player to the "Phogue" account.
            security.Tunnel(CommandBuilder.SecurityAccountAddPlayer("Phogue", CommonProtocolType.DiceBattlefield3, "ABCDEF").SetOrigin(CommandOrigin.Local));

            // Now add the same player to the "PapaCharlie9" account.
            ICommandResult result = security.Tunnel(CommandBuilder.SecurityAccountAddPlayer("PapaCharlie9", CommonProtocolType.DiceBattlefield3, "ABCDEF").SetOrigin(CommandOrigin.Local));

            // Validate the command was a success and the player is attached to the "PapaCharlie9" account
            // and no longer attached to the "Phogue" account.
            Assert.IsTrue(result.Success);
            Assert.AreEqual(result.CommandResultType, CommandResultType.Success);
            Assert.IsNull(security.Groups.SelectMany(group => group.Accounts).Where(account => account.Username == "Phogue").SelectMany(account => account.Players).FirstOrDefault());
            Assert.AreEqual(security.Groups.SelectMany(group => group.Accounts).Where(account => account.Username == "PapaCharlie9").SelectMany(account => account.Players).First().ProtocolType, CommonProtocolType.DiceBattlefield3);
            Assert.AreEqual(security.Groups.SelectMany(group => group.Accounts).Where(account => account.Username == "PapaCharlie9").SelectMany(account => account.Players).First().Uid, "ABCDEF");
        }

        /// <summary>
        ///     Tests that we cannot add a player to an account if we don't have permission to do so.
        /// </summary>
        [Test]
        public void TestInsufficientPermission() {
            var security = new SecurityController();
            security.Tunnel(CommandBuilder.SecurityAddGroup("GroupName").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupAddAccount("GroupName", "Phogue").SetOrigin(CommandOrigin.Local));

            ICommandResult result = security.Tunnel(CommandBuilder.SecurityAccountAddPlayer("PapaCharlie9", CommonProtocolType.DiceBattlefield3, "ABCDEF")
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