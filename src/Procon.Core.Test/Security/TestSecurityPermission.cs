using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Procon.Core.Test.Security {
    using Procon.Core.Security;
    using Procon.Net.Protocols;
    using Procon.Net.Utils;

    [TestFixture]
    public class TestSecurityPermission {

        #region Get Account

        [Test]
        public void TestSecurityGetAccountByUsername() {
            SecurityController security = new SecurityController();
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAddGroup, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "GroupName" }) });
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityGroupAddAccount, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "GroupName", "Phogue" }) });
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAccountAddPlayer, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "Phogue", CommonGameType.BF_3, "ABCDEF" }) });

            Account account = security.GetAccount("Phogue");

            // Validate the account was fetched successfully
            Assert.IsNotNull(account);
            Assert.AreEqual("Phogue", account.Username);
        }

        [Test]
        public void TestSecurityGetAccountByPlayerDetails() {
            SecurityController security = new SecurityController();
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAddGroup, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "GroupName" }) });
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityGroupAddAccount, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "GroupName", "Phogue" }) });
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAccountAddPlayer, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "Phogue", CommonGameType.BF_3, "ABCDEF" }) });

            Account account = security.GetAccount(CommonGameType.BF_3, "ABCDEF");

            // Validate the account was fetched successfully
            Assert.IsNotNull(account);
            Assert.AreEqual("Phogue", account.Username);
        }

        [Test]
        public void TestSecurityGetAccountByCommandInitiatorWithUsername() {
            SecurityController security = new SecurityController();
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAddGroup, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "GroupName" }) });
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityGroupAddAccount, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "GroupName", "Phogue" }) });
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAccountAddPlayer, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "Phogue", CommonGameType.BF_3, "ABCDEF" }) });

            Account account = security.GetAccount(new Command() {
                Username = "Phogue"
            });

            // Validate the account was fetched successfully
            Assert.IsNotNull(account);
            Assert.AreEqual("Phogue", account.Username);
        }

        [Test]
        public void TestSecurityGetAccountByCommandInitiatorWithPlayerDetails() {
            SecurityController security = new SecurityController();
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAddGroup, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "GroupName" }) });
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityGroupAddAccount, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "GroupName", "Phogue" }) });
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAccountAddPlayer, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "Phogue", CommonGameType.BF_3, "ABCDEF" }) });

            Account account = security.GetAccount(new Command() {
                GameType = CommonGameType.BF_3,
                Uid = "ABCDEF"
            });

            // Validate the account was fetched successfully
            Assert.IsNotNull(account);
            Assert.AreEqual("Phogue", account.Username);
        }

        #endregion

        #region More authority versus less authority

        [Test]
        public void TestSecurityQueryDetailsMoreAuthorityByPlayerDetails() {
            SecurityController security = new SecurityController();
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAddGroup, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "FirstGroupName" }) });
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityGroupAddAccount, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "FirstGroupName", "Phogue" }) });
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityGroupSetPermission, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "FirstGroupName", CommandType.NetworkProtocolActionKick, 100 }) });
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAccountAddPlayer, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "Phogue", CommonGameType.BF_3, "ABCDEF" }) });

            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAddGroup, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "SecondGroupName" }) });
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityGroupAddAccount, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "SecondGroupName", "Zaeed" }) });
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityGroupSetPermission, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "SecondGroupName", CommandType.NetworkProtocolActionKick, 50 }) });
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAccountAddPlayer, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "Zaeed", CommonGameType.BF_3, "0123456789" }) });

            CommandResultArgs result = security.Execute(new Command() {
                CommandType = CommandType.SecurityQueryPermission,
                Username = "Phogue",
                Origin = CommandOrigin.Remote,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    CommandType.NetworkProtocolActionKick,
                    CommonGameType.BF_3, 
                    "0123456789"
                })
            });

            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Success, result.Status);
        }

        [Test]
        public void TestSecurityQueryDetailsMoreAuthorityByAccountUsername() {
            SecurityController security = new SecurityController();
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAddGroup, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "FirstGroupName" }) });
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityGroupAddAccount, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "FirstGroupName", "Phogue" }) });
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityGroupSetPermission, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "FirstGroupName", CommandType.NetworkProtocolActionKick, 100 }) });
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAccountAddPlayer, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "Phogue", CommonGameType.BF_3, "ABCDEF" }) });

            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAddGroup, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "SecondGroupName" }) });
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityGroupAddAccount, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "SecondGroupName", "Zaeed" }) });
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityGroupSetPermission, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "SecondGroupName", CommandType.NetworkProtocolActionKick, 50 }) });
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAccountAddPlayer, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "Zaeed", CommonGameType.BF_3, "0123456789" }) });

            CommandResultArgs result = security.Execute(new Command() {
                CommandType = CommandType.SecurityQueryPermission,
                Username = "Phogue",
                Origin = CommandOrigin.Remote,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    CommandType.NetworkProtocolActionKick,
                    "Zaeed"
                })
            });

            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Success, result.Status);
        }

        #endregion

        #region Less authority versus more authority

        [Test]
        public void TestSecurityQueryDetailsLessAuthorityByPlayerDetails() {
            SecurityController security = new SecurityController();
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAddGroup, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "FirstGroupName" }) });
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityGroupAddAccount, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "FirstGroupName", "Phogue" }) });
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityGroupSetPermission, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "FirstGroupName", CommandType.NetworkProtocolActionKick, 100 }) });
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAccountAddPlayer, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "Phogue", CommonGameType.BF_3, "ABCDEF" }) });

            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAddGroup, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "SecondGroupName" }) });
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityGroupAddAccount, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "SecondGroupName", "Zaeed" }) });
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityGroupSetPermission, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "SecondGroupName", CommandType.NetworkProtocolActionKick, 50 }) });
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAccountAddPlayer, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "Zaeed", CommonGameType.BF_3, "0123456789" }) });

            CommandResultArgs result = security.Execute(new Command() {
                CommandType = CommandType.SecurityQueryPermission,
                Username = "Zaeed",
                Origin = CommandOrigin.Remote,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    CommandType.NetworkProtocolActionKick, 
                    CommonGameType.BF_3, 
                    "ABCDEF"
                })
            });

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.InsufficientAuthority, result.Status);
        }

        [Test]
        public void TestSecurityQueryDetailsLessAuthorityByAccountUsername() {
            SecurityController security = new SecurityController();
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAddGroup, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "FirstGroupName" }) });
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityGroupAddAccount, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "FirstGroupName", "Phogue" }) });
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityGroupSetPermission, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "FirstGroupName", CommandType.NetworkProtocolActionKick, 100 }) });
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAccountAddPlayer, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "Phogue", CommonGameType.BF_3, "ABCDEF" }) });

            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAddGroup, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "SecondGroupName" }) });
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityGroupAddAccount, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "SecondGroupName", "Zaeed" }) });
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityGroupSetPermission, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "SecondGroupName", CommandType.NetworkProtocolActionKick, 50 }) });
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAccountAddPlayer, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "Zaeed", CommonGameType.BF_3, "0123456789" }) });

            CommandResultArgs result = security.Execute(new Command() {
                CommandType = CommandType.SecurityQueryPermission,
                Username = "Zaeed",
                Origin = CommandOrigin.Remote,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    CommandType.NetworkProtocolActionKick, 
                    "Phogue"
                })
            });

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.InsufficientAuthority, result.Status);
        }

        #endregion

        #region Any authority versus no authority

        [Test]
        public void TestSecurityQueryDetailsNoAuthorityByPlayerDetails() {
            SecurityController security = new SecurityController();
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAddGroup, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "FirstGroupName" }) });
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityGroupAddAccount, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "FirstGroupName", "Phogue" }) });
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityGroupSetPermission, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "FirstGroupName", CommandType.NetworkProtocolActionKick, 100 }) });
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAccountAddPlayer, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "Phogue", CommonGameType.BF_3, "ABCDEF" }) });

            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAddGroup, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "SecondGroupName" }) });
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityGroupAddAccount, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "SecondGroupName", "Zaeed" }) });
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityGroupSetPermission, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "SecondGroupName", CommandType.NetworkProtocolActionKick, 50 }) });
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAccountAddPlayer, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "Zaeed", CommonGameType.BF_3, "0123456789" }) });

            CommandResultArgs result = security.Execute(new Command() {
                CommandType = CommandType.SecurityQueryPermission,
                Username = "Phogue",
                Origin = CommandOrigin.Remote,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    CommandType.NetworkProtocolActionKick, 
                    CommonGameType.BF_3, 
                    "DoesNotExist"
                })
            });

            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Success, result.Status);
        }

        [Test]
        public void TestSecurityQueryDetailsNoAuthorityByAccountUsername() {
            SecurityController security = new SecurityController();
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAddGroup, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "FirstGroupName" }) });
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityGroupAddAccount, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "FirstGroupName", "Phogue" }) });
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityGroupSetPermission, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "FirstGroupName", CommandType.NetworkProtocolActionKick, 100 }) });
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAccountAddPlayer, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "Phogue", CommonGameType.BF_3, "ABCDEF" }) });

            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAddGroup, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "SecondGroupName" }) });
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityGroupAddAccount, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "SecondGroupName", "Zaeed" }) });
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityGroupSetPermission, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "SecondGroupName", CommandType.NetworkProtocolActionKick, 50 }) });
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAccountAddPlayer, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "Zaeed", CommonGameType.BF_3, "0123456789" }) });

            CommandResultArgs result = security.Execute(new Command() {
                CommandType = CommandType.SecurityQueryPermission,
                Username = "Phogue",
                Origin = CommandOrigin.Remote,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    CommandType.NetworkProtocolActionKick,
                    "DoesNotExist"
                })
            });

            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Success, result.Status);
        }

        #endregion

        #region Origins

        /// <summary>
        /// Tests that a security check will succeed if the origin is a plugin and no further
        /// details about who executed the command are sent through.
        /// </summary>
        [Test]
        public void TestSecurityQueryDetailsPluginOrigin() {
            SecurityController security = new SecurityController();
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAddGroup, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "FirstGroupName" }) });
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityGroupAddAccount, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "FirstGroupName", "Phogue" }) });
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityGroupSetPermission, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "FirstGroupName", CommandType.NetworkProtocolActionKick, 100 }) });
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAccountAddPlayer, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "Phogue", CommonGameType.BF_3, "ABCDEF" }) });

            CommandResultArgs result = security.Execute(new Command() {
                CommandType = CommandType.SecurityQueryPermission,
                Origin = CommandOrigin.Plugin,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    CommandType.NetworkProtocolActionKick, 
                    CommonGameType.BF_3, 
                    "DoesNotExist"
                })
            });

            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Success, result.Status);
        }

        [Test]
        public void TestSecurityQueryDetailsUnknownOrigin() {
            SecurityController security = new SecurityController();
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAddGroup, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "FirstGroupName" }) });
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityGroupAddAccount, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "FirstGroupName", "Phogue" }) });
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityGroupSetPermission, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "FirstGroupName", CommandType.NetworkProtocolActionKick, 100 }) });
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAccountAddPlayer, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "Phogue", CommonGameType.BF_3, "ABCDEF" }) });

            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAddGroup, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "SecondGroupName" }) });
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityGroupAddAccount, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "SecondGroupName", "Zaeed" }) });
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityGroupSetPermission, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "SecondGroupName", CommandType.NetworkProtocolActionKick, 50 }) });
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAccountAddPlayer, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "Zaeed", CommonGameType.BF_3, "0123456789" }) });

            CommandResultArgs result = security.Execute(new Command() {
                CommandType = CommandType.SecurityQueryPermission,
                Username = "Phogue",
                Origin = CommandOrigin.None,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    CommandType.NetworkProtocolActionKick, 
                    CommonGameType.BF_3, 
                    "DoesNotExist"
                })
            });

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.InsufficientPermissions, result.Status);
        }

        #endregion

    }
}
