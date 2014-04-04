using System.Linq;
using NUnit.Framework;
using Procon.Core.Security;
using Procon.Core.Shared;
using Procon.Core.Shared.Models;

namespace Procon.Core.Test.Security.Permission {
    [TestFixture]
    public class TestSecurityGroupSetPermissionDescription {
        /// <summary>
        /// Tests that a remote call with no permissions will result in an InsufficientPermissions status
        /// </summary>
        [Test]
        public void TestInsufficientPermissions() {
            var security = new SecurityController();

            security.Tunnel(CommandBuilder.SecurityAddGroup("GroupName").SetOrigin(CommandOrigin.Local));

            ICommandResult result = security.Tunnel(CommandBuilder.SecurityGroupSetPermissionDescription("GroupName", CommandType.VariablesSet.ToString(), "Description!").SetOrigin(CommandOrigin.Remote).SetAuthentication(new CommandAuthenticationModel() {
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

            ICommandResult result = security.Tunnel(CommandBuilder.SecurityGroupSetPermissionDescription("ThisIsNotValid", CommandType.VariablesSet.ToString(), "Description!").SetOrigin(CommandOrigin.Local));

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

            ICommandResult result = security.Tunnel(CommandBuilder.SecurityGroupSetPermissionDescription("GroupName", CommandType.VariablesSet.ToString(), "Description!").SetOrigin(CommandOrigin.Local));

            // Make sure it was not successful.
            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Success, result.CommandResultType);
        }

        /// <summary>
        /// Tests that a description is appended to the list for a permission
        /// </summary>
        [Test]
        public void TestPermissionsdescriptionAppended() {
            var security = new SecurityController();

            security.Tunnel(CommandBuilder.SecurityAddGroup("GroupName").SetOrigin(CommandOrigin.Local));

            security.Tunnel(CommandBuilder.SecurityGroupSetPermissionDescription("GroupName", CommandType.VariablesSet.ToString(), "Description!").SetOrigin(CommandOrigin.Local));

            var group = security.Groups.First(item => item.Name == "GroupName");

            Assert.AreEqual("Description!", group.Permissions.First(permission => permission.CommandType == CommandType.VariablesSet).Description);
        }

        /// <summary>
        /// Tests custom description on custom permission appended
        /// </summary>
        [Test]
        public void TestCustomPermissionsCustomdescriptionAppended() {
            var security = new SecurityController();

            security.Tunnel(CommandBuilder.SecurityAddGroup("GroupName").SetOrigin(CommandOrigin.Local));

            security.Tunnel(CommandBuilder.SecurityGroupSetPermissionDescription("GroupName", "Custom Permission", "Description!").SetOrigin(CommandOrigin.Local));

            var group = security.Groups.First(item => item.Name == "GroupName");

            Assert.AreEqual("Description!", group.Permissions.First(permission => permission.Name == "Custom Permission").Description);
        }
    }
}
