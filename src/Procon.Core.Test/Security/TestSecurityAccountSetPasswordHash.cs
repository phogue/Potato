using System;
using NUnit.Framework;
using Procon.Core.Security;
using Procon.Core.Shared;
using Procon.Core.Shared.Models;
using Procon.Net.Shared.Utils;

namespace Procon.Core.Test.Security {
    [TestFixture]
    public class TestSecurityAccountSetPasswordHash {
        [SetUp]
        public void Initialize() {
            SharedReferences.Setup();
        }

        /// <summary>
        ///     Tests that we cannot set the password of an account if we do not have permission to do so.
        ///     Thinking about this, we may need to write this to allow users to set their own passwords. This should be
        ///     done within the SecurityController to determine if a CommandName can edit it's own account details.
        /// </summary>
        [Test]
        public void TestInsufficientPermission() {
            var security = new SecurityController();
            security.Tunnel(CommandBuilder.SecurityAddGroup("GroupName").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupAddAccount("GroupName", "Phogue").SetOrigin(CommandOrigin.Local));

            ICommandResult result = security.Tunnel(CommandBuilder.SecurityAccountSetPasswordHash("Phogue", BCrypt.HashPassword("password", BCrypt.GenerateSalt()))
                .SetOrigin(CommandOrigin.Remote)
                .SetAuthentication(new CommandAuthenticationModel() {
                    Username = "Phogue"
                })
            );

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.InsufficientPermissions, result.Status);
        }

        /// <summary>
        ///     Test that setting a new password can be authenticated against
        /// </summary>
        [Test]
        public void TestSetSuccess() {
            String generatedPassword = StringExtensions.RandomString(10);
            String generatedPasswordHash = BCrypt.HashPassword(generatedPassword, BCrypt.GenerateSalt());

            var security = new SecurityController();
            security.Tunnel(CommandBuilder.SecurityAddGroup("GroupName").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupAddAccount("GroupName", "Phogue").SetOrigin(CommandOrigin.Local));

            // Now change the password of the account.
            security.Tunnel(CommandBuilder.SecurityAccountSetPasswordHash("Phogue", generatedPasswordHash).SetOrigin(CommandOrigin.Local));

            // Now validate that we can authenticate against the newly set password.
            ICommandResult result = security.Tunnel(CommandBuilder.SecurityAccountAuthenticate("Phogue", generatedPassword).SetOrigin(CommandOrigin.Local));

            // Validate that we could authenticate with our new password.
            Assert.IsTrue(result.Success);
            Assert.AreEqual(result.Status, CommandResultType.Success);
        }

        /// <summary>
        ///     Tests we get an empty command result back if the account we try to set a password on
        ///     does not exist.
        /// </summary>
        [Test]
        public void TestAccountDoesNotExist() {
            var security = new SecurityController();
            security.Tunnel(CommandBuilder.SecurityAddGroup("GroupName").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupAddAccount("GroupName", "Phogue").SetOrigin(CommandOrigin.Local));

            // Now change the password of the account.
            ICommandResult result = security.Tunnel(CommandBuilder.SecurityAccountSetPasswordHash("DoesNotExist", BCrypt.HashPassword("password", BCrypt.GenerateSalt())).SetOrigin(CommandOrigin.Local));

            // Validate that we could not set a password and the result returned false.
            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.DoesNotExists, result.Status);
        }

        /// <summary>
        ///     Validates that we can't set an empty password.
        /// </summary>
        [Test]
        public void TestEmptyPassword() {
            var security = new SecurityController();
            security.Tunnel(CommandBuilder.SecurityAddGroup("GroupName").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupAddAccount("GroupName", "Phogue").SetOrigin(CommandOrigin.Local));

            // Now change the password of the account.
            ICommandResult result = security.Tunnel(CommandBuilder.SecurityAccountSetPasswordHash("Phogue", String.Empty).SetOrigin(CommandOrigin.Local));

            // Validate that we could not set a password and the result returned false.
            Assert.IsFalse(result.Success);
            Assert.AreEqual(result.Status, CommandResultType.InvalidParameter);
        }
    }
}
