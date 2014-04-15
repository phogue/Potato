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
    public class TestSecurityAccountSetPreferredLanguageCode {
        [SetUp]
        public void Initialize() {
            SharedReferences.Setup();
        }

        /// <summary>
        /// Tests that an account cannot set the preferred language of another account unless they
        /// have permission to do so.
        /// </summary>
        [Test]
        public void TestInsufficientPermission() {
            var security = new SecurityController();
            security.Tunnel(CommandBuilder.SecurityAddGroup("GroupName").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupAddAccount("GroupName", "Phogue").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupAddAccount("GroupName", "Ike").SetOrigin(CommandOrigin.Local));

            // Now change the language of the account.
            ICommandResult result = security.Tunnel(CommandBuilder.SecurityAccountSetPreferredLanguageCode("Ike", "de-DE")
                .SetOrigin(CommandOrigin.Remote)
                .SetAuthentication(new CommandAuthenticationModel() {
                    Username = "Phogue"
                })
            );

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.InsufficientPermissions, result.CommandResultType);
        }

        /// <summary>
        ///     Test setting the preferred language for an account works.
        /// </summary>
        [Test]
        public void TestSetSuccess() {
            var security = new SecurityController();
            security.Tunnel(CommandBuilder.SecurityAddGroup("GroupName").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupAddAccount("GroupName", "Phogue").SetOrigin(CommandOrigin.Local));

            // Now change the language of the account.
            ICommandResult result = security.Tunnel(CommandBuilder.SecurityAccountSetPreferredLanguageCode("Phogue", "de-DE").SetOrigin(CommandOrigin.Local));

            // Make sure it was successful.
            Assert.IsTrue(result.Success);
            Assert.AreEqual(result.CommandResultType, CommandResultType.Success);
            Assert.AreEqual(security.Groups.Last().Accounts.First().PreferredLanguageCode, "de-DE");
        }

        /// <summary>
        /// Tests that an account can set their own preferred language, even if their group does not have permission to do so.
        /// </summary>
        [Test]
        public void TestOwnAccount() {
            var security = new SecurityController();
            security.Tunnel(CommandBuilder.SecurityAddGroup("GroupName").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupAddAccount("GroupName", "Phogue").SetOrigin(CommandOrigin.Local));

            ICommandResult result = security.Tunnel(CommandBuilder.SecurityAccountSetPreferredLanguageCode("Phogue", "de-DE")
                .SetOrigin(CommandOrigin.Remote)
                .SetAuthentication(new CommandAuthenticationModel() {
                    Username = "Phogue"        
                })
            );

            // Make sure it was successful.
            Assert.IsTrue(result.Success);
            Assert.AreEqual(result.CommandResultType, CommandResultType.Success);
            Assert.AreEqual(security.Groups.Last().Accounts.First().PreferredLanguageCode, "de-DE");
        }

        /// <summary>
        ///     Test that we get nothing back if the account does not exist.
        /// </summary>
        [Test]
        public void TestAccountDoesNotExist() {
            var security = new SecurityController();
            security.Tunnel(CommandBuilder.SecurityAddGroup("GroupName").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupAddAccount("GroupName", "ThisExists").SetOrigin(CommandOrigin.Local));

            // Now change the language of the account.
            ICommandResult result = security.Tunnel(CommandBuilder.SecurityAccountSetPreferredLanguageCode("ThisDoesNotExist", "de-DE").SetOrigin(CommandOrigin.Local));

            // Make sure we get nothing back if we try to change the language code of
            // an account that does not exist.
            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.DoesNotExists, result.CommandResultType);
        }

        /// <summary>
        ///     Test setting the preferred language for an account works.
        /// </summary>
        [Test]
        public void TestLanguageDoesNotExist() {
            var security = new SecurityController();
            security.Tunnel(CommandBuilder.SecurityAddGroup("GroupName").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupAddAccount("GroupName", "Phogue").SetOrigin(CommandOrigin.Local));

            // Now change the language of the account.
            ICommandResult result = security.Tunnel(CommandBuilder.SecurityAccountSetPreferredLanguageCode("ThisDoesNotExist", "zu-ZU").SetOrigin(CommandOrigin.Local));

            // Make sure it was successful.
            Assert.IsFalse(result.Success);
            Assert.AreEqual(result.CommandResultType, CommandResultType.DoesNotExists);
            Assert.AreEqual(security.Groups.Last().Accounts.First().PreferredLanguageCode, String.Empty);
        }
    }
}