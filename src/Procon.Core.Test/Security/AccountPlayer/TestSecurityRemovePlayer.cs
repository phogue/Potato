using System;
using System.Linq;
using NUnit.Framework;
using Procon.Core.Security;
using Procon.Core.Shared;
using Procon.Core.Shared.Models;
using Procon.Net.Shared.Protocols;

namespace Procon.Core.Test.Security.AccountPlayer {
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
            ICommandResult result = security.Tunnel(CommandBuilder.SecurityRemovePlayer(CommonProtocolType.DiceBattlefield3, "ABCDEF").SetOrigin(CommandOrigin.Local));

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
            ICommandResult result = security.Tunnel(CommandBuilder.SecurityRemovePlayer(CommonProtocolType.DiceBattlefield3, "ABCDEF").SetOrigin(CommandOrigin.Local));

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
            ICommandResult result = security.Tunnel(CommandBuilder.SecurityRemovePlayer(CommonProtocolType.DiceBattlefield3, String.Empty).SetOrigin(CommandOrigin.Local));

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
            ICommandResult result = security.Tunnel(CommandBuilder.SecurityRemovePlayer(CommonProtocolType.DiceBattlefield3, "ABCDEF")
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
