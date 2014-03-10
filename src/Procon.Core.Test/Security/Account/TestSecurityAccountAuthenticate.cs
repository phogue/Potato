using System;
using NUnit.Framework;
using Procon.Core.Security;
using Procon.Core.Shared;
using Procon.Core.Shared.Models;
using Procon.Net.Shared.Utils;

namespace Procon.Core.Test.Security.Account {
    [TestFixture]
    public class Test {
        [SetUp]
        public void Initialize() {
            SharedReferences.Setup();
        }

        /// <summary>
        ///     Tests that authentication returns nothing if an account does not exist.
        /// </summary>
        [Test]
        public void TestAccountDoesNotExist() {
            String generatedAuthenticatePassword = StringExtensions.RandomString(10);

            var security = new SecurityController();
            security.Tunnel(CommandBuilder.SecurityAddGroup("GroupName").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupAddAccount("GroupName", "ThisExists").SetOrigin(CommandOrigin.Local));

            // Now authenticate against an empty security object which has no accounts.
            ICommandResult result = security.Tunnel(CommandBuilder.SecurityAccountAuthenticate("DoesNotExist", generatedAuthenticatePassword).SetOrigin(CommandOrigin.Local));

            // Validate that we get nothing back.
            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.DoesNotExists, result.Status);
        }

        /// <summary>
        ///     Test that setting a new password can be authenticated against
        /// </summary>
        [Test]
        public void TestIncorrectPassword() {
            String generatedSetPassword = StringExtensions.RandomString(10);
            String generatedAuthenticatePassword = StringExtensions.RandomString(10);

            var security = new SecurityController();
            security.Tunnel(CommandBuilder.SecurityAddGroup("GroupName").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupAddAccount("GroupName", "Phogue").SetOrigin(CommandOrigin.Local));

            // Now change the password of the account.
            security.Tunnel(CommandBuilder.SecurityAccountSetPassword("Phogue", generatedSetPassword).SetOrigin(CommandOrigin.Local));

            // Now validate that we can authenticate against the newly set password.
            ICommandResult result = security.Tunnel(CommandBuilder.SecurityAccountAuthenticate("Phogue", generatedAuthenticatePassword).SetOrigin(CommandOrigin.Local));

            // Validate that we could authenticate with our new password.
            Assert.IsFalse(result.Success);
            Assert.AreEqual(result.Status, CommandResultType.Failed);
        }

        /// <summary>
        ///     Tests that the account cannot be authenticated against if the account does not have permissions
        ///     to authenticate in the first place.
        /// </summary>
        [Test]
        public void TestInsufficientPermission() {
            var security = new SecurityController();
            security.Tunnel(CommandBuilder.SecurityAddGroup("GroupName").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupAddAccount("GroupName", "Phogue").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityAccountSetPassword("Phogue", "password").SetOrigin(CommandOrigin.Local));

            ICommandResult result = security.Tunnel(CommandBuilder.SecurityAccountAuthenticate("Phogue", "password").SetOrigin(CommandOrigin.Local).SetOrigin(CommandOrigin.Remote).SetAuthentication(new CommandAuthenticationModel() {
                Username = "Phogue"
            }));

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.InsufficientPermissions, result.Status);
        }

        /// <summary>
        ///     Validates that we can't login to an account that has not had a password setup for it yet.
        /// </summary>
        [Test]
        public void TestUnsetPassword() {
            var security = new SecurityController();
            security.Tunnel(CommandBuilder.SecurityAddGroup("GroupName").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupAddAccount("GroupName", "Phogue").SetOrigin(CommandOrigin.Local));

            // Now send an empty password through to authenticate against.
            ICommandResult result = security.Tunnel(CommandBuilder.SecurityAccountAuthenticate("Phogue", String.Empty).SetOrigin(CommandOrigin.Local));

            // Validate that we couldn't login because the server does not have a password set for it yet.
            Assert.IsFalse(result.Success);
            Assert.AreEqual(result.Status, CommandResultType.DoesNotExists);
        }
    }
}
