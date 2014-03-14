using NUnit.Framework;
using Procon.Core.Security;
using Procon.Core.Shared;

namespace Procon.Core.Test.Security {
    [TestFixture]
    public class TestDispatchGroupCheck {
        /// <summary>
        /// Tests that a single account's group will match if they are identical.
        /// </summary>
        [Test]
        public void TestCheckByAccountUsernameIsIdentical() {
            var security = new SecurityController();
            security.Tunnel(CommandBuilder.SecurityAddGroup("GroupName").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupAddAccount("GroupName", "Phogue").SetOrigin(CommandOrigin.Local));

            // Now change the language of the account.
            ICommandResult result = security.DispatchGroupCheck(new Command() {
                Authentication = {
                    Username = "Phogue"
                }
            }, "GroupName");

            // Make sure it was successful.
            Assert.IsTrue(result.Success);
            Assert.AreEqual(result.CommandResultType, CommandResultType.Success);
        }

        /// <summary>
        /// Tests that the the comparison fails against a different group to the executor
        /// </summary>
        [Test]
        public void TestCheckByAccountUsernameIsNotIdentical() {
            var security = new SecurityController();
            security.Tunnel(CommandBuilder.SecurityAddGroup("FirstGroupName").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupAddAccount("FirstGroupName", "Phogue").SetOrigin(CommandOrigin.Local));

            security.Tunnel(CommandBuilder.SecurityAddGroup("SecondGroupName").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupAddAccount("GroupName", "Ike").SetOrigin(CommandOrigin.Local));

            // Now change the language of the account.
            ICommandResult result = security.DispatchGroupCheck(new Command() {
                Authentication = {
                    Username = "Phogue"
                }
            }, "SecondGroupName");

            // Make sure it was successful.
            Assert.IsFalse(result.Success);
            Assert.AreEqual(result.CommandResultType, CommandResultType.Failed);
        }

        /// <summary>
        /// Tests that the account will not be identical if the target account does not exist
        /// </summary>
        [Test]
        public void TestCheckByAccountUsernameFailsOnTargetAccountNotExist() {
            var security = new SecurityController();
            security.Tunnel(CommandBuilder.SecurityAddGroup("GroupName").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupAddAccount("GroupName", "Phogue").SetOrigin(CommandOrigin.Local));

            // Now change the language of the account.
            ICommandResult result = security.DispatchGroupCheck(new Command() {
                Authentication = {
                    Username = "Phogue"
                }
            }, "AccountDoesNotExist");

            // Make sure it was successful.
            Assert.IsFalse(result.Success);
            Assert.AreEqual(result.CommandResultType, CommandResultType.Failed);
        }

        /// <summary>
        /// Tests that the account will not be identical if the source account does not exist
        /// </summary>
        [Test]
        public void TestCheckByAccountUsernameFailsOnSourceAccountNotExist() {
            var security = new SecurityController();
            security.Tunnel(CommandBuilder.SecurityAddGroup("GroupName").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupAddAccount("GroupName", "Phogue").SetOrigin(CommandOrigin.Local));

            // Now change the language of the account.
            ICommandResult result = security.DispatchGroupCheck(new Command() {
                Authentication = {
                    Username = "AccountDoesNotExist"
                }
            }, "GroupName");

            // Make sure it was successful.
            Assert.IsFalse(result.Success);
            Assert.AreEqual(result.CommandResultType, CommandResultType.Failed);
        }

    }
}