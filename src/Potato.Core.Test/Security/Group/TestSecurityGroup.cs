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
#region

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Potato.Core.Security;
using Potato.Core.Shared;
using Potato.Core.Shared.Models;

#endregion

namespace Potato.Core.Test.Security.Group {
    [TestFixture]
    public class TestSecurityGroup {
        [SetUp]
        public void Initialize() {
            SharedReferences.Setup();
        }

        /// <summary>
        ///     Testing that we can add a simple group.
        /// </summary>
        [Test]
        public void TestSecurityAddGroup() {
            var security = new SecurityController();

            // Add a group.
            ICommandResult result = security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAddGroup,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName"
                })
            });

            // Make sure it was successful.
            Assert.IsTrue(result.Success);
            Assert.AreEqual(security.Groups.Last().Name, "GroupName");
        }

        /// <summary>
        ///     Testing that two groups with identical names can't be added to the security model.
        /// </summary>
        [Test]
        public void TestSecurityAddGroupDuplicateGroupName() {
            var security = new SecurityController();
            ICommandResult result = security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAddGroup,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName"
                })
            });

            // Test that the group was initially added.
            Assert.AreEqual(security.Groups.Last().Name, "GroupName");

            // Now readd the same group name.
            result = security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAddGroup,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName"
                })
            });

            // Test the second result, make sure it failed.
            Assert.IsFalse(result.Success);
            Assert.AreEqual(result.CommandResultType, CommandResultType.AlreadyExists);
        }

        /// <summary>
        ///     Testing that a group with an empty name can't be added to the security model.
        /// </summary>
        [Test]
        public void TestSecurityAddGroupEmptyGroupName() {
            var security = new SecurityController();

            // Add a group with an empty name.
            ICommandResult result = security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAddGroup,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    String.Empty
                })
            });

            // Make sure adding an empty group fails.
            Assert.IsFalse(result.Success);
            Assert.AreEqual(result.CommandResultType, CommandResultType.InvalidParameter);
        }

        /// <summary>
        ///     Tests that a group cannot be added unless the user has sufficient permissions to do so.
        /// </summary>
        [Test]
        public void TestSecurityAddGroupInsufficientPermission() {
            var security = new SecurityController();

            // Add a group.
            ICommandResult result = security.Tunnel(new Command() {
                CommandType = CommandType.SecurityAddGroup,
                Authentication = {
                    Username = "Phogue"
                },
                Origin = CommandOrigin.Remote,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName"
                })
            });

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.InsufficientPermissions, result.CommandResultType);
        }

        /// <summary>
        ///     Tests that permissions will be copied from one group to another.
        /// </summary>
        [Test]
        public void TestSecurityGroupsCopyPermissions() {
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
                CommandType = CommandType.SecurityAddGroup,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "SecondGroupName"
                })
            });

            // Test that the groups were initially added.
            Assert.IsNotNull(security.Groups.FirstOrDefault(group => @group.Name == "FirstGroupName"));
            Assert.IsNotNull(security.Groups.FirstOrDefault(group => @group.Name == "SecondGroupName"));

            // Setup two permissions on the first group.
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupSetPermission,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "FirstGroupName",
                    CommandType.VariablesSet,
                    77
                })
            });
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupSetPermission,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "FirstGroupName",
                    CommandType.VariablesSetA,
                    88
                })
            });

            var firstGroup = security.Groups.FirstOrDefault(group => @group.Name == "FirstGroupName") ?? new GroupModel();
            var secondGroup = security.Groups.FirstOrDefault(group => @group.Name == "SecondGroupName") ?? new GroupModel();

            // Validate original permissions were added.
            Assert.AreEqual(firstGroup.Permissions.First(permission => permission.Name == CommandType.VariablesSet.ToString()).Authority, 77);
            Assert.AreEqual(firstGroup.Permissions.First(permission => permission.Name == CommandType.VariablesSetA.ToString()).Authority, 88);
            Assert.IsNull(secondGroup.Permissions.First(permission => permission.Name == CommandType.VariablesSet.ToString()).Authority);
            Assert.IsNull(secondGroup.Permissions.First(permission => permission.Name == CommandType.VariablesSetA.ToString()).Authority);

            // Now copy the permissions from the first group, to the other group.
            ICommandResult result = security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupCopyPermissions,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "FirstGroupName",
                    "SecondGroupName"
                })
            });

            // Now make sure the user was initially added.
            Assert.IsTrue(result.Success);
            Assert.AreEqual(result.CommandResultType, CommandResultType.Success);
            Assert.AreEqual(secondGroup.Permissions.First(permission => permission.Name == CommandType.VariablesSet.ToString()).Authority, 77);
            Assert.AreEqual(secondGroup.Permissions.First(permission => permission.Name == CommandType.VariablesSetA.ToString()).Authority, 88);
        }

        /// <summary>
        ///     Tests the command to add an account failes if the user has insufficient privileges.
        /// </summary>
        [Test]
        public void TestSecurityGroupsCopyPermissionsInsufficientPermission() {
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
                CommandType = CommandType.SecurityAddGroup,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "SecondGroupName"
                })
            });

            ICommandResult result = security.Tunnel(new Command() {
                CommandType = CommandType.SecurityGroupCopyPermissions,
                Authentication = {
                    Username = "Phogue"
                },
                Origin = CommandOrigin.Remote,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "FirstGroupName",
                    "SecondGroupName"
                })
            });

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.InsufficientPermissions, result.CommandResultType);
        }

        /// <summary>
        ///     Tests that the correct error message is returned when permissions can't be copied from a source group because the source group does not exist.
        /// </summary>
        [Test]
        public void TestSecurityGroupsCopyPermissionsSourceDoesNotExist() {
            var security = new SecurityController();
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAddGroup,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "SecondGroupName"
                })
            });

            // Test that the group was initially added.
            Assert.IsNotNull(security.Groups.FirstOrDefault(group => @group.Name == "SecondGroupName"));

            // Now copy the permissions from the first group, to the other group.
            ICommandResult result = security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupCopyPermissions,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "FirstGroupName",
                    "SecondGroupName"
                })
            });

            Assert.IsFalse(result.Success);
            Assert.AreEqual(result.CommandResultType, CommandResultType.DoesNotExists);
        }

        /// <summary>
        ///     Tests that a custom permission can be set against a group.
        /// </summary>
        [Test]
        public void TestSecurityGroupsSetCustomPermission() {
            var security = new SecurityController();
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAddGroup,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName"
                })
            });

            // Test that the group was initially added.
            Assert.AreEqual(security.Groups.Last().Name, "GroupName");

            // Now set the kick permission
            ICommandResult result = security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupSetPermission,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName",
                    "CustomPermission",
                    50
                })
            });

            // Make sure setting the kick permission was successfull.
            Assert.IsTrue(result.Success);
            Assert.AreEqual(result.CommandResultType, CommandResultType.Success);
            Assert.AreEqual(security.Groups.Last().Permissions.First(permission => permission.Name == "CustomPermission").Authority, 50);
        }

        [Test]
        public void TestSecurityGroupsSetPermission() {
            var security = new SecurityController();
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAddGroup,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName"
                })
            });

            // Test that the group was initially added.
            Assert.AreEqual(security.Groups.Last().Name, "GroupName");

            // Now set the kick permission
            ICommandResult result = security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupSetPermission,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName",
                    CommandType.VariablesSet,
                    50
                })
            });

            // Make sure setting the kick permission was successfull.
            Assert.IsTrue(result.Success);
            Assert.AreEqual(result.CommandResultType, CommandResultType.Success);
            Assert.AreEqual(security.Groups.Last().Permissions.First(permission => permission.Name == CommandType.VariablesSet.ToString()).Authority, 50);
        }

        /// <summary>
        ///     Checks that if a permission is not in the list (usually if we add to CommandName enum) that
        ///     the permission will be created and set.
        /// </summary>
        [Test]
        public void TestSecurityGroupsSetPermissionDynamicallyCreate() {
            var security = new SecurityController();
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAddGroup,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName"
                })
            });

            // Test that the group was initially added.
            Assert.AreEqual(security.Groups.Last().Name, "GroupName");

            // Remove the kick permission.
            security.Groups.Last().Permissions.RemoveAll(permission => permission.Name == CommandType.VariablesSet.ToString());

            // Validate the kick permission does not exist.
            Assert.IsNull(security.Groups.Last().Permissions.FirstOrDefault(permission => permission.Name == CommandType.VariablesSet.ToString()));

            // Now set the kick permission
            ICommandResult result = security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupSetPermission,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName",
                    CommandType.VariablesSet,
                    50
                })
            });

            // Make sure setting the kick permission was successfull.
            Assert.IsTrue(result.Success);
            Assert.AreEqual(result.CommandResultType, CommandResultType.Success);
            Assert.AreEqual(security.Groups.Last().Permissions.First(permission => permission.Name == CommandType.VariablesSet.ToString()).Authority, 50);
        }


        [Test]
        public void TestSecurityGroupsSetPermissionGroupDoesNotExist() {
            var security = new SecurityController();
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAddGroup,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "ThisIsValid"
                })
            });

            // Now set the kick permission
            ICommandResult result = security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupSetPermission,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "ThisIsNotValid",
                    CommandType.VariablesSet,
                    50
                })
            });

            // Make sure the command just nulls out. It couldn't find anything to even try to set the permission.
            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.DoesNotExists, result.CommandResultType);
        }

        /// <summary>
        /// Tests that a user can't revoke their groups permission to set permissions. They will need to get another
        /// group to do this. 
        /// </summary>
        [Test]
        public void TestSecurityGroupsSetPermissionSettingOwnSetPermissionToNothingDenied() {
            var security = new SecurityController();
            security.Tunnel(CommandBuilder.SecurityAddGroup("GroupName").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupAddAccount("GroupName", "Phogue").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityAccountSetPassword("Phogue", "Password").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupSetPermission("GroupName", CommandType.SecurityGroupSetPermission, 50).SetOrigin(CommandOrigin.Local));

            // Now attempt to revoke our permission to set a permission
            ICommandResult result = security.Tunnel(CommandBuilder.SecurityGroupSetPermission("GroupName", CommandType.SecurityGroupSetPermission, 0).SetOrigin(CommandOrigin.Remote).SetAuthentication(new CommandAuthenticationModel() {
                Username = "Phogue",
                PasswordPlainText = "password"
            }));

            // Make sure the command just nulls out. It couldn't find anything to even try to set the permission.
            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.InvalidParameter, result.CommandResultType);
        }

        /// <summary>
        /// Tests that a user can't revoke their groups permission to set authenticate. They will need to get another
        /// group to do this. 
        /// </summary>
        [Test]
        public void TestSecurityGroupsSetPermissionSettingOwnAuthenticateToNothingDenied() {
            var security = new SecurityController();
            security.Tunnel(CommandBuilder.SecurityAddGroup("GroupName").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupAddAccount("GroupName", "Phogue").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityAccountSetPassword("Phogue", "Password").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupSetPermission("GroupName", CommandType.SecurityGroupSetPermission, 50).SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupSetPermission("GroupName", CommandType.SecurityAccountAuthenticate, 50).SetOrigin(CommandOrigin.Local));

            // Now attempt to revoke our permission to authenticate with the system
            ICommandResult result = security.Tunnel(CommandBuilder.SecurityGroupSetPermission("GroupName", CommandType.SecurityAccountAuthenticate, 0).SetOrigin(CommandOrigin.Remote).SetAuthentication(new CommandAuthenticationModel() {
                Username = "Phogue",
                PasswordPlainText = "password"
            }));

            // Make sure the command just nulls out. It couldn't find anything to even try to set the permission.
            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.InvalidParameter, result.CommandResultType);
        }

        /// <summary>
        /// Tests the command to add an account failes if the user has insufficient privileges.
        /// </summary>
        [Test]
        public void TestSecurityGroupsSetPermissionInsufficientPermission() {
            var security = new SecurityController();
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAddGroup,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName"
                })
            });

            ICommandResult result = security.Tunnel(new Command() {
                CommandType = CommandType.SecurityGroupSetPermission,
                Authentication = {
                    Username = "Phogue"
                },
                Origin = CommandOrigin.Remote,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName",
                    CommandType.VariablesSet,
                    50
                })
            });

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.InsufficientPermissions, result.CommandResultType);
        }

        /// <summary>
        /// Testing if "Kick" will be converted since this value will be serialized.
        /// </summary>
        [Test]
        public void TestSecurityGroupsSetPermissionTypeConvert() {
            var security = new SecurityController();
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAddGroup,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName"
                })
            });

            // Test that the group was initially added.
            Assert.AreEqual(security.Groups.Last().Name, "GroupName");

            // Now set the kick permission
            ICommandResult result = security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupSetPermission,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName",
                    CommandType.VariablesSet.ToString(),
                    60
                })
            });

            // Make sure setting the kick permission was successfull.
            Assert.IsTrue(result.Success);
            Assert.AreEqual(result.CommandResultType, CommandResultType.Success);
            Assert.AreEqual(security.Groups.Last().Permissions.First(permission => permission.Name == CommandType.VariablesSet.ToString()).Authority, 60);
        }

        /// <summary>
        ///     Testing that a group can be removed by its name.
        /// </summary>
        [Test]
        public void TestSecurityRemoveGroup() {
            var security = new SecurityController();
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAddGroup,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName"
                })
            });

            // Test that the group was initially added.
            Assert.AreEqual(security.Groups.Last().Name, "GroupName");

            ICommandResult result = security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityRemoveGroup,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName"
                })
            });

            // Make sure it was successful.
            Assert.IsTrue(result.Success);
            Assert.AreEqual(security.Groups.Count, 1);
        }

        /// <summary>
        /// Tests that a group can be removed if executed locally
        /// </summary>
        [Test]
        public void TestRemoveGroupByLocalSuccess() {
            var security = new SecurityController();
            security.Tunnel(CommandBuilder.SecurityAddGroup("GroupName").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupAddAccount("GroupName", "Phogue").SetOrigin(CommandOrigin.Local));

            ICommandResult result = security.Tunnel(CommandBuilder.SecurityRemoveGroup("GroupName").SetOrigin(CommandOrigin.Local));

            // Make sure it was successful.
            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Success, result.CommandResultType);
        }

        /// <summary>
        /// Tests that a user cannot remove their own group
        /// </summary>
        [Test]
        public void TestRemoveOwnGroupFailure() {
            var security = new SecurityController();
            security.Tunnel(CommandBuilder.SecurityAddGroup("GroupName").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupAddAccount("GroupName", "Phogue").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupSetPermission("GroupName", CommandType.SecurityRemoveGroup, 1).SetOrigin(CommandOrigin.Local));

            ICommandResult result = security.Tunnel(CommandBuilder.SecurityRemoveGroup("GroupName").SetOrigin(CommandOrigin.Remote).SetAuthentication(new CommandAuthenticationModel() {
                Username = "Phogue"
            }));

            // Make sure it was successful.
            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.InvalidParameter, result.CommandResultType);
        }

        /// <summary>
        /// Tests that removing the guest account will fail.
        /// </summary>
        [Test]
        public void TestSecurityRemoveGuestGroup() {
            var security = new SecurityController();

            ICommandResult result = security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityRemoveGroup,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "Guest"
                })
            });

            // Make sure it was successful.
            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.InvalidParameter, result.CommandResultType);
            Assert.AreEqual(security.Groups.Count, 1);
        }

        /// <summary>
        ///     Testing that you can't remove a group name that is empty.
        /// </summary>
        [Test]
        public void TestSecurityRemoveGroupEmptyGroupName() {
            var security = new SecurityController();

            // Add a group with an empty name.
            ICommandResult result = security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityRemoveGroup,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    String.Empty
                })
            });

            // Make sure adding an empty group fails.
            Assert.IsFalse(result.Success);
            Assert.AreEqual(result.CommandResultType, CommandResultType.InvalidParameter);
        }

        [Test]
        public void TestSecurityRemoveGroupInsufficientPermission() {
            var security = new SecurityController();
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAddGroup,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName"
                })
            });

            ICommandResult result = security.Tunnel(new Command() {
                CommandType = CommandType.SecurityRemoveGroup,
                Authentication = {
                    Username = "Phogue"
                },
                Origin = CommandOrigin.Remote,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName"
                })
            });

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.InsufficientPermissions, result.CommandResultType);
        }

        /// <summary>
        ///     Testing that you can't remove a group that does not exist and provides correct errors.
        /// </summary>
        [Test]
        public void TestSecurityRemoveGroupNotExists() {
            var security = new SecurityController();
            ICommandResult result = security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityRemoveGroup,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName"
                })
            });

            // Make sure it was not successful.
            Assert.IsFalse(result.Success);
            Assert.AreEqual(result.CommandResultType, CommandResultType.DoesNotExists);
        }
    }
}