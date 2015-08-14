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
using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Potato.Core.Security;
using Potato.Core.Shared;
using Potato.Core.Shared.Models;

namespace Potato.Core.Test.Security.Permission {
    [TestFixture]
    public class TestSecurityGroupRemovePermissionTrait {
        /// <summary>
        /// Tests that a remote call with no permissions will result in an InsufficientPermissions status
        /// </summary>
        [Test]
        public void TestInsufficientPermissions() {
            var security = new SecurityController();

            security.Tunnel(CommandBuilder.SecurityAddGroup("GroupName").SetOrigin(CommandOrigin.Local));

            var result = security.Tunnel(CommandBuilder.SecurityGroupRemovePermissionTrait("GroupName", CommandType.VariablesSet.ToString(), PermissionTraitsType.Boolean).SetOrigin(CommandOrigin.Remote).SetAuthentication(new CommandAuthenticationModel() {
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

            var result = security.Tunnel(CommandBuilder.SecurityGroupRemovePermissionTrait("ThisIsNotValid", CommandType.VariablesSet.ToString(), PermissionTraitsType.Boolean).SetOrigin(CommandOrigin.Local));

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

            var result = security.Tunnel(CommandBuilder.SecurityGroupRemovePermissionTrait("GroupName", CommandType.VariablesSet.ToString(), PermissionTraitsType.Boolean).SetOrigin(CommandOrigin.Local));

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

            Assert.AreEqual(new List<string>(), group.Permissions.First(permission => permission.CommandType == CommandType.VariablesSet).Traits);
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

            Assert.AreEqual(new List<string>(), group.Permissions.First(permission => permission.CommandType == CommandType.VariablesSet).Traits);
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

            Assert.AreEqual(new List<string>() { PermissionTraitsType.Boolean }, group.Permissions.First(permission => permission.CommandType == CommandType.VariablesSet).Traits);
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

            Assert.AreEqual(new List<string>(), group.Permissions.First(permission => permission.Name == "Custom Permission").Traits);
        }
    }
}
