using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Procon.Core.Security;
using Procon.Core.Shared;
using Procon.Core.Shared.Models;

namespace Procon.Core.Test.Security.Account {
    [TestFixture]
    public class TestSecurityAccountAuthenticateToken {
        /// <summary>
        ///     Tests that we cannot set the password of an account if we do not have permission to do so.
        /// </summary>
        [Test]
        public void TestInsufficientPermission() {
            var security = new SecurityController();
            security.Tunnel(CommandBuilder.SecurityAddGroup("GroupName").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupAddAccount("GroupName", "Phogue").SetOrigin(CommandOrigin.Local));

            ICommandResult result = security.Tunnel(CommandBuilder.SecurityAccountAuthenticateToken(Guid.NewGuid(), "TokenHash", "id")
                .SetOrigin(CommandOrigin.Remote)
                .SetAuthentication(new CommandAuthenticationModel() {
                    Username = "Phogue"
                })
            );

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.InsufficientPermissions, result.CommandResultType);
        }


        /// <summary>
        /// Tests that a generic Failed is returned if the user has no tokens attached to them.
        /// </summary>
        [Test]
        public void TestCannotAuthenticateAgainstEmptyTokenList() {
            const string identifier = "192.168.1.1";

            var security = new SecurityController();
            security.Tunnel(CommandBuilder.SecurityAddGroup("GroupName").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupAddAccount("GroupName", "Phogue").SetOrigin(CommandOrigin.Local));

            // Now validate that we can authenticate against the newly appended token hash
            ICommandResult result = security.Tunnel(CommandBuilder.SecurityAccountAuthenticateToken(Guid.NewGuid(), "token", identifier).SetOrigin(CommandOrigin.Local));

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.Failed, result.CommandResultType);
        }

        /// <summary>
        /// Tests that a generic Failed is returned if the token id does not exist
        /// </summary>
        [Test]
        public void TestCannotAuthenticateAgainstDifferentTokenId() {
            const string identifier = "192.168.1.1";

            AccessTokenModel accessToken = new AccessTokenModel() {
                Account = new AccountModel() {
                    Username = "Phogue",
                    PasswordHash = "MyPasswordHash"
                }
            };

            var token = accessToken.Generate(identifier);

            var security = new SecurityController();
            security.Tunnel(CommandBuilder.SecurityAddGroup("GroupName").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupAddAccount("GroupName", "Phogue").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityAccountSetPasswordHash("Phogue", "MyPasswordHash").SetOrigin(CommandOrigin.Local));

            // Now append the token onto the account.
            security.Tunnel(CommandBuilder.SecurityAccountAppendAccessToken("Phogue", accessToken.Id, accessToken.TokenHash, accessToken.LastTouched).SetOrigin(CommandOrigin.Local));

            // Now validate that we can authenticate against the newly appended token hash
            ICommandResult result = security.Tunnel(CommandBuilder.SecurityAccountAuthenticateToken(Guid.NewGuid(), token, identifier).SetOrigin(CommandOrigin.Local));

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.Failed, result.CommandResultType);
        }

        /// <summary>
        /// Tests that setting a new token can be validated against.
        /// </summary>
        [Test]
        public void TestSetSuccessCanAuthenticateAgainst() {
            const string identifier = "192.168.1.1";

            AccessTokenModel accessToken = new AccessTokenModel() {
                Account = new AccountModel() {
                    Username = "Phogue",
                    PasswordHash = "MyPasswordHash"
                }
            };

            var token = accessToken.Generate(identifier);

            var security = new SecurityController();
            security.Tunnel(CommandBuilder.SecurityAddGroup("GroupName").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupAddAccount("GroupName", "Phogue").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityAccountSetPasswordHash("Phogue", "MyPasswordHash").SetOrigin(CommandOrigin.Local));

            // Now append the token onto the account.
            security.Tunnel(CommandBuilder.SecurityAccountAppendAccessToken("Phogue", accessToken.Id, accessToken.TokenHash, accessToken.LastTouched).SetOrigin(CommandOrigin.Local));

            // Now validate that we can authenticate against the newly appended token hash
            ICommandResult result = security.Tunnel(CommandBuilder.SecurityAccountAuthenticateToken(accessToken.Id, token, identifier).SetOrigin(CommandOrigin.Local));

            Assert.IsTrue(result.Success);
            Assert.AreEqual(result.CommandResultType, CommandResultType.Success);
        }

        /// <summary>
        /// Tests that setting an access token, then changing a password hash will invalidate the token.
        /// </summary>
        [Test]
        public void TestModifiedPasswordHashAfterSettingInvalidatesToken() {
            const string identifier = "192.168.1.1";

            AccessTokenModel accessToken = new AccessTokenModel() {
                Account = new AccountModel() {
                    Username = "Phogue",
                    PasswordHash = "MyPasswordHash"
                }
            };

            var token = accessToken.Generate(identifier);

            var security = new SecurityController();
            security.Tunnel(CommandBuilder.SecurityAddGroup("GroupName").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupAddAccount("GroupName", "Phogue").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityAccountSetPasswordHash("Phogue", "MyPasswordHash").SetOrigin(CommandOrigin.Local));

            // Now append the token onto the account.
            security.Tunnel(CommandBuilder.SecurityAccountAppendAccessToken("Phogue", accessToken.Id, accessToken.TokenHash, accessToken.LastTouched).SetOrigin(CommandOrigin.Local));

            security.Tunnel(CommandBuilder.SecurityAccountSetPasswordHash("Phogue", "MyModifiedPasswordHash").SetOrigin(CommandOrigin.Local));

            // Now validate that we can authenticate against the newly appended token hash
            ICommandResult result = security.Tunnel(CommandBuilder.SecurityAccountAuthenticateToken(accessToken.Id, token, identifier).SetOrigin(CommandOrigin.Local));

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.Failed, result.CommandResultType);
        }


        /// <summary>
        /// Tests that setting an access token, but attempting to authenticate with a different identifier will result in failed authentication
        /// </summary>
        [Test]
        public void TestModifiedIdentiferAfterSettingInvalidatesToken() {
            const string identifier = "192.168.1.1";

            AccessTokenModel accessToken = new AccessTokenModel() {
                Account = new AccountModel() {
                    Username = "Phogue",
                    PasswordHash = "MyPasswordHash"
                }
            };

            var token = accessToken.Generate(identifier);

            var security = new SecurityController();
            security.Tunnel(CommandBuilder.SecurityAddGroup("GroupName").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupAddAccount("GroupName", "Phogue").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityAccountSetPasswordHash("Phogue", "MyPasswordHash").SetOrigin(CommandOrigin.Local));

            // Now append the token onto the account.
            security.Tunnel(CommandBuilder.SecurityAccountAppendAccessToken("Phogue", accessToken.Id, accessToken.TokenHash, accessToken.LastTouched).SetOrigin(CommandOrigin.Local));

            // Now validate that we can authenticate against the newly appended token hash
            ICommandResult result = security.Tunnel(CommandBuilder.SecurityAccountAuthenticateToken(accessToken.Id, token, "192.168.1.2").SetOrigin(CommandOrigin.Local));

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.Failed, result.CommandResultType);
        }
    }
}
