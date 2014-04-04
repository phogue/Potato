using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Procon.Core.Security;
using Procon.Core.Shared;
using Procon.Core.Shared.Models;

namespace Procon.Core.Test.Security.Permission {
    [TestFixture]
    public class TestSecurityGroupRemovePermissionTrait {
        /// <summary>
        /// Tests that a remote call with no permissions will result in an InsufficientPermissions status
        /// </summary>
        [Test]
        public void TestInsufficientPermissions() {
            var security = new SecurityController();

            security.Tunnel(CommandBuilder.SecurityAddGroup("GroupName").SetOrigin(CommandOrigin.Local));

            ICommandResult result = security.Tunnel(CommandBuilder.SecurityGroupRemovePermissionTrait("GroupName", CommandType.VariablesSet.ToString(), PermissionTraitsType.Boolean).SetOrigin(CommandOrigin.Remote).SetAuthentication(new CommandAuthenticationModel() {
                Username = "Phogue"
            }));

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.InsufficientPermissions, result.CommandResultType);
        }

        /// <summary>
        /// Tests that attempting to set a group that does not exist to administrator permissions will fail
        /// with a status of "DoesNotExist"
        /// </summary>
        [Test]
        public void TestGroupDoesNotExist() {
            var security = new SecurityController();
            security.Tunnel(CommandBuilder.SecurityAddGroup("ThisIsValid").SetOrigin(CommandOrigin.Local));

            ICommandResult result = security.Tunnel(CommandBuilder.SecurityGroupRemovePermissionTrait("ThisIsNotValid", CommandType.VariablesSet.ToString(), PermissionTraitsType.Boolean).SetOrigin(CommandOrigin.Local));

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.DoesNotExists, result.CommandResultType);
        }

        /// <summary>
        /// Tests that setting the permissions of the group to administrator will succeed 
        /// </summary>
        [Test]
        public void TestSuccess() {
            var security = new SecurityController();

            security.Tunnel(CommandBuilder.SecurityAddGroup("GroupName").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupAppendPermissionTrait("GroupName", CommandType.VariablesSet.ToString(), PermissionTraitsType.Boolean).SetOrigin(CommandOrigin.Local));

            ICommandResult result = security.Tunnel(CommandBuilder.SecurityGroupRemovePermissionTrait("GroupName", CommandType.VariablesSet.ToString(), PermissionTraitsType.Boolean).SetOrigin(CommandOrigin.Local));

            // Make sure it was not successful.
            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Success, result.CommandResultType);
        }

        /// <summary>
        /// Tests that a trait is removed to the list for a permission
        /// </summary>
        [Test]
        public void TestPermissionsTraitRemoved() {
            var security = new SecurityController();

            security.Tunnel(CommandBuilder.SecurityAddGroup("GroupName").SetOrigin(CommandOrigin.Local));

            security.Tunnel(CommandBuilder.SecurityGroupAppendPermissionTrait("GroupName", CommandType.VariablesSet.ToString(), PermissionTraitsType.Boolean).SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupRemovePermissionTrait("GroupName", CommandType.VariablesSet.ToString(), PermissionTraitsType.Boolean).SetOrigin(CommandOrigin.Local));
            
            var group = security.Groups.First(item => item.Name == "GroupName");

            Assert.AreEqual(new List<String>(), group.Permissions.First(permission => permission.CommandType == CommandType.VariablesSet).Traits);
        }

        /// <summary>
        /// Tests that removing a trait twice will still be successful (no errors or funny business)
        /// </summary>
        [Test]
        public void TestPermissionsTraitRemovedAlreadyRemoved() {
            var security = new SecurityController();

            security.Tunnel(CommandBuilder.SecurityAddGroup("GroupName").SetOrigin(CommandOrigin.Local));

            security.Tunnel(CommandBuilder.SecurityGroupAppendPermissionTrait("GroupName", CommandType.VariablesSet.ToString(), PermissionTraitsType.Boolean).SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupRemovePermissionTrait("GroupName", CommandType.VariablesSet.ToString(), PermissionTraitsType.Boolean).SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupRemovePermissionTrait("GroupName", CommandType.VariablesSet.ToString(), PermissionTraitsType.Boolean).SetOrigin(CommandOrigin.Local));

            var group = security.Groups.First(item => item.Name == "GroupName");

            Assert.AreEqual(new List<String>(), group.Permissions.First(permission => permission.CommandType == CommandType.VariablesSet).Traits);
        }

        /// <summary>
        /// Tests custom trait removed
        /// </summary>
        [Test]
        public void TestPermissionsCustomTraitAppended() {
            var security = new SecurityController();

            security.Tunnel(CommandBuilder.SecurityAddGroup("GroupName").SetOrigin(CommandOrigin.Local));

            security.Tunnel(CommandBuilder.SecurityGroupAppendPermissionTrait("GroupName", CommandType.VariablesSet.ToString(), "Custom Trait").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupRemovePermissionTrait("GroupName", CommandType.VariablesSet.ToString(), "Custom Trait").SetOrigin(CommandOrigin.Local));

            var group = security.Groups.First(item => item.Name == "GroupName");

            Assert.AreEqual(new List<String>(), group.Permissions.First(permission => permission.CommandType == CommandType.VariablesSet).Traits);
        }

        /// <summary>
        /// Tests custom trait on custom permission appended
        /// </summary>
        [Test]
        public void TestCustomPermissionsCustomTraitAppended() {
            var security = new SecurityController();

            security.Tunnel(CommandBuilder.SecurityAddGroup("GroupName").SetOrigin(CommandOrigin.Local));

            security.Tunnel(CommandBuilder.SecurityGroupAppendPermissionTrait("GroupName", "Custom Permission", "Custom Trait").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupRemovePermissionTrait("GroupName", "Custom Permission", "Custom Trait").SetOrigin(CommandOrigin.Local));

            var group = security.Groups.First(item => item.Name == "GroupName");

            Assert.AreEqual(new List<String>(), group.Permissions.First(permission => permission.Name == "Custom Permission").Traits);
        }
    }
}
