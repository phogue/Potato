#region

using System;
using System.Collections.Generic;
using NUnit.Framework;
using Procon.Core.Security;
using Procon.Core.Shared;
using Procon.Core.Shared.Models;
using Procon.Net.Protocols;
using Procon.Net.Shared.Protocols;

#endregion

namespace Procon.Core.Test.Security {
    [TestFixture]
    public class TestSecurityPermission {
        [Test]
        public void TestSecurityGetAccountByCommandInitiatorWithPlayerDetails() {
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

            AccountModel account = security.GetAccount(new Command() {
                GameType = CommonGameType.BF_3,
                Uid = "ABCDEF"
            });

            // Validate the account was fetched successfully
            Assert.IsNotNull(account);
            Assert.AreEqual("Phogue", account.Username);
        }

        [Test]
        public void TestSecurityGetAccountByCommandInitiatorWithUsername() {
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

            AccountModel account = security.GetAccount(new Command() {
                Username = "Phogue"
            });

            // Validate the account was fetched successfully
            Assert.IsNotNull(account);
            Assert.AreEqual("Phogue", account.Username);
        }

        [Test]
        public void TestSecurityGetAccountByPlayerDetails() {
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

            AccountModel account = security.GetAccount(CommonGameType.BF_3, "ABCDEF");

            // Validate the account was fetched successfully
            Assert.IsNotNull(account);
            Assert.AreEqual("Phogue", account.Username);
        }

        [Test]
        public void TestSecurityGetAccountByUsername() {
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

            AccountModel account = security.GetAccount("Phogue");

            // Validate the account was fetched successfully
            Assert.IsNotNull(account);
            Assert.AreEqual("Phogue", account.Username);
        }

        [Test]
        public void TestSecurityQueryDetailsLessAuthorityByAccountUsername() {
            var security = new SecurityController();
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAddGroup,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "FirstGroupName"
                })
            });
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupAddAccount,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "FirstGroupName",
                    "Phogue"
                })
            });
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupSetPermission,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "FirstGroupName",
                    CommandType.NetworkProtocolActionKick,
                    100
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

            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAddGroup,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "SecondGroupName"
                })
            });
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupAddAccount,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "SecondGroupName",
                    "Zaeed"
                })
            });
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupSetPermission,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "SecondGroupName",
                    CommandType.NetworkProtocolActionKick,
                    50
                })
            });
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAccountAddPlayer,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "Zaeed",
                    CommonGameType.BF_3,
                    "0123456789"
                })
            });

            CommandResultArgs result = security.Tunnel(new Command() {
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

        [Test]
        public void TestSecurityQueryDetailsLessAuthorityByPlayerDetails() {
            var security = new SecurityController();
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAddGroup,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "FirstGroupName"
                })
            });
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupAddAccount,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "FirstGroupName",
                    "Phogue"
                })
            });
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupSetPermission,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "FirstGroupName",
                    CommandType.NetworkProtocolActionKick,
                    100
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

            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAddGroup,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "SecondGroupName"
                })
            });
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupAddAccount,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "SecondGroupName",
                    "Zaeed"
                })
            });
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupSetPermission,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "SecondGroupName",
                    CommandType.NetworkProtocolActionKick,
                    50
                })
            });
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAccountAddPlayer,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "Zaeed",
                    CommonGameType.BF_3,
                    "0123456789"
                })
            });

            CommandResultArgs result = security.Tunnel(new Command() {
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
        public void TestSecurityQueryDetailsMoreAuthorityByAccountUsername() {
            var security = new SecurityController();
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAddGroup,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "FirstGroupName"
                })
            });
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupAddAccount,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "FirstGroupName",
                    "Phogue"
                })
            });
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupSetPermission,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "FirstGroupName",
                    CommandType.NetworkProtocolActionKick,
                    100
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

            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAddGroup,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "SecondGroupName"
                })
            });
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupAddAccount,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "SecondGroupName",
                    "Zaeed"
                })
            });
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupSetPermission,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "SecondGroupName",
                    CommandType.NetworkProtocolActionKick,
                    50
                })
            });
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAccountAddPlayer,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "Zaeed",
                    CommonGameType.BF_3,
                    "0123456789"
                })
            });

            CommandResultArgs result = security.Tunnel(new Command() {
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

        [Test]
        public void TestSecurityQueryDetailsMoreAuthorityByPlayerDetails() {
            var security = new SecurityController();
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAddGroup,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "FirstGroupName"
                })
            });
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupAddAccount,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "FirstGroupName",
                    "Phogue"
                })
            });
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupSetPermission,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "FirstGroupName",
                    CommandType.NetworkProtocolActionKick,
                    100
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

            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAddGroup,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "SecondGroupName"
                })
            });
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupAddAccount,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "SecondGroupName",
                    "Zaeed"
                })
            });
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupSetPermission,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "SecondGroupName",
                    CommandType.NetworkProtocolActionKick,
                    50
                })
            });
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAccountAddPlayer,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "Zaeed",
                    CommonGameType.BF_3,
                    "0123456789"
                })
            });

            CommandResultArgs result = security.Tunnel(new Command() {
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
        public void TestSecurityQueryDetailsNoAuthorityByAccountUsername() {
            var security = new SecurityController();
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAddGroup,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "FirstGroupName"
                })
            });
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupAddAccount,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "FirstGroupName",
                    "Phogue"
                })
            });
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupSetPermission,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "FirstGroupName",
                    CommandType.NetworkProtocolActionKick,
                    100
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

            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAddGroup,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "SecondGroupName"
                })
            });
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupAddAccount,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "SecondGroupName",
                    "Zaeed"
                })
            });
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupSetPermission,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "SecondGroupName",
                    CommandType.NetworkProtocolActionKick,
                    50
                })
            });
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAccountAddPlayer,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "Zaeed",
                    CommonGameType.BF_3,
                    "0123456789"
                })
            });

            CommandResultArgs result = security.Tunnel(new Command() {
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

        [Test]
        public void TestSecurityQueryDetailsNoAuthorityByPlayerDetails() {
            var security = new SecurityController();
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAddGroup,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "FirstGroupName"
                })
            });
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupAddAccount,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "FirstGroupName",
                    "Phogue"
                })
            });
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupSetPermission,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "FirstGroupName",
                    CommandType.NetworkProtocolActionKick,
                    100
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

            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAddGroup,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "SecondGroupName"
                })
            });
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupAddAccount,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "SecondGroupName",
                    "Zaeed"
                })
            });
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupSetPermission,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "SecondGroupName",
                    CommandType.NetworkProtocolActionKick,
                    50
                })
            });
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAccountAddPlayer,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "Zaeed",
                    CommonGameType.BF_3,
                    "0123456789"
                })
            });

            CommandResultArgs result = security.Tunnel(new Command() {
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

        /// <summary>
        ///     Tests that a security check will succeed if the origin is a plugin and no further
        ///     details about who executed the command are sent through.
        /// </summary>
        [Test]
        public void TestSecurityQueryDetailsPluginOrigin() {
            var security = new SecurityController();
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAddGroup,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "FirstGroupName"
                })
            });
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupAddAccount,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "FirstGroupName",
                    "Phogue"
                })
            });
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupSetPermission,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "FirstGroupName",
                    CommandType.NetworkProtocolActionKick,
                    100
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

            CommandResultArgs result = security.Tunnel(new Command() {
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
            var security = new SecurityController();
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAddGroup,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "FirstGroupName"
                })
            });
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupAddAccount,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "FirstGroupName",
                    "Phogue"
                })
            });
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupSetPermission,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "FirstGroupName",
                    CommandType.NetworkProtocolActionKick,
                    100
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

            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAddGroup,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "SecondGroupName"
                })
            });
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupAddAccount,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "SecondGroupName",
                    "Zaeed"
                })
            });
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupSetPermission,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "SecondGroupName",
                    CommandType.NetworkProtocolActionKick,
                    50
                })
            });
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAccountAddPlayer,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "Zaeed",
                    CommonGameType.BF_3,
                    "0123456789"
                })
            });

            CommandResultArgs result = security.Tunnel(new Command() {
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
    }
}