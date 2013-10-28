using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Procon.Core.Test.Security {
    using Procon.Core.Security;
    using Procon.Net.Protocols;

    [TestClass]
    public class TestSecurityPlayer {

        #region Adding Players

        /// <summary>
        /// Tests that a player can be added to an account.
        /// </summary>
        [TestMethod]
        public void TestSecurityAccountAddPlayer() {
            SecurityController security = new SecurityController();
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAddGroup, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "GroupName" }) });

            // Test that the group was initially added.
            Assert.AreEqual<String>(security.Groups.First().Name, "GroupName");

            // Now add the user.
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityGroupAddAccount, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "GroupName", "Phogue" }) });

            // Test that the account was added to the group.
            Assert.AreEqual<String>(security.Groups.SelectMany(group => group.Accounts).First().Username, "Phogue");

            // Now add a player to the "Phogue" account.
            CommandResultArgs result = security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAccountAddPlayer, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "Phogue", CommonGameType.BF_3, "ABCDEF" }) });

            // Validate the player was added successfully.
            Assert.IsTrue(result.Success);
            Assert.AreEqual<CommandResultType>(result.Status, CommandResultType.Success);
            Assert.AreEqual<String>(security.Groups.SelectMany(group => group.Accounts).SelectMany(account => account.Players).First().GameType, CommonGameType.BF_3);
            Assert.AreEqual<String>(security.Groups.SelectMany(group => group.Accounts).SelectMany(account => account.Players).First().Uid, "ABCDEF");
        }

        /// <summary>
        /// Tests that we cannot add a player to an account if we don't have permission to do so.
        /// </summary>
        [TestMethod]
        public void TestSecurityAccountAddPlayerInsufficientPermission() {
            SecurityController security = new SecurityController();
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAddGroup, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "GroupName" }) });
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityGroupAddAccount, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "GroupName", "Phogue" }) });

            CommandResultArgs result = security.Execute(new Command() {
                CommandType = CommandType.SecurityAccountAddPlayer,
                Username = "Phogue",
                Origin = CommandOrigin.Remote,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "Phogue",
                    CommonGameType.BF_3,
                    "ABCDEF"
                })
            });

            Assert.IsFalse(result.Success);
            Assert.AreEqual<CommandResultType>(CommandResultType.InsufficientPermissions, result.Status);
        }

        /// <summary>
        /// Tests that a player will be re-assigned to another account if the player already exists.
        /// </summary>
        [TestMethod]
        public void TestSecurityAccountAddPlayerExistingPlayer() {
            SecurityController security = new SecurityController();
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAddGroup, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "GroupName" }) });

            // Test that the group was initially added.
            Assert.AreEqual<String>(security.Groups.First().Name, "GroupName");

            // Now add the user.
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityGroupAddAccount, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "GroupName", "Phogue" }) });
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityGroupAddAccount, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "GroupName", "PapaCharlie9" }) });

            // Test that the account was added to the group.
            Assert.IsNotNull(security.Groups.SelectMany(group => group.Accounts).Where(account => account.Username == "Phogue").FirstOrDefault());
            Assert.IsNotNull(security.Groups.SelectMany(group => group.Accounts).Where(account => account.Username == "PapaCharlie9").FirstOrDefault());

            // Now add a player to the "Phogue" account.
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAccountAddPlayer, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "Phogue", CommonGameType.BF_3, "ABCDEF" }) });


            // Validate the player was added successfully to the Phogue account.
            // and the PapaCharlie9 account still has no players.
            Assert.AreEqual<String>(security.Groups.SelectMany(group => group.Accounts).Where(account => account.Username == "Phogue").SelectMany(account => account.Players).First().GameType, CommonGameType.BF_3);
            Assert.AreEqual<String>(security.Groups.SelectMany(group => group.Accounts).Where(account => account.Username == "Phogue").SelectMany(account => account.Players).First().Uid, "ABCDEF");
            Assert.IsNull(security.Groups.SelectMany(group => group.Accounts).Where(account => account.Username == "PapaCharlie9").SelectMany(account => account.Players).FirstOrDefault());

            // Now add a player to the "PapaCharlie9" account.
            CommandResultArgs result = security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAccountAddPlayer, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "PapaCharlie9", CommonGameType.BF_3, "ABCDEF" }) });

            // Validate the command was a success and the player is attached to the "PapaCharlie9" account
            // and no longer attached to the "Phogue" account.
            Assert.IsTrue(result.Success);
            Assert.AreEqual<CommandResultType>(result.Status, CommandResultType.Success);
            Assert.IsNull(security.Groups.SelectMany(group => group.Accounts).Where(account => account.Username == "Phogue").SelectMany(account => account.Players).FirstOrDefault());
            Assert.AreEqual<String>(security.Groups.SelectMany(group => group.Accounts).Where(account => account.Username == "PapaCharlie9").SelectMany(account => account.Players).First().GameType, CommonGameType.BF_3);
            Assert.AreEqual<String>(security.Groups.SelectMany(group => group.Accounts).Where(account => account.Username == "PapaCharlie9").SelectMany(account => account.Players).First().Uid, "ABCDEF");
        }

        /// <summary>
        /// Tests that a player cannot be added with a zero length UID.
        /// </summary>
        [TestMethod]
        public void TestSecurityAccountAddPlayerEmptyUID() {
            SecurityController security = new SecurityController();
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAddGroup, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "GroupName" }) });

            // Test that the group was initially added.
            Assert.AreEqual<String>(security.Groups.First().Name, "GroupName");

            // Now add the user.
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityGroupAddAccount, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "GroupName", "Phogue" }) });

            // Test that the account was added to the group.
            Assert.AreEqual<String>(security.Groups.SelectMany(group => group.Accounts).First().Username, "Phogue");

            // Now add a player to the "Phogue" account.
            CommandResultArgs result = security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAccountAddPlayer, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "Phogue", CommonGameType.BF_3, String.Empty }) });

            // Validate the player was added successfully.
            Assert.IsFalse(result.Success);
            Assert.AreEqual<CommandResultType>(result.Status, CommandResultType.InvalidParameter);
        }

        #endregion

        #region Removing Players

        /// <summary>
        /// Tests that a player can be removed if the player exists.
        /// </summary>
        [TestMethod]
        public void TestSecurityRemovePlayer() {
            SecurityController security = new SecurityController();
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAddGroup, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "GroupName" }) });

            // Test that the group was initially added.
            Assert.AreEqual<String>(security.Groups.First().Name, "GroupName");

            // Now add the user.
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityGroupAddAccount, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "GroupName", "Phogue" }) });

            // Test that the account was added to the group.
            Assert.IsNotNull(security.Groups.SelectMany(group => group.Accounts).Where(account => account.Username == "Phogue").FirstOrDefault());

            // Now add a player to the "Phogue" account.
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAccountAddPlayer, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "Phogue", CommonGameType.BF_3, "ABCDEF" }) });

            // Validate the player was added successfully to the Phogue account.
            // and the PapaCharlie9 account still has no players.
            Assert.AreEqual<String>(security.Groups.SelectMany(group => group.Accounts).Where(account => account.Username == "Phogue").SelectMany(account => account.Players).First().GameType, CommonGameType.BF_3);
            Assert.AreEqual<String>(security.Groups.SelectMany(group => group.Accounts).Where(account => account.Username == "Phogue").SelectMany(account => account.Players).First().Uid, "ABCDEF");

            // Now remove the player.
            CommandResultArgs result = security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityRemovePlayer, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { CommonGameType.BF_3, "ABCDEF" }) });

            // Validate the command was a success and the player is not attached to any accounts.
            // and no longer attached to the "Phogue" account.
            Assert.IsTrue(result.Success);
            Assert.AreEqual<CommandResultType>(result.Status, CommandResultType.Success);
            Assert.IsNull(security.Groups.SelectMany(group => group.Accounts).Where(account => account.Username == "Phogue").SelectMany(account => account.Players).FirstOrDefault());
        }

        /// <summary>
        /// Tests that a player will not be removed as well as returning the valid errors if the player does not exist.
        /// </summary>
        [TestMethod]
        public void TestSecurityRemovePlayerDoesNotExist() {
            SecurityController security = new SecurityController();

            // Remove a player, though no accounts/players/groups exist.
            CommandResultArgs result = security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityRemovePlayer, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { CommonGameType.BF_3, "ABCDEF" }) });

            // Validate the command failed and returned the correct error status.
            Assert.IsFalse(result.Success);
            Assert.AreEqual<CommandResultType>(result.Status, CommandResultType.DoesNotExists);
            Assert.IsNull(security.Groups.SelectMany(group => group.Accounts).SelectMany(account => account.Players).FirstOrDefault());
        }

        /// <summary>
        /// Tests that a player cannot be removed if it has a zero length UID.
        /// </summary>
        [TestMethod]
        public void TestSecurityRemovePlayerEmptyUid() {
            SecurityController security = new SecurityController();

            // Remove a player, though no accounts/players/groups exist.
            CommandResultArgs result = security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityRemovePlayer, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { CommonGameType.BF_3, String.Empty }) });
 
            // Validate the command failed and returned the correct error status.
            Assert.IsFalse(result.Success);
            Assert.AreEqual<CommandResultType>(result.Status, CommandResultType.InvalidParameter);
            Assert.IsNull(security.Groups.SelectMany(group => group.Accounts).SelectMany(account => account.Players).FirstOrDefault());
        }

        /// <summary>
        /// Ensures a player cannot be removed if the account does not have permission to do so.
        /// </summary>
        [TestMethod]
        public void TestSecurityRemovePlayerInsufficientPermission() {
            SecurityController security = new SecurityController();
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAddGroup, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "GroupName" }) });
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityGroupAddAccount, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "GroupName", "Phogue" }) });
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAccountAddPlayer, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "Phogue", CommonGameType.BF_3, "ABCDEF" }) });

            // Now remove the player.
            CommandResultArgs result = security.Execute(new Command() {
                CommandType = CommandType.SecurityRemovePlayer,
                Username = "Phogue",
                Origin = CommandOrigin.Remote,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    CommonGameType.BF_3, 
                    "ABCDEF"
                })
            });

            Assert.IsFalse(result.Success);
            Assert.AreEqual<CommandResultType>(CommandResultType.InsufficientPermissions, result.Status);
        }

        #endregion
    }
}
