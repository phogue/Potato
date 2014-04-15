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
using NUnit.Framework;
using Potato.Core.Security;
using Potato.Core.Shared;
using Potato.Core.Shared.Models;
using Potato.Net.Shared.Protocols;

namespace Potato.Core.Test.Security.Account {
    [TestFixture]
    public class TestGetAccount {
        [SetUp]
        public void Initialize() {
            SharedReferences.Setup();
        }

        [Test]
        public void TestByCommandInitiatorWithPlayerDetails() {
            var security = new SecurityController();
            security.Tunnel(CommandBuilder.SecurityAddGroup("GroupName").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupAddAccount("GroupName", "Phogue").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityAccountAddPlayer("Phogue", CommonProtocolType.DiceBattlefield3, "ABCDEF").SetOrigin(CommandOrigin.Local));

            AccountModel account = security.GetAccount(new Command() {
                Authentication = {
                    GameType = CommonProtocolType.DiceBattlefield3,
                    Uid = "ABCDEF"
                }
            });

            // Validate the account was fetched successfully
            Assert.IsNotNull(account);
            Assert.AreEqual("Phogue", account.Username);
        }

        [Test]
        public void TestByCommandInitiatorWithUsername() {
            var security = new SecurityController();
            security.Tunnel(CommandBuilder.SecurityAddGroup("GroupName").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupAddAccount("GroupName", "Phogue").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityAccountAddPlayer("Phogue", CommonProtocolType.DiceBattlefield3, "ABCDEF").SetOrigin(CommandOrigin.Local));

            AccountModel account = security.GetAccount(new Command() {
                Authentication = {
                    Username = "Phogue"
                }
            });

            // Validate the account was fetched successfully
            Assert.IsNotNull(account);
            Assert.AreEqual("Phogue", account.Username);
        }

        [Test]
        public void TestByCommandInitiatorWithUsernameCaseInsensitive() {
            var security = new SecurityController();
            security.Tunnel(CommandBuilder.SecurityAddGroup("GroupName").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupAddAccount("GroupName", "Phogue").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityAccountAddPlayer("Phogue", CommonProtocolType.DiceBattlefield3, "ABCDEF").SetOrigin(CommandOrigin.Local));

            AccountModel account = security.GetAccount(new Command() {
                Authentication = {
                    Username = "PHOGUE"
                }
            });

            // Validate the account was fetched successfully
            Assert.IsNotNull(account);
            Assert.AreEqual("Phogue", account.Username);
        }

        [Test]
        public void TestByPlayerDetails() {
            var security = new SecurityController();
            security.Tunnel(CommandBuilder.SecurityAddGroup("GroupName").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupAddAccount("GroupName", "Phogue").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityAccountAddPlayer("Phogue", CommonProtocolType.DiceBattlefield3, "ABCDEF").SetOrigin(CommandOrigin.Local));

            AccountModel account = security.GetAccount(CommonProtocolType.DiceBattlefield3, "ABCDEF");

            // Validate the account was fetched successfully
            Assert.IsNotNull(account);
            Assert.AreEqual("Phogue", account.Username);
        }

        [Test]
        public void TestByUsername() {
            var security = new SecurityController();
            security.Tunnel(CommandBuilder.SecurityAddGroup("GroupName").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupAddAccount("GroupName", "Phogue").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityAccountAddPlayer("Phogue", CommonProtocolType.DiceBattlefield3, "ABCDEF").SetOrigin(CommandOrigin.Local));

            AccountModel account = security.GetAccount("Phogue");

            // Validate the account was fetched successfully
            Assert.IsNotNull(account);
            Assert.AreEqual("Phogue", account.Username);
        }

        [Test]
        public void TestByUsernameCaseInsensitive() {
            var security = new SecurityController();
            security.Tunnel(CommandBuilder.SecurityAddGroup("GroupName").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupAddAccount("GroupName", "Phogue").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityAccountAddPlayer("Phogue", CommonProtocolType.DiceBattlefield3, "ABCDEF").SetOrigin(CommandOrigin.Local));

            AccountModel account = security.GetAccount("PHOGUE");

            // Validate the account was fetched successfully
            Assert.IsNotNull(account);
            Assert.AreEqual("Phogue", account.Username);
        }

        [Test]
        public void TestByTokenId() {
            Guid tokenId = Guid.NewGuid();

            var security = new SecurityController();
            security.Tunnel(CommandBuilder.SecurityAddGroup("GroupName").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupAddAccount("GroupName", "Phogue").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityAccountAddPlayer("Phogue", CommonProtocolType.DiceBattlefield3, "ABCDEF").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityAccountAppendAccessToken("Phogue", tokenId, "Token Hash", DateTime.Now).SetOrigin(CommandOrigin.Local));

            AccountModel account = security.GetAccount(tokenId);

            // Validate the account was fetched successfully
            Assert.IsNotNull(account);
            Assert.AreEqual("Phogue", account.Username);
        }

        /// <summary>
        /// Tests that if no account exists with the specified access token, no account is returned.
        /// </summary>
        [Test]
        public void TestByTokenIdDoesNotExist() {
            Guid tokenId = Guid.NewGuid();

            var security = new SecurityController();
            security.Tunnel(CommandBuilder.SecurityAddGroup("GroupName").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupAddAccount("GroupName", "Phogue").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityAccountAddPlayer("Phogue", CommonProtocolType.DiceBattlefield3, "ABCDEF").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityAccountAppendAccessToken("Phogue", tokenId, "Token Hash", DateTime.Now).SetOrigin(CommandOrigin.Local));

            AccountModel account = security.GetAccount(Guid.NewGuid());

            // Validate the account was not found
            Assert.IsNull(account);
        }
    }
}