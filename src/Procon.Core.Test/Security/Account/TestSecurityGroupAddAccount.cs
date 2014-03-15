using System;
using System.Linq;
using NUnit.Framework;
using Procon.Core.Security;
using Procon.Core.Shared;
using Procon.Core.Shared.Models;

namespace Procon.Core.Test.Security.Account {
    [TestFixture]
    public class TestSecurityGroupAddAccount {
        [SetUp]
        public void Initialize() {
            SharedReferences.Setup();
        }

        /// <summary>
        ///     Tests that adding a simpel account can be completed.
        /// </summary>
        [Test]
        public void TestAddSuccess() {
            var security = new SecurityController();
            security.Tunnel(CommandBuilder.SecurityAddGroup("GroupName").SetOrigin(CommandOrigin.Local));

            // Now add the user.
            ICommandResult result = security.Tunnel(CommandBuilder.SecurityGroupAddAccount("GroupName", "Phogue").SetOrigin(CommandOrigin.Local));

            // Make sure the account was successfully created.
            Assert.IsTrue(result.Success);
            Assert.AreEqual(result.CommandResultType, CommandResultType.Success);
            Assert.AreEqual(security.Groups.SelectMany(group => group.Accounts).First().Username, "Phogue");
        }

        /// <summary>
        ///     Tests that an account cannot be added if the username is empty.
        /// </summary>
        [Test]
        public void TestEmptyUsername() {
            var security = new SecurityController();
            security.Tunnel(CommandBuilder.SecurityAddGroup("GroupName").SetOrigin(CommandOrigin.Local));

            // Now add the user.
            ICommandResult result = security.Tunnel(CommandBuilder.SecurityGroupAddAccount("GroupName", String.Empty).SetOrigin(CommandOrigin.Local));

            // Make sure the account was successfully created.
            Assert.IsFalse(result.Success);
            Assert.AreEqual(result.CommandResultType, CommandResultType.InvalidParameter);
        }

        /// <summary>
        ///     Tests that adding an account with a duplicate username will move the user to the new group.
        /// </summary>
        [Test]
        public void TestExistingName() {
            var security = new SecurityController();
            security.Tunnel(CommandBuilder.SecurityAddGroup("FirstGroupName").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityAddGroup("SecondGroupName").SetOrigin(CommandOrigin.Local));

            // Now add the user.
            security.Tunnel(CommandBuilder.SecurityGroupAddAccount("FirstGroupName", "Phogue").SetOrigin(CommandOrigin.Local));

            // Now move the user to the second group.
            ICommandResult result = security.Tunnel(CommandBuilder.SecurityGroupAddAccount("SecondGroupName", "Phogue").SetOrigin(CommandOrigin.Local));

            // Make sure setting the kick permission was successfull.
            Assert.IsTrue(result.Success);
            Assert.AreEqual(result.CommandResultType, CommandResultType.Success);
            Assert.IsNull(security.Groups.Where(group => group.Name == "FirstGroupName").SelectMany(group => group.Accounts).FirstOrDefault());
            Assert.AreEqual(security.Groups.Where(group => group.Name == "SecondGroupName").SelectMany(group => group.Accounts).First().Username, "Phogue");
        }

        /// <summary>
        ///     Tests that adding an account with a duplicate username will move the user to the new group,
        /// even if the supplied move name has a different case to the original name
        /// </summary>
        [Test]
        public void TestExistingNameCaseInsensitive() {
            var security = new SecurityController();
            security.Tunnel(CommandBuilder.SecurityAddGroup("FirstGroupName").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityAddGroup("SecondGroupName").SetOrigin(CommandOrigin.Local));

            // Now add the user.
            security.Tunnel(CommandBuilder.SecurityGroupAddAccount("FirstGroupName", "Phogue").SetOrigin(CommandOrigin.Local));

            // Now move the user to the second group.
            ICommandResult result = security.Tunnel(CommandBuilder.SecurityGroupAddAccount("SecondGroupName", "PHOGUE").SetOrigin(CommandOrigin.Local));

            // Make sure setting the kick permission was successfull.
            Assert.IsTrue(result.Success);
            Assert.AreEqual(result.CommandResultType, CommandResultType.Success);
            Assert.IsNull(security.Groups.Where(group => group.Name == "FirstGroupName").SelectMany(group => group.Accounts).FirstOrDefault());
            Assert.AreEqual(security.Groups.Where(group => group.Name == "SecondGroupName").SelectMany(group => group.Accounts).First().Username, "Phogue");
        }

        /// <summary>
        ///     Tests that adding a simpel account can be completed.
        /// </summary>
        [Test]
        public void TestGroupDoesNotExist() {
            var security = new SecurityController();

            // Add the user.
            ICommandResult result = security.Tunnel(CommandBuilder.SecurityGroupAddAccount("NonExistentGroup", "Phogue").SetOrigin(CommandOrigin.Local));

            // Make sure the command returned nothing
            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.DoesNotExists, result.CommandResultType);
        }

        /// <summary>
        /// Tests that adding an account to a guest group will fail.
        /// </summary>
        [Test]
        public void TestGroupGuest() {
            var security = new SecurityController();

            // Add the user.
            ICommandResult result = security.Tunnel(CommandBuilder.SecurityGroupAddAccount("Guest", "Phogue").SetOrigin(CommandOrigin.Local));

            // Make sure the command returned nothing
            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.InvalidParameter, result.CommandResultType);
        }

        /// <summary>
        ///     Tests the command to add an account failes if the user has insufficient privileges.
        /// </summary>
        [Test]
        public void TestInsufficientPermission() {
            var security = new SecurityController();
            security.Tunnel(CommandBuilder.SecurityAddGroup("GroupName").SetOrigin(CommandOrigin.Local));

            ICommandResult result = security.Tunnel(CommandBuilder.SecurityGroupAddAccount("GroupName", "Phogue")
                .SetOrigin(CommandOrigin.Remote)
                .SetAuthentication(new CommandAuthenticationModel() {
                    Username = "Phogue"
                })
            );

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.InsufficientPermissions, result.CommandResultType);
        }
    }
}
