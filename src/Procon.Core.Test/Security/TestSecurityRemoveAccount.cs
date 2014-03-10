using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Procon.Core.Security;
using Procon.Core.Shared;
using Procon.Core.Shared.Models;

namespace Procon.Core.Test.Security {
    [TestFixture]
    public class TestSecurityRemoveAccount {

        /// <summary>
        ///     Testing that an account can be removed by its name.
        /// </summary>
        [Test]
        public void TestSecurityRemoveAccountModel() {
            var security = new SecurityController();
            security.Tunnel(CommandBuilder.SecurityAddGroup("GroupName").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupAddAccount("GroupName", "Phogue").SetOrigin(CommandOrigin.Local));

            // Test that the group was initially added.
            Assert.AreEqual(security.Groups.Last().Accounts.First().Username, "Phogue");

            ICommandResult result = security.Tunnel(CommandBuilder.SecurityRemoveAccount("Phogue").SetOrigin(CommandOrigin.Local));

            // Make sure it was successful.
            Assert.IsTrue(result.Success);
            Assert.AreEqual(0, security.Groups.Last().Accounts.Count);
        }

        /// <summary>
        /// Tests that an account can be removed if the command is locally called
        /// </summary>
        [Test]
        public void TestRemoveAccountByLocalSuccess() {
            var security = new SecurityController();
            security.Tunnel(CommandBuilder.SecurityAddGroup("GroupName").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupAddAccount("GroupName", "Phogue").SetOrigin(CommandOrigin.Local));

            ICommandResult result = security.Tunnel(CommandBuilder.SecurityRemoveAccount("Phogue").SetOrigin(CommandOrigin.Local));

            // Make sure the command failed. The user cannot remove their own account.
            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Success, result.Status);
        }

        /// <summary>
        /// Tests that an account will not be removed if the owner is attempting to remove it.
        /// </summary>
        [Test]
        public void TestRemoveOwnAccountFailure() {
            var security = new SecurityController();
            security.Tunnel(CommandBuilder.SecurityAddGroup("GroupName").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupAddAccount("GroupName", "Phogue").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupSetPermission("GroupName", CommandType.SecurityRemoveAccount, 1).SetOrigin(CommandOrigin.Local));

            ICommandResult result = security.Tunnel(CommandBuilder.SecurityRemoveAccount("Phogue").SetOrigin(CommandOrigin.Remote).SetAuthentication(new CommandAuthenticationModel() {
                Username = "Phogue"
            }));

            // Make sure the command failed. The user cannot remove their own account.
            Assert.IsFalse(result.Success);
            Assert.AreEqual(result.Status, CommandResultType.InvalidParameter);
        }

        /// <summary>
        ///     Testing that you can't remove a group name that is empty.
        /// </summary>
        [Test]
        public void TestSecurityRemoveAccountEmptyAccountUsername() {
            var security = new SecurityController();

            // Add a group with an empty name.
            ICommandResult result = security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityRemoveAccount,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    String.Empty
                })
            });

            // Make sure adding an empty group fails.
            Assert.IsFalse(result.Success);
            Assert.AreEqual(result.Status, CommandResultType.InvalidParameter);
        }

        /// <summary>
        ///     Tests the command to add a player to an account fails if the user has insufficient privileges.
        /// </summary>
        [Test]
        public void TestSecurityRemoveAccountInsufficientPermission() {
            var security = new SecurityController();
            security.Tunnel(CommandBuilder.SecurityAddGroup("GroupName").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupAddAccount("GroupName", "Phogue").SetOrigin(CommandOrigin.Local));

            // Test that the group was initially added.
            Assert.AreEqual(security.Groups.Last().Accounts.First().Username, "Phogue");

            ICommandResult result = security.Tunnel(CommandBuilder.SecurityRemoveAccount("Phogue").SetOrigin(CommandOrigin.Remote).SetAuthentication(new CommandAuthenticationModel() {
                Username = "Phogue"
            }));

            // Make sure it was successful.
            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.InsufficientPermissions, result.Status);
        }

        /// <summary>
        ///     Testing that you can't remove a group that does not exist and provides correct errors.
        /// </summary>
        [Test]
        public void TestSecurityRemoveAccountNotExists() {
            var security = new SecurityController();
            ICommandResult result = security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityRemoveAccount,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "Phogue"
                })
            });

            // Make sure it was not successful.
            Assert.IsFalse(result.Success);
            Assert.AreEqual(result.Status, CommandResultType.DoesNotExists);
        }
    }
}
