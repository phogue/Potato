#region

using System;
using System.Collections.Generic;
using NUnit.Framework;
using Procon.Core.Security;
using Procon.Core.Shared;
using Procon.Net.Shared.Protocols;

#endregion

namespace Procon.Core.Test.Security {
    [TestFixture]
    public class TestSecurityPermission {
        [SetUp]
        public void Initialize() {
            SharedReferences.Setup();
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
                    CommandType.VariablesSet,
                    100
                })
            });
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAccountAddPlayer,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "Phogue",
                    CommonProtocolType.DiceBattlefield3,
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
                    CommandType.VariablesSet,
                    50
                })
            });
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAccountAddPlayer,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "Zaeed",
                    CommonProtocolType.DiceBattlefield3,
                    "0123456789"
                })
            });

            ICommandResult result = security.Tunnel(new Command() {
                CommandType = CommandType.SecurityQueryPermission,
                Authentication = {
                    Username = "Zaeed",
                },
                Origin = CommandOrigin.Remote,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    CommandType.VariablesSet,
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
                    CommandType.VariablesSet,
                    100
                })
            });
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAccountAddPlayer,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "Phogue",
                    CommonProtocolType.DiceBattlefield3,
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
                    CommandType.VariablesSet,
                    50
                })
            });
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAccountAddPlayer,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "Zaeed",
                    CommonProtocolType.DiceBattlefield3,
                    "0123456789"
                })
            });

            ICommandResult result = security.Tunnel(new Command() {
                CommandType = CommandType.SecurityQueryPermission,
                Authentication = {
                    GameType = CommonProtocolType.DiceBattlefield3,
                    Uid = "0123456789"
                },
                Origin = CommandOrigin.Remote,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    CommandType.VariablesSet,
                    CommonProtocolType.DiceBattlefield3,
                    "ABCDEF"
                })
            });

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.InsufficientAuthority, result.Status);
        }

        /// <summary>
        /// Tests that no authority set for a permission will result in a simple
        /// insufficient permission error.
        /// </summary>
        [Test]
        public void TestSecurityQueryDetailsNoAuthorityByPlayerDetailsAsGuest() {
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
                    CommandType.VariablesSet,
                    100
                })
            });
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAccountAddPlayer,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "Phogue",
                    CommonProtocolType.DiceBattlefield3,
                    "ABCDEF"
                })
            });

            ICommandResult result = security.Tunnel(new Command() {
                CommandType = CommandType.SecurityQueryPermission,
                Authentication = {
                    GameType = CommonProtocolType.DiceBattlefield3,
                    Uid = "0123456789"
                },
                Origin = CommandOrigin.Plugin,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    CommandType.VariablesSet,
                    CommonProtocolType.DiceBattlefield3,
                    "ABCDEF"
                })
            });

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.InsufficientPermissions, result.Status);
        }

        /// <summary>
        /// Tests that a guest account with less authority against an established account with more authority
        /// will fail.
        /// </summary>
        [Test]
        public void TestSecurityQueryDetailsLessAuthorityByPlayerDetailsAsGuest() {
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
                    CommandType.VariablesSet,
                    100
                })
            });
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAccountAddPlayer,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "Phogue",
                    CommonProtocolType.DiceBattlefield3,
                    "ABCDEF"
                })
            });

            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupSetPermission,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "Guest",
                    CommandType.VariablesSet,
                    50
                })
            });

            ICommandResult result = security.Tunnel(new Command() {
                CommandType = CommandType.SecurityQueryPermission,
                Authentication = {
                    GameType = CommonProtocolType.DiceBattlefield3,
                    Uid = "0123456789"
                },
                Origin = CommandOrigin.Plugin,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    CommandType.VariablesSet,
                    CommonProtocolType.DiceBattlefield3,
                    "ABCDEF"
                })
            });

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.InsufficientAuthority, result.Status);
        }

        /// <summary>
        /// Tests that a guest with more authority will be successful against an established 
        /// account with less authority.
        /// </summary>
        [Test]
        public void TestSecurityQueryDetailsMoreAuthorityByPlayerDetailsAsGuest() {
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
                    CommandType.VariablesSet,
                    100
                })
            });
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAccountAddPlayer,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "Phogue",
                    CommonProtocolType.DiceBattlefield3,
                    "ABCDEF"
                })
            });

            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupSetPermission,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "Guest",
                    CommandType.VariablesSet,
                    200
                })
            });

            ICommandResult result = security.Tunnel(new Command() {
                CommandType = CommandType.SecurityQueryPermission,
                Authentication = {
                    GameType = CommonProtocolType.DiceBattlefield3,
                    Uid = "0123456789"
                },
                Origin = CommandOrigin.Plugin,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    CommandType.VariablesSet,
                    CommonProtocolType.DiceBattlefield3,
                    "ABCDEF"
                })
            });

            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Success, result.Status);
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
                    CommandType.VariablesSet,
                    100
                })
            });
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAccountAddPlayer,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "Phogue",
                    CommonProtocolType.DiceBattlefield3,
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
                    CommandType.VariablesSet,
                    50
                })
            });
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAccountAddPlayer,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "Zaeed",
                    CommonProtocolType.DiceBattlefield3,
                    "0123456789"
                })
            });

            ICommandResult result = security.Tunnel(new Command() {
                CommandType = CommandType.SecurityQueryPermission,
                Authentication = {
                    Username = "Phogue"
                },
                Origin = CommandOrigin.Remote,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    CommandType.VariablesSet,
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
                    CommandType.VariablesSet,
                    100
                })
            });
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAccountAddPlayer,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "Phogue",
                    CommonProtocolType.DiceBattlefield3,
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
                    CommandType.VariablesSet,
                    50
                })
            });
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAccountAddPlayer,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "Zaeed",
                    CommonProtocolType.DiceBattlefield3,
                    "0123456789"
                })
            });

            ICommandResult result = security.Tunnel(new Command() {
                CommandType = CommandType.SecurityQueryPermission,
                Authentication = {
                    Username = "Phogue"
                },
                Origin = CommandOrigin.Remote,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    CommandType.VariablesSet,
                    CommonProtocolType.DiceBattlefield3,
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
                    CommandType.VariablesSet,
                    100
                })
            });
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAccountAddPlayer,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "Phogue",
                    CommonProtocolType.DiceBattlefield3,
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
                    CommandType.VariablesSet,
                    50
                })
            });
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAccountAddPlayer,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "Zaeed",
                    CommonProtocolType.DiceBattlefield3,
                    "0123456789"
                })
            });

            ICommandResult result = security.Tunnel(new Command() {
                CommandType = CommandType.SecurityQueryPermission,
                Authentication = {
                    Username = "Phogue"
                },
                Origin = CommandOrigin.Remote,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    CommandType.VariablesSet,
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
                    CommandType.VariablesSet,
                    100
                })
            });
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAccountAddPlayer,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "Phogue",
                    CommonProtocolType.DiceBattlefield3,
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
                    CommandType.VariablesSet,
                    50
                })
            });
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAccountAddPlayer,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "Zaeed",
                    CommonProtocolType.DiceBattlefield3,
                    "0123456789"
                })
            });

            ICommandResult result = security.Tunnel(new Command() {
                CommandType = CommandType.SecurityQueryPermission,
                Authentication = {
                    Username = "Phogue"
                },
                Origin = CommandOrigin.Remote,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    CommandType.VariablesSet,
                    CommonProtocolType.DiceBattlefield3,
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
                    CommandType.VariablesSet,
                    100
                })
            });
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAccountAddPlayer,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "Phogue",
                    CommonProtocolType.DiceBattlefield3,
                    "ABCDEF"
                })
            });

            ICommandResult result = security.Tunnel(new Command() {
                CommandType = CommandType.SecurityQueryPermission,
                Origin = CommandOrigin.Plugin,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    CommandType.VariablesSet,
                    CommonProtocolType.DiceBattlefield3,
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
                    CommandType.VariablesSet,
                    100
                })
            });
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAccountAddPlayer,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "Phogue",
                    CommonProtocolType.DiceBattlefield3,
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
                    CommandType.VariablesSet,
                    50
                })
            });
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAccountAddPlayer,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "Zaeed",
                    CommonProtocolType.DiceBattlefield3,
                    "0123456789"
                })
            });

            ICommandResult result = security.Tunnel(new Command() {
                CommandType = CommandType.SecurityQueryPermission,
                Authentication = {
                    Username = "Phogue"
                },
                Origin = CommandOrigin.None,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    CommandType.VariablesSet,
                    CommonProtocolType.DiceBattlefield3,
                    "DoesNotExist"
                })
            });

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.InsufficientPermissions, result.Status);
        }
    }
}