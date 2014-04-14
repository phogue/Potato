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
using System.Linq;
using NUnit.Framework;
using Procon.Core.Security;
using Procon.Core.Shared;
using Procon.Core.Shared.Models;
using Procon.Net.Shared.Utils;

namespace Procon.Core.Test.Security.Account {
    [TestFixture]
    public class TestSecurityAccountAuthenticate {
        [SetUp]
        public void Initialize() {
            SharedReferences.Setup();
        }

        /// <summary>
        /// Tests authentication is successful when correct details are supplied
        /// </summary>
        [Test]
        public void TestAuthenticationSuccess() {
            String generatedAuthenticatePassword = StringExtensions.RandomString(10);

            var security = new SecurityController();
            security.Tunnel(CommandBuilder.SecurityAddGroup("GroupName").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupAddAccount("GroupName", "Phogue").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityAccountSetPassword("Phogue", generatedAuthenticatePassword).SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupSetPermission("GroupName", CommandType.SecurityAccountAuthenticate, 50).SetOrigin(CommandOrigin.Local));

            // Now authenticate against an empty security object which has no accounts.
            ICommandResult result = security.Tunnel(CommandBuilder.SecurityAccountAuthenticate("Phogue", generatedAuthenticatePassword, String.Empty).SetOrigin(CommandOrigin.Remote).SetAuthentication(new CommandAuthenticationModel() {
                Username = "Phogue"
            }));

            // Validate that we get nothing back.
            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Success, result.CommandResultType);
        }

        /// <summary>
        ///     Tests that authentication returns the users account and group when successful.
        /// </summary>
        [Test]
        public void TestAccountGroupReturnedInResult() {
            String generatedAuthenticatePassword = StringExtensions.RandomString(10);

            var security = new SecurityController();
            security.Tunnel(CommandBuilder.SecurityAddGroup("GroupName").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupAddAccount("GroupName", "Phogue").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityAccountSetPassword("Phogue", generatedAuthenticatePassword).SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupSetPermission("GroupName", CommandType.SecurityAccountAuthenticate, 50).SetOrigin(CommandOrigin.Local));

            // Now authenticate against an empty security object which has no accounts.
            ICommandResult result = security.Tunnel(CommandBuilder.SecurityAccountAuthenticate("Phogue", generatedAuthenticatePassword, String.Empty).SetOrigin(CommandOrigin.Remote).SetAuthentication(new CommandAuthenticationModel() {
                Username = "Phogue"
            }));

            // Validate that we get nothing back.
            Assert.AreEqual("Phogue", result.Scope.Accounts.First().Username);
            Assert.AreEqual("GroupName", result.Scope.Groups.First().Name);
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
            ICommandResult result = security.Tunnel(CommandBuilder.SecurityAccountAuthenticate("DoesNotExist", generatedAuthenticatePassword, String.Empty).SetOrigin(CommandOrigin.Local));

            // Validate that we get nothing back.
            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.Failed, result.CommandResultType);
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
            ICommandResult result = security.Tunnel(CommandBuilder.SecurityAccountAuthenticate("Phogue", generatedAuthenticatePassword, String.Empty).SetOrigin(CommandOrigin.Local));

            // Validate that we could authenticate with our new password.
            Assert.IsFalse(result.Success);
            Assert.AreEqual(result.CommandResultType, CommandResultType.Failed);
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

            ICommandResult result = security.Tunnel(CommandBuilder.SecurityAccountAuthenticate("Phogue", "password", String.Empty).SetOrigin(CommandOrigin.Local).SetOrigin(CommandOrigin.Remote).SetAuthentication(new CommandAuthenticationModel() {
                Username = "Phogue"
            }));

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.InsufficientPermissions, result.CommandResultType);
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
            ICommandResult result = security.Tunnel(CommandBuilder.SecurityAccountAuthenticate("Phogue", String.Empty, String.Empty).SetOrigin(CommandOrigin.Local));

            // Validate that we couldn't login because the server does not have a password set for it yet.
            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.Failed, result.CommandResultType);
        }

        /// <summary>
        /// Tests we get a token back when authenticating with an identifier.
        /// </summary>
        [Test]
        public void TestAuthenticationSuccessTokenGenerated() {
            String generatedAuthenticatePassword = StringExtensions.RandomString(10);

            var security = new SecurityController();
            security.Tunnel(CommandBuilder.SecurityAddGroup("GroupName").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupAddAccount("GroupName", "Phogue").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityAccountSetPassword("Phogue", generatedAuthenticatePassword).SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupSetPermission("GroupName", CommandType.SecurityAccountAuthenticate, 50).SetOrigin(CommandOrigin.Local));

            ICommandResult result = security.Tunnel(CommandBuilder.SecurityAccountAuthenticate("Phogue", generatedAuthenticatePassword, "192.168.1.1").SetOrigin(CommandOrigin.Remote).SetAuthentication(new CommandAuthenticationModel() {
                Username = "Phogue"
            }));

            Assert.IsNotEmpty(result.Scope.AccessTokens);
        }

        /// <summary>
        /// Tests that we can authenticate with the token that was generated.
        /// </summary>
        [Test]
        public void TestAuthenticationSuccessCanThenAuthenticateWithToken() {
            String generatedAuthenticatePassword = StringExtensions.RandomString(10);

            var security = new SecurityController();
            security.Tunnel(CommandBuilder.SecurityAddGroup("GroupName").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupAddAccount("GroupName", "Phogue").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityAccountSetPassword("Phogue", generatedAuthenticatePassword).SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupSetPermission("GroupName", CommandType.SecurityAccountAuthenticate, 50).SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupSetPermission("GroupName", CommandType.SecurityAccountAuthenticateToken, 50).SetOrigin(CommandOrigin.Local));

            ICommandResult result = security.Tunnel(CommandBuilder.SecurityAccountAuthenticate("Phogue", generatedAuthenticatePassword, "192.168.1.1").SetOrigin(CommandOrigin.Remote).SetAuthentication(new CommandAuthenticationModel() {
                Username = "Phogue"
            }));

            AccessTokenTransportModel token = result.Scope.AccessTokens.First();

            result = security.Tunnel(CommandBuilder.SecurityAccountAuthenticateToken(token.Id, token.Token, "192.168.1.1").SetOrigin(CommandOrigin.Remote).SetAuthentication(new CommandAuthenticationModel() {
                Token = token.Token,
                TokenId = token.Id
            }));

            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Success, result.CommandResultType);
        }

        /// <summary>
        /// Tests that authenticating with a generated token will fail with a different identifier
        /// </summary>
        /// <remarks>This is tested more thoroughly elsewhere, but this is just testing the entire process.</remarks>
        [Test]
        public void TestAuthenticationFailureWhenAuthenticateWithTokenDifferentIdentifier() {
            String generatedAuthenticatePassword = StringExtensions.RandomString(10);

            var security = new SecurityController();
            security.Tunnel(CommandBuilder.SecurityAddGroup("GroupName").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupAddAccount("GroupName", "Phogue").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityAccountSetPassword("Phogue", generatedAuthenticatePassword).SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupSetPermission("GroupName", CommandType.SecurityAccountAuthenticate, 50).SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupSetPermission("GroupName", CommandType.SecurityAccountAuthenticateToken, 50).SetOrigin(CommandOrigin.Local));

            ICommandResult result = security.Tunnel(CommandBuilder.SecurityAccountAuthenticate("Phogue", generatedAuthenticatePassword, "192.168.1.1").SetOrigin(CommandOrigin.Remote).SetAuthentication(new CommandAuthenticationModel() {
                Username = "Phogue"
            }));

            AccessTokenTransportModel token = result.Scope.AccessTokens.First();

            result = security.Tunnel(CommandBuilder.SecurityAccountAuthenticateToken(token.Id, token.Token, "192.168.1.2").SetOrigin(CommandOrigin.Remote).SetAuthentication(new CommandAuthenticationModel() {
                Token = token.Token,
                TokenId = token.Id
            }));

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.Failed, result.CommandResultType);
        }
    }
}
