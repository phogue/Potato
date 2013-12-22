#region

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Procon.Core.Security;
using Procon.Core.Shared;

#endregion

namespace Procon.Core.Test.Security {
    [TestFixture]
    public class TestSecurityGroup {
        /// <summary>
        ///     Testing that we can add a simple group.
        /// </summary>
        [Test]
        public void TestSecurityAddGroup() {
            var security = new SecurityController();

            // Add a group.
            CommandResultArgs result = security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAddGroup,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName"
                })
            });

            // Make sure it was successful.
            Assert.IsTrue(result.Success);
            Assert.AreEqual(security.Groups.First().Name, "GroupName");
        }

        /// <summary>
        ///     Testing that two groups with identical names can't be added to the security model.
        /// </summary>
        [Test]
        public void TestSecurityAddGroupDuplicateGroupName() {
            var security = new SecurityController();
            CommandResultArgs result = security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAddGroup,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName"
                })
            });

            // Test that the group was initially added.
            Assert.AreEqual(security.Groups.First().Name, "GroupName");

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
            Assert.AreEqual(result.Status, CommandResultType.AlreadyExists);
        }

        /// <summary>
        ///     Testing that a group with an empty name can't be added to the security model.
        /// </summary>
        [Test]
        public void TestSecurityAddGroupEmptyGroupName() {
            var security = new SecurityController();

            // Add a group with an empty name.
            CommandResultArgs result = security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAddGroup,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    String.Empty
                })
            });

            // Make sure adding an empty group fails.
            Assert.IsFalse(result.Success);
            Assert.AreEqual(result.Status, CommandResultType.InvalidParameter);
        }

        /// <summary>
        ///     Tests that a group cannot be added unless the user has sufficient permissions to do so.
        /// </summary>
        [Test]
        public void TestSecurityAddGroupInsufficientPermission() {
            var security = new SecurityController();

            // Add a group.
            CommandResultArgs result = security.Tunnel(new Command() {
                CommandType = CommandType.SecurityAddGroup,
                Username = "Phogue",
                Origin = CommandOrigin.Remote,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName"
                })
            });

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.InsufficientPermissions, result.Status);
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
            Assert.IsNotNull(security.Groups.Where(group => group.Name == "FirstGroupName").FirstOrDefault());
            Assert.IsNotNull(security.Groups.Where(group => group.Name == "SecondGroupName").FirstOrDefault());

            // Setup two permissions on the first group.
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupSetPermission,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "FirstGroupName",
                    CommandType.NetworkProtocolActionKick,
                    77
                })
            });
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupSetPermission,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "FirstGroupName",
                    CommandType.NetworkProtocolActionBan,
                    88
                })
            });

            // Validate original permissions were added.
            Assert.AreEqual(security.Groups.Where(group => group.Name == "FirstGroupName").FirstOrDefault().Permissions.Where(permission => permission.Name == CommandType.NetworkProtocolActionKick.ToString()).First().Authority, 77);
            Assert.AreEqual(security.Groups.Where(group => group.Name == "FirstGroupName").FirstOrDefault().Permissions.Where(permission => permission.Name == CommandType.NetworkProtocolActionBan.ToString()).First().Authority, 88);
            Assert.IsNull(security.Groups.Where(group => group.Name == "SecondGroupName").FirstOrDefault().Permissions.Where(permission => permission.Name == CommandType.NetworkProtocolActionKick.ToString()).First().Authority);
            Assert.IsNull(security.Groups.Where(group => group.Name == "SecondGroupName").FirstOrDefault().Permissions.Where(permission => permission.Name == CommandType.NetworkProtocolActionBan.ToString()).First().Authority);

            // Now copy the permissions from the first group, to the other group.
            CommandResultArgs result = security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupCopyPermissions,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "FirstGroupName",
                    "SecondGroupName"
                })
            });

            // Now make sure the user was initially added.
            Assert.IsTrue(result.Success);
            Assert.AreEqual(result.Status, CommandResultType.Success);
            Assert.AreEqual(security.Groups.Where(group => group.Name == "SecondGroupName").FirstOrDefault().Permissions.Where(permission => permission.Name == CommandType.NetworkProtocolActionKick.ToString()).First().Authority, 77);
            Assert.AreEqual(security.Groups.Where(group => group.Name == "SecondGroupName").FirstOrDefault().Permissions.Where(permission => permission.Name == CommandType.NetworkProtocolActionBan.ToString()).First().Authority, 88);
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

            CommandResultArgs result = security.Tunnel(new Command() {
                CommandType = CommandType.SecurityGroupCopyPermissions,
                Username = "Phogue",
                Origin = CommandOrigin.Remote,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "FirstGroupName",
                    "SecondGroupName"
                })
            });

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.InsufficientPermissions, result.Status);
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
            Assert.IsNotNull(security.Groups.Where(group => group.Name == "SecondGroupName").FirstOrDefault());

            // Now copy the permissions from the first group, to the other group.
            CommandResultArgs result = security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupCopyPermissions,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "FirstGroupName",
                    "SecondGroupName"
                })
            });

            Assert.IsFalse(result.Success);
            Assert.AreEqual(result.Status, CommandResultType.DoesNotExists);
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
            Assert.AreEqual(security.Groups.First().Name, "GroupName");

            // Now set the kick permission
            CommandResultArgs result = security.Tunnel(new Command() {
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
            Assert.AreEqual(result.Status, CommandResultType.Success);
            Assert.AreEqual(security.Groups.First().Permissions.Where(permission => permission.Name == "CustomPermission").First().Authority, 50);
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
            Assert.AreEqual(security.Groups.First().Name, "GroupName");

            // Now set the kick permission
            CommandResultArgs result = security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupSetPermission,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName",
                    CommandType.NetworkProtocolActionKick,
                    50
                })
            });

            // Make sure setting the kick permission was successfull.
            Assert.IsTrue(result.Success);
            Assert.AreEqual(result.Status, CommandResultType.Success);
            Assert.AreEqual(security.Groups.First().Permissions.Where(permission => permission.Name == CommandType.NetworkProtocolActionKick.ToString()).First().Authority, 50);
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
            Assert.AreEqual(security.Groups.First().Name, "GroupName");

            // Remove the kick permission.
            security.Groups.First().Permissions.RemoveAll(permission => permission.Name == CommandType.NetworkProtocolActionKick.ToString());

            // Validate the kick permission does not exist.
            Assert.IsNull(security.Groups.First().Permissions.FirstOrDefault(permission => permission.Name == CommandType.NetworkProtocolActionKick.ToString()));

            // Now set the kick permission
            CommandResultArgs result = security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupSetPermission,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName",
                    CommandType.NetworkProtocolActionKick,
                    50
                })
            });

            // Make sure setting the kick permission was successfull.
            Assert.IsTrue(result.Success);
            Assert.AreEqual(result.Status, CommandResultType.Success);
            Assert.AreEqual(security.Groups.First().Permissions.Where(permission => permission.Name == CommandType.NetworkProtocolActionKick.ToString()).First().Authority, 50);
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
            CommandResultArgs result = security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupSetPermission,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "ThisIsNotValid",
                    CommandType.NetworkProtocolActionKick,
                    50
                })
            });

            // Make sure the command just nulls out. It couldn't find anything to even try to set the permission.
            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.DoesNotExists, result.Status);
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

            CommandResultArgs result = security.Tunnel(new Command() {
                CommandType = CommandType.SecurityGroupSetPermission,
                Username = "Phogue",
                Origin = CommandOrigin.Remote,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName",
                    CommandType.NetworkProtocolActionKick,
                    50
                })
            });

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.InsufficientPermissions, result.Status);
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
            Assert.AreEqual(security.Groups.First().Name, "GroupName");

            // Now set the kick permission
            CommandResultArgs result = security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupSetPermission,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName",
                    "NetworkProtocolActionKick",
                    60
                })
            });

            // Make sure setting the kick permission was successfull.
            Assert.IsTrue(result.Success);
            Assert.AreEqual(result.Status, CommandResultType.Success);
            Assert.AreEqual(security.Groups.First().Permissions.First(permission => permission.Name == CommandType.NetworkProtocolActionKick.ToString()).Authority, 60);
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
            Assert.AreEqual(security.Groups.First().Name, "GroupName");

            CommandResultArgs result = security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityRemoveGroup,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName"
                })
            });

            // Make sure it was successful.
            Assert.IsTrue(result.Success);
            Assert.AreEqual(security.Groups.Count, 0);
        }

        /// <summary>
        ///     Testing that you can't remove a group name that is empty.
        /// </summary>
        [Test]
        public void TestSecurityRemoveGroupEmptyGroupName() {
            var security = new SecurityController();

            // Add a group with an empty name.
            CommandResultArgs result = security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityRemoveGroup,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    String.Empty
                })
            });

            // Make sure adding an empty group fails.
            Assert.IsFalse(result.Success);
            Assert.AreEqual(result.Status, CommandResultType.InvalidParameter);
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

            CommandResultArgs result = security.Tunnel(new Command() {
                CommandType = CommandType.SecurityRemoveGroup,
                Username = "Phogue",
                Origin = CommandOrigin.Remote,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName"
                })
            });

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.InsufficientPermissions, result.Status);
        }

        /// <summary>
        ///     Testing that you can't remove a group that does not exist and provides correct errors.
        /// </summary>
        [Test]
        public void TestSecurityRemoveGroupNotExists() {
            var security = new SecurityController();
            CommandResultArgs result = security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityRemoveGroup,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName"
                })
            });

            // Make sure it was not successful.
            Assert.IsFalse(result.Success);
            Assert.AreEqual(result.Status, CommandResultType.DoesNotExists);
        }
    }
}