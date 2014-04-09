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
using NUnit.Framework;
using Procon.Core.Security;
using Procon.Core.Shared;
using Procon.Core.Shared.Models;
using Procon.Net.Shared.Protocols;

namespace Procon.Core.Test.Security.Permission {
    [TestFixture]
    public class TestSecurityQueryPermission {
        [SetUp]
        public void Initialize() {
            SharedReferences.Setup();
        }
        
        /// <summary>
        /// Tests that a group with 0 authority in a permission is equal to having
        /// no permission set at all (null)
        /// </summary>
        [Test]
        public void TestZeroedAuthorityEqualsNoPermissions() {
            var security = new SecurityController();

            security.Tunnel(CommandBuilder.SecurityAddGroup("GroupName").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupAddAccount("GroupName", "Phogue").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupSetPermission("GroupName", CommandType.VariablesSet, 0).SetOrigin(CommandOrigin.Local));

            ICommandResult result = security.Tunnel(CommandBuilder.SecurityQueryPermission(CommandType.VariablesSet, "DoesNotExist")
                .SetOrigin(CommandOrigin.Remote)
                .SetAuthentication(new CommandAuthenticationModel() {
                    Username = "Phogue"
                })
            );

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.InsufficientPermissions, result.CommandResultType);
        }

        /// <summary>
        /// Tests that a group with 0 authority in a permission is equal to having
        /// no permission set at all (null)
        /// </summary>
        [Test]
        public void TestZeroedAuthorityEqualsNoPermissionsCaseInsensitive() {
            var security = new SecurityController();

            security.Tunnel(CommandBuilder.SecurityAddGroup("GroupName").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupAddAccount("GroupName", "Phogue").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupSetPermission("GroupName", CommandType.VariablesSet, 0).SetOrigin(CommandOrigin.Local));

            ICommandResult result = security.Tunnel(CommandBuilder.SecurityQueryPermission(CommandType.VariablesSet, "DoesNotExist")
                .SetOrigin(CommandOrigin.Remote)
                .SetAuthentication(new CommandAuthenticationModel() {
                    Username = "PHOGUE"
                })
            );

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.InsufficientPermissions, result.CommandResultType);
        }

        [Test]
        public void TestDetailsLessAuthorityByAccountUsername() {
            var security = new SecurityController();
            security.Tunnel(CommandBuilder.SecurityAddGroup("FirstGroupName").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupAddAccount("FirstGroupName", "Phogue").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupSetPermission("FirstGroupName", CommandType.VariablesSet, 100).SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityAccountAddPlayer("Phogue", CommonProtocolType.DiceBattlefield3, "ABCDEF").SetOrigin(CommandOrigin.Local));

            security.Tunnel(CommandBuilder.SecurityAddGroup("SecondGroupName").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupAddAccount("SecondGroupName", "Zaeed").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupSetPermission("SecondGroupName", CommandType.VariablesSet, 50).SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityAccountAddPlayer("Zaeed", CommonProtocolType.DiceBattlefield3, "0123456789").SetOrigin(CommandOrigin.Local));

            ICommandResult result = security.Tunnel(CommandBuilder.SecurityQueryPermission(CommandType.VariablesSet, "Phogue")
                .SetOrigin(CommandOrigin.Remote)
                .SetAuthentication(new CommandAuthenticationModel() {
                    Username = "Zaeed"
                })
            );

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.InsufficientAuthority, result.CommandResultType);
        }

        [Test]
        public void TestDetailsLessAuthorityByPlayerDetails() {
            var security = new SecurityController();
            security.Tunnel(CommandBuilder.SecurityAddGroup("FirstGroupName").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupAddAccount("FirstGroupName", "Phogue").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupSetPermission("FirstGroupName", CommandType.VariablesSet, 100).SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityAccountAddPlayer("Phogue", CommonProtocolType.DiceBattlefield3, "ABCDEF").SetOrigin(CommandOrigin.Local));

            security.Tunnel(CommandBuilder.SecurityAddGroup("SecondGroupName").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupAddAccount("SecondGroupName", "Zaeed").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupSetPermission("SecondGroupName", CommandType.VariablesSet, 50).SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityAccountAddPlayer("Zaeed", CommonProtocolType.DiceBattlefield3, "0123456789").SetOrigin(CommandOrigin.Local));

            ICommandResult result = security.Tunnel(CommandBuilder.SecurityQueryPermission(CommandType.VariablesSet, CommonProtocolType.DiceBattlefield3, "ABCDEF")
                .SetOrigin(CommandOrigin.Remote)
                .SetAuthentication(new CommandAuthenticationModel() {
                    GameType = CommonProtocolType.DiceBattlefield3,
                    Uid = "0123456789"
                })
            );

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.InsufficientAuthority, result.CommandResultType);
        }

        /// <summary>
        /// Tests that no authority set for a permission will result in a simple
        /// insufficient permission error.
        /// </summary>
        [Test]
        public void TestDetailsNoAuthorityByPlayerDetailsAsGuest() {
            var security = new SecurityController();

            security.Tunnel(CommandBuilder.SecurityAddGroup("FirstGroupName").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupAddAccount("FirstGroupName", "Phogue").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupSetPermission("FirstGroupName", CommandType.VariablesSet, 100).SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityAccountAddPlayer("Phogue", CommonProtocolType.DiceBattlefield3, "ABCDEF").SetOrigin(CommandOrigin.Local));

            ICommandResult result = security.Tunnel(CommandBuilder.SecurityQueryPermission(CommandType.VariablesSet, CommonProtocolType.DiceBattlefield3, "ABCDEF")
                .SetOrigin(CommandOrigin.Remote)
                .SetAuthentication(new CommandAuthenticationModel() {
                    GameType = CommonProtocolType.DiceBattlefield3,
                    Uid = "0123456789"
                })
            );

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.InsufficientPermissions, result.CommandResultType);
        }

        /// <summary>
        /// Tests that a guest account with less authority against an established account with more authority
        /// will fail.
        /// </summary>
        [Test]
        public void TestDetailsLessAuthorityByPlayerDetailsAsGuest() {
            var security = new SecurityController();

            security.Tunnel(CommandBuilder.SecurityAddGroup("FirstGroupName").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupAddAccount("FirstGroupName", "Phogue").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupSetPermission("FirstGroupName", CommandType.VariablesSet, 100).SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityAccountAddPlayer("Phogue", CommonProtocolType.DiceBattlefield3, "ABCDEF").SetOrigin(CommandOrigin.Local));

            security.Tunnel(CommandBuilder.SecurityGroupSetPermission("Guest", CommandType.VariablesSet, 50).SetOrigin(CommandOrigin.Local));

            ICommandResult result = security.Tunnel(CommandBuilder.SecurityQueryPermission(CommandType.VariablesSet, CommonProtocolType.DiceBattlefield3, "ABCDEF")
                .SetOrigin(CommandOrigin.Remote)
                .SetAuthentication(new CommandAuthenticationModel() {
                    GameType = CommonProtocolType.DiceBattlefield3,
                    Uid = "0123456789"
                })
            );

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.InsufficientAuthority, result.CommandResultType);
        }

        /// <summary>
        /// Tests that a guest with more authority will be successful against an established 
        /// account with less authority.
        /// </summary>
        [Test]
        public void TestDetailsMoreAuthorityByPlayerDetailsAsGuest() {
            var security = new SecurityController();

            security.Tunnel(CommandBuilder.SecurityAddGroup("FirstGroupName").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupAddAccount("FirstGroupName", "Phogue").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupSetPermission("FirstGroupName", CommandType.VariablesSet, 100).SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityAccountAddPlayer("Phogue", CommonProtocolType.DiceBattlefield3, "ABCDEF").SetOrigin(CommandOrigin.Local));

            security.Tunnel(CommandBuilder.SecurityGroupSetPermission("Guest", CommandType.VariablesSet, 200).SetOrigin(CommandOrigin.Local));

            ICommandResult result = security.Tunnel(CommandBuilder.SecurityQueryPermission(CommandType.VariablesSet, CommonProtocolType.DiceBattlefield3, "ABCDEF")
                .SetOrigin(CommandOrigin.Remote)
                .SetAuthentication(new CommandAuthenticationModel() {
                    GameType = CommonProtocolType.DiceBattlefield3,
                    Uid = "0123456789"
                })
            );

            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Success, result.CommandResultType);
        }

        [Test]
        public void TestDetailsMoreAuthorityByAccountUsername() {
            var security = new SecurityController();

            security.Tunnel(CommandBuilder.SecurityAddGroup("FirstGroupName").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupAddAccount("FirstGroupName", "Phogue").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupSetPermission("FirstGroupName", CommandType.VariablesSet, 100).SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityAccountAddPlayer("Phogue", CommonProtocolType.DiceBattlefield3, "ABCDEF").SetOrigin(CommandOrigin.Local));

            security.Tunnel(CommandBuilder.SecurityAddGroup("SecondGroupName").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupAddAccount("SecondGroupName", "Zaeed").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupSetPermission("SecondGroupName", CommandType.VariablesSet, 50).SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityAccountAddPlayer("Zaeed", CommonProtocolType.DiceBattlefield3, "0123456789").SetOrigin(CommandOrigin.Local));

            ICommandResult result = security.Tunnel(CommandBuilder.SecurityQueryPermission(CommandType.VariablesSet, "Zaeed")
                .SetOrigin(CommandOrigin.Remote)
                .SetAuthentication(new CommandAuthenticationModel() {
                    Username = "Phogue"
                })
            );

            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Success, result.CommandResultType);
        }

        [Test]
        public void TestDetailsMoreAuthorityByAccountUsernameCaseInsensitive() {
            var security = new SecurityController();

            security.Tunnel(CommandBuilder.SecurityAddGroup("FirstGroupName").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupAddAccount("FirstGroupName", "Phogue").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupSetPermission("FirstGroupName", CommandType.VariablesSet, 100).SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityAccountAddPlayer("Phogue", CommonProtocolType.DiceBattlefield3, "ABCDEF").SetOrigin(CommandOrigin.Local));

            security.Tunnel(CommandBuilder.SecurityAddGroup("SecondGroupName").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupAddAccount("SecondGroupName", "Zaeed").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupSetPermission("SecondGroupName", CommandType.VariablesSet, 50).SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityAccountAddPlayer("Zaeed", CommonProtocolType.DiceBattlefield3, "0123456789").SetOrigin(CommandOrigin.Local));

            ICommandResult result = security.Tunnel(CommandBuilder.SecurityQueryPermission(CommandType.VariablesSet, "Zaeed")
                .SetOrigin(CommandOrigin.Remote)
                .SetAuthentication(new CommandAuthenticationModel() {
                    Username = "PHOGUE"
                })
            );

            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Success, result.CommandResultType);
        }

        [Test]
        public void TestDetailsMoreAuthorityByPlayerDetails() {
            var security = new SecurityController();

            security.Tunnel(CommandBuilder.SecurityAddGroup("FirstGroupName").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupAddAccount("FirstGroupName", "Phogue").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupSetPermission("FirstGroupName", CommandType.VariablesSet, 100).SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityAccountAddPlayer("Phogue", CommonProtocolType.DiceBattlefield3, "ABCDEF").SetOrigin(CommandOrigin.Local));

            security.Tunnel(CommandBuilder.SecurityAddGroup("SecondGroupName").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupAddAccount("SecondGroupName", "Zaeed").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupSetPermission("SecondGroupName", CommandType.VariablesSet, 50).SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityAccountAddPlayer("Zaeed", CommonProtocolType.DiceBattlefield3, "0123456789").SetOrigin(CommandOrigin.Local));

            ICommandResult result = security.Tunnel(CommandBuilder.SecurityQueryPermission(CommandType.VariablesSet, CommonProtocolType.DiceBattlefield3, "0123456789")
                .SetOrigin(CommandOrigin.Remote)
                .SetAuthentication(new CommandAuthenticationModel() {
                    Username = "Phogue"
                })
            );

            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Success, result.CommandResultType);
        }

        [Test]
        public void TestDetailsNoAuthorityByAccountUsername() {
            var security = new SecurityController();

            security.Tunnel(CommandBuilder.SecurityAddGroup("FirstGroupName").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupAddAccount("FirstGroupName", "Phogue").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupSetPermission("FirstGroupName", CommandType.VariablesSet, 100).SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityAccountAddPlayer("Phogue", CommonProtocolType.DiceBattlefield3, "ABCDEF").SetOrigin(CommandOrigin.Local));

            security.Tunnel(CommandBuilder.SecurityAddGroup("SecondGroupName").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupAddAccount("SecondGroupName", "Zaeed").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupSetPermission("SecondGroupName", CommandType.VariablesSet, 50).SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityAccountAddPlayer("Zaeed", CommonProtocolType.DiceBattlefield3, "0123456789").SetOrigin(CommandOrigin.Local));

            ICommandResult result = security.Tunnel(CommandBuilder.SecurityQueryPermission(CommandType.VariablesSet, "DoesNotExist")
                .SetOrigin(CommandOrigin.Remote)
                .SetAuthentication(new CommandAuthenticationModel() {
                    Username = "Phogue"
                })
            );

            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Success, result.CommandResultType);
        }

        [Test]
        public void TestDetailsNoAuthorityByAccountUsernameCaseInsensitive() {
            var security = new SecurityController();

            security.Tunnel(CommandBuilder.SecurityAddGroup("FirstGroupName").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupAddAccount("FirstGroupName", "Phogue").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupSetPermission("FirstGroupName", CommandType.VariablesSet, 100).SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityAccountAddPlayer("Phogue", CommonProtocolType.DiceBattlefield3, "ABCDEF").SetOrigin(CommandOrigin.Local));

            security.Tunnel(CommandBuilder.SecurityAddGroup("SecondGroupName").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupAddAccount("SecondGroupName", "Zaeed").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupSetPermission("SecondGroupName", CommandType.VariablesSet, 50).SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityAccountAddPlayer("Zaeed", CommonProtocolType.DiceBattlefield3, "0123456789").SetOrigin(CommandOrigin.Local));

            ICommandResult result = security.Tunnel(CommandBuilder.SecurityQueryPermission(CommandType.VariablesSet, "DoesNotExist")
                .SetOrigin(CommandOrigin.Remote)
                .SetAuthentication(new CommandAuthenticationModel() {
                    Username = "PHOGUE"
                })
            );

            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Success, result.CommandResultType);
        }

        [Test]
        public void TestDetailsNoAuthorityByPlayerDetails() {
            var security = new SecurityController();

            security.Tunnel(CommandBuilder.SecurityAddGroup("FirstGroupName").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupAddAccount("FirstGroupName", "Phogue").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupSetPermission("FirstGroupName", CommandType.VariablesSet, 100).SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityAccountAddPlayer("Phogue", CommonProtocolType.DiceBattlefield3, "ABCDEF").SetOrigin(CommandOrigin.Local));

            security.Tunnel(CommandBuilder.SecurityAddGroup("SecondGroupName").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupAddAccount("SecondGroupName", "Zaeed").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupSetPermission("SecondGroupName", CommandType.VariablesSet, 50).SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityAccountAddPlayer("Zaeed", CommonProtocolType.DiceBattlefield3, "0123456789").SetOrigin(CommandOrigin.Local));

            ICommandResult result = security.Tunnel(CommandBuilder.SecurityQueryPermission(CommandType.VariablesSet, CommonProtocolType.DiceBattlefield3, "DoesNotExist")
                .SetOrigin(CommandOrigin.Remote)
                .SetAuthentication(new CommandAuthenticationModel() {
                    Username = "Phogue"
                })
            );

            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Success, result.CommandResultType);
        }

        /// <summary>
        ///     Tests that a security check will succeed if the origin is a plugin and no further
        ///     details about who executed the command are sent through.
        /// </summary>
        [Test]
        public void TestDetailsPluginOrigin() {
            var security = new SecurityController();

            security.Tunnel(CommandBuilder.SecurityAddGroup("FirstGroupName").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupAddAccount("FirstGroupName", "Phogue").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupSetPermission("FirstGroupName", CommandType.VariablesSet, 100).SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityAccountAddPlayer("Phogue", CommonProtocolType.DiceBattlefield3, "ABCDEF").SetOrigin(CommandOrigin.Local));

            ICommandResult result = security.Tunnel(CommandBuilder.SecurityQueryPermission(CommandType.VariablesSet, "DoesNotExist")
                .SetOrigin(CommandOrigin.Plugin)
            );

            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Success, result.CommandResultType);
        }

        [Test]
        public void TestDetailsUnknownOrigin() {
            var security = new SecurityController();

            security.Tunnel(CommandBuilder.SecurityAddGroup("FirstGroupName").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupAddAccount("FirstGroupName", "Phogue").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupSetPermission("FirstGroupName", CommandType.VariablesSet, 100).SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityAccountAddPlayer("Phogue", CommonProtocolType.DiceBattlefield3, "ABCDEF").SetOrigin(CommandOrigin.Local));

            security.Tunnel(CommandBuilder.SecurityAddGroup("SecondGroupName").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupAddAccount("SecondGroupName", "Zaeed").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupSetPermission("SecondGroupName", CommandType.VariablesSet, 50).SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityAccountAddPlayer("Zaeed", CommonProtocolType.DiceBattlefield3, "0123456789").SetOrigin(CommandOrigin.Local));

            ICommandResult result = security.Tunnel(CommandBuilder.SecurityQueryPermission(CommandType.VariablesSet, CommonProtocolType.DiceBattlefield3, "DoesNotExist")
                .SetOrigin(CommandOrigin.None)
                .SetAuthentication(new CommandAuthenticationModel() {
                    Username = "Phogue"
                })
            );

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.InsufficientPermissions, result.CommandResultType);
        }
    }
}