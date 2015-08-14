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
using Potato.Core.Security;
using Potato.Core.Shared;
using Potato.Core.Shared.Models;

namespace Potato.Core.Test.Security.Account {
    [TestFixture]
    public class TestSecurityAccountAppendAccessToken {
        [SetUp]
        public void Initialize() {
            SharedReferences.Setup();
        }

        /// <summary>
        ///     Tests that we cannot set the password of an account if we do not have permission to do so.
        /// </summary>
        [Test]
        public void TestInsufficientPermission() {
            var security = new SecurityController();
            security.Tunnel(CommandBuilder.SecurityAddGroup("GroupName").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupAddAccount("GroupName", "Phogue").SetOrigin(CommandOrigin.Local));

            var result = security.Tunnel(CommandBuilder.SecurityAccountAppendAccessToken("Phogue", Guid.NewGuid(), "TokenHash", DateTime.Now)
                .SetOrigin(CommandOrigin.Remote)
                .SetAuthentication(new CommandAuthenticationModel() {
                    Username = "Phogue"
                })
            );

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.InsufficientPermissions, result.CommandResultType);
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

            // Now append the token onto the account.
            var result = security.Tunnel(CommandBuilder.SecurityAccountAppendAccessToken("DoesNotExist", Guid.NewGuid(), "TokenHash", DateTime.Now).SetOrigin(CommandOrigin.Local));

            // Validate that we could not set a password and the result returned false.
            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.DoesNotExists, result.CommandResultType);
        }

        /// <summary>
        /// Validates that a token cannot be appended with an empty guid
        /// </summary>
        [Test]
        public void TestEmptyId() {
            var security = new SecurityController();
            security.Tunnel(CommandBuilder.SecurityAddGroup("GroupName").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupAddAccount("GroupName", "Phogue").SetOrigin(CommandOrigin.Local));

            // Now append the token onto the account.
            var result = security.Tunnel(CommandBuilder.SecurityAccountAppendAccessToken("Phogue", Guid.Empty, "TokenHash", DateTime.Now).SetOrigin(CommandOrigin.Local));

            // Validate that we could not set a password and the result returned false.
            Assert.IsFalse(result.Success);
            Assert.AreEqual(result.CommandResultType, CommandResultType.InvalidParameter);
        }

        /// <summary>
        /// Validates that a token cannot be appended with an empty TokenHash
        /// </summary>
        [Test]
        public void TestEmptyTokenHash() {
            var security = new SecurityController();
            security.Tunnel(CommandBuilder.SecurityAddGroup("GroupName").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupAddAccount("GroupName", "Phogue").SetOrigin(CommandOrigin.Local));

            // Now append the token onto the account.
            var result = security.Tunnel(CommandBuilder.SecurityAccountAppendAccessToken("Phogue", Guid.NewGuid(), "", DateTime.Now).SetOrigin(CommandOrigin.Local));

            // Validate that we could not set a password and the result returned false.
            Assert.IsFalse(result.Success);
            Assert.AreEqual(result.CommandResultType, CommandResultType.InvalidParameter);
        }

        /// <summary>
        /// Validates that a token cannot be appended with an expired touched time
        /// </summary>
        [Test]
        public void TestExpiredToken() {
            var security = new SecurityController();
            security.Tunnel(CommandBuilder.SecurityAddGroup("GroupName").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupAddAccount("GroupName", "Phogue").SetOrigin(CommandOrigin.Local));

            // Now append the token onto the account.
            var result = security.Tunnel(CommandBuilder.SecurityAccountAppendAccessToken("Phogue", Guid.NewGuid(), "TokenHash", DateTime.Now.AddDays(-3)).SetOrigin(CommandOrigin.Local));

            // Validate that we could not set a password and the result returned false.
            Assert.IsFalse(result.Success);
            Assert.AreEqual(result.CommandResultType, CommandResultType.InvalidParameter);
        }

        /// <summary>
        /// Tests that appending a new valid token hash returns success
        /// </summary>
        [Test]
        public void TestSetSuccess() {
            var security = new SecurityController();
            security.Tunnel(CommandBuilder.SecurityAddGroup("GroupName").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupAddAccount("GroupName", "Phogue").SetOrigin(CommandOrigin.Local));

            // Now append the token onto the account.
            var result = security.Tunnel(CommandBuilder.SecurityAccountAppendAccessToken("Phogue", Guid.NewGuid(), "TokenHash", DateTime.Now).SetOrigin(CommandOrigin.Local));

            Assert.IsTrue(result.Success);
            Assert.AreEqual(result.CommandResultType, CommandResultType.Success);
        }

        /// <summary>
        /// Tests that setting a new token appears in the list of tokens for the account.
        /// </summary>
        [Test]
        public void TestSetSuccessAppearsInAccountTokens() {
            var security = new SecurityController();
            security.Tunnel(CommandBuilder.SecurityAddGroup("GroupName").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupAddAccount("GroupName", "Phogue").SetOrigin(CommandOrigin.Local));

            // Now append the token onto the account.
            security.Tunnel(CommandBuilder.SecurityAccountAppendAccessToken("Phogue", Guid.NewGuid(), "TokenHash", DateTime.Now).SetOrigin(CommandOrigin.Local));

            Assert.IsNotEmpty(security.Groups.SelectMany(group => group.Accounts).First(account => account.Username == "Phogue").AccessTokens);
        }

        /// <summary>
        /// Tests that multiple token id's won't exist, but instead later tokens will overwrite the token hash
        /// </summary>
        [Test]
        public void TestSetSuccessIdenticalIdsOverwriteWithNewData() {
            var security = new SecurityController();
            security.Tunnel(CommandBuilder.SecurityAddGroup("GroupName").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupAddAccount("GroupName", "Phogue").SetOrigin(CommandOrigin.Local));

            var id = Guid.NewGuid();

            // Now append the token onto the account.
            security.Tunnel(CommandBuilder.SecurityAccountAppendAccessToken("Phogue", id, "TokenHashOne", DateTime.Now).SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityAccountAppendAccessToken("Phogue", id, "TokenHashTwo", DateTime.Now).SetOrigin(CommandOrigin.Local));

            Assert.AreEqual("TokenHashTwo", security.Groups.SelectMany(group => @group.Accounts).First(account => account.Username == "Phogue").AccessTokens.First().Value.TokenHash);
        }

        /// <summary>
        /// Tests adding above the maximum amount of token hashes will cull the list of the oldest token
        /// </summary>
        [Test]
        public void TestAddingTooManyTokensCullsTheTokenList() {
            var security = new SecurityController();
            security.Tunnel(CommandBuilder.SecurityAddGroup("GroupName").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupAddAccount("GroupName", "Phogue").SetOrigin(CommandOrigin.Local));

            var oldest = Guid.NewGuid();

            // The default limit is 5. I figured I should test with the defaults.
            security.Tunnel(CommandBuilder.SecurityAccountAppendAccessToken("Phogue", oldest, "TokenHashOldest", DateTime.Now.AddHours(-1)).SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityAccountAppendAccessToken("Phogue", Guid.NewGuid(), "TokenHashOne", DateTime.Now).SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityAccountAppendAccessToken("Phogue", Guid.NewGuid(), "TokenHashTwo", DateTime.Now).SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityAccountAppendAccessToken("Phogue", Guid.NewGuid(), "TokenHashThree", DateTime.Now).SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityAccountAppendAccessToken("Phogue", Guid.NewGuid(), "TokenHashFour", DateTime.Now).SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityAccountAppendAccessToken("Phogue", Guid.NewGuid(), "TokenHashFive", DateTime.Now).SetOrigin(CommandOrigin.Local));

            // Test that we have the maximum and the oldest token isn't in the list.
            Assert.AreEqual(5, security.Groups.SelectMany(group => @group.Accounts).First(account => account.Username == "Phogue").AccessTokens.Count);
            Assert.IsNull(security.Groups.SelectMany(group => @group.Accounts).First(account => account.Username == "Phogue").AccessTokens.Where(token => token.Key == oldest).Select(token => token.Value).FirstOrDefault());
        }

    }
}
