#region

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Procon.Core.Security;
using Procon.Core.Shared;
using Procon.Net.Protocols;
using Procon.Net.Shared.Protocols;

#endregion

namespace Procon.Core.Test.Security {
    [TestFixture]
    public class TestSecurityPlayer {
        /// <summary>
        ///     Tests that a player can be added to an account.
        /// </summary>
        [Test]
        public void TestSecurityAccountAddPlayer() {
            var security = new SecurityController();
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAddGroup,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName"
                })
            });

            // Test that the group was initially added.
            Assert.AreEqual(security.Groups.First().Name, "GroupName");

            // Now add the user.
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupAddAccount,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName",
                    "Phogue"
                })
            });

            // Test that the account was added to the group.
            Assert.AreEqual(security.Groups.SelectMany(group => group.Accounts).First().Username, "Phogue");

            // Now add a player to the "Phogue" account.
            CommandResultArgs result = security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAccountAddPlayer,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "Phogue",
                    CommonGameType.BF_3,
                    "ABCDEF"
                })
            });

            // Validate the player was added successfully.
            Assert.IsTrue(result.Success);
            Assert.AreEqual(result.Status, CommandResultType.Success);
            Assert.AreEqual(security.Groups.SelectMany(group => group.Accounts).SelectMany(account => account.Players).First().GameType, CommonGameType.BF_3);
            Assert.AreEqual(security.Groups.SelectMany(group => group.Accounts).SelectMany(account => account.Players).First().Uid, "ABCDEF");
        }

        /// <summary>
        ///     Tests that a player cannot be added with a zero length UID.
        /// </summary>
        [Test]
        public void TestSecurityAccountAddPlayerEmptyUID() {
            var security = new SecurityController();
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAddGroup,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName"
                })
            });

            // Test that the group was initially added.
            Assert.AreEqual(security.Groups.First().Name, "GroupName");

            // Now add the user.
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupAddAccount,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName",
                    "Phogue"
                })
            });

            // Test that the account was added to the group.
            Assert.AreEqual(security.Groups.SelectMany(group => group.Accounts).First().Username, "Phogue");

            // Now add a player to the "Phogue" account.
            CommandResultArgs result = security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAccountAddPlayer,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "Phogue",
                    CommonGameType.BF_3,
                    String.Empty
                })
            });

            // Validate the player was added successfully.
            Assert.IsFalse(result.Success);
            Assert.AreEqual(result.Status, CommandResultType.InvalidParameter);
        }

        /// <summary>
        ///     Tests that a player will be re-assigned to another account if the player already exists.
        /// </summary>
        [Test]
        public void TestSecurityAccountAddPlayerExistingPlayer() {
            var security = new SecurityController();
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAddGroup,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName"
                })
            });

            // Test that the group was initially added.
            Assert.AreEqual(security.Groups.First().Name, "GroupName");

            // Now add the user.
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupAddAccount,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName",
                    "Phogue"
                })
            });
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupAddAccount,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName",
                    "PapaCharlie9"
                })
            });

            // Test that the account was added to the group.
            Assert.IsNotNull(security.Groups.SelectMany(group => group.Accounts).Where(account => account.Username == "Phogue").FirstOrDefault());
            Assert.IsNotNull(security.Groups.SelectMany(group => group.Accounts).Where(account => account.Username == "PapaCharlie9").FirstOrDefault());

            // Now add a player to the "Phogue" account.
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAccountAddPlayer,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "Phogue",
                    CommonGameType.BF_3,
                    "ABCDEF"
                })
            });


            // Validate the player was added successfully to the Phogue account.
            // and the PapaCharlie9 account still has no players.
            Assert.AreEqual(security.Groups.SelectMany(group => group.Accounts).Where(account => account.Username == "Phogue").SelectMany(account => account.Players).First().GameType, CommonGameType.BF_3);
            Assert.AreEqual(security.Groups.SelectMany(group => group.Accounts).Where(account => account.Username == "Phogue").SelectMany(account => account.Players).First().Uid, "ABCDEF");
            Assert.IsNull(security.Groups.SelectMany(group => group.Accounts).Where(account => account.Username == "PapaCharlie9").SelectMany(account => account.Players).FirstOrDefault());

            // Now add a player to the "PapaCharlie9" account.
            CommandResultArgs result = security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAccountAddPlayer,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "PapaCharlie9",
                    CommonGameType.BF_3,
                    "ABCDEF"
                })
            });

            // Validate the command was a success and the player is attached to the "PapaCharlie9" account
            // and no longer attached to the "Phogue" account.
            Assert.IsTrue(result.Success);
            Assert.AreEqual(result.Status, CommandResultType.Success);
            Assert.IsNull(security.Groups.SelectMany(group => group.Accounts).Where(account => account.Username == "Phogue").SelectMany(account => account.Players).FirstOrDefault());
            Assert.AreEqual(security.Groups.SelectMany(group => group.Accounts).Where(account => account.Username == "PapaCharlie9").SelectMany(account => account.Players).First().GameType, CommonGameType.BF_3);
            Assert.AreEqual(security.Groups.SelectMany(group => group.Accounts).Where(account => account.Username == "PapaCharlie9").SelectMany(account => account.Players).First().Uid, "ABCDEF");
        }

        /// <summary>
        ///     Tests that we cannot add a player to an account if we don't have permission to do so.
        /// </summary>
        [Test]
        public void TestSecurityAccountAddPlayerInsufficientPermission() {
            var security = new SecurityController();
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAddGroup,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName"
                })
            });
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupAddAccount,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName",
                    "Phogue"
                })
            });

            CommandResultArgs result = security.Tunnel(new Command() {
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
            Assert.AreEqual(CommandResultType.InsufficientPermissions, result.Status);
        }

        /// <summary>
        ///     Tests that a player can be removed if the player exists.
        /// </summary>
        [Test]
        public void TestSecurityRemovePlayer() {
            var security = new SecurityController();
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAddGroup,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName"
                })
            });

            // Test that the group was initially added.
            Assert.AreEqual(security.Groups.First().Name, "GroupName");

            // Now add the user.
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupAddAccount,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName",
                    "Phogue"
                })
            });

            // Test that the account was added to the group.
            Assert.IsNotNull(security.Groups.SelectMany(group => group.Accounts).Where(account => account.Username == "Phogue").FirstOrDefault());

            // Now add a player to the "Phogue" account.
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAccountAddPlayer,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "Phogue",
                    CommonGameType.BF_3,
                    "ABCDEF"
                })
            });

            // Validate the player was added successfully to the Phogue account.
            // and the PapaCharlie9 account still has no players.
            Assert.AreEqual(security.Groups.SelectMany(group => group.Accounts).Where(account => account.Username == "Phogue").SelectMany(account => account.Players).First().GameType, CommonGameType.BF_3);
            Assert.AreEqual(security.Groups.SelectMany(group => group.Accounts).Where(account => account.Username == "Phogue").SelectMany(account => account.Players).First().Uid, "ABCDEF");

            // Now remove the player.
            CommandResultArgs result = security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityRemovePlayer,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    CommonGameType.BF_3,
                    "ABCDEF"
                })
            });

            // Validate the command was a success and the player is not attached to any accounts.
            // and no longer attached to the "Phogue" account.
            Assert.IsTrue(result.Success);
            Assert.AreEqual(result.Status, CommandResultType.Success);
            Assert.IsNull(security.Groups.SelectMany(group => group.Accounts).Where(account => account.Username == "Phogue").SelectMany(account => account.Players).FirstOrDefault());
        }

        /// <summary>
        ///     Tests that a player will not be removed as well as returning the valid errors if the player does not exist.
        /// </summary>
        [Test]
        public void TestSecurityRemovePlayerDoesNotExist() {
            var security = new SecurityController();

            // Remove a player, though no accounts/players/groups exist.
            CommandResultArgs result = security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityRemovePlayer,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    CommonGameType.BF_3,
                    "ABCDEF"
                })
            });

            // Validate the command failed and returned the correct error status.
            Assert.IsFalse(result.Success);
            Assert.AreEqual(result.Status, CommandResultType.DoesNotExists);
            Assert.IsNull(security.Groups.SelectMany(group => group.Accounts).SelectMany(account => account.Players).FirstOrDefault());
        }

        /// <summary>
        ///     Tests that a player cannot be removed if it has a zero length UID.
        /// </summary>
        [Test]
        public void TestSecurityRemovePlayerEmptyUid() {
            var security = new SecurityController();

            // Remove a player, though no accounts/players/groups exist.
            CommandResultArgs result = security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityRemovePlayer,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    CommonGameType.BF_3,
                    String.Empty
                })
            });

            // Validate the command failed and returned the correct error status.
            Assert.IsFalse(result.Success);
            Assert.AreEqual(result.Status, CommandResultType.InvalidParameter);
            Assert.IsNull(security.Groups.SelectMany(group => group.Accounts).SelectMany(account => account.Players).FirstOrDefault());
        }

        /// <summary>
        ///     Ensures a player cannot be removed if the account does not have permission to do so.
        /// </summary>
        [Test]
        public void TestSecurityRemovePlayerInsufficientPermission() {
            var security = new SecurityController();
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAddGroup,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName"
                })
            });
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupAddAccount,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName",
                    "Phogue"
                })
            });
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAccountAddPlayer,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "Phogue",
                    CommonGameType.BF_3,
                    "ABCDEF"
                })
            });

            // Now remove the player.
            CommandResultArgs result = security.Tunnel(new Command() {
                CommandType = CommandType.SecurityRemovePlayer,
                Username = "Phogue",
                Origin = CommandOrigin.Remote,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    CommonGameType.BF_3,
                    "ABCDEF"
                })
            });

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.InsufficientPermissions, result.Status);
        }
    }
}