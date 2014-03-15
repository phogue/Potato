using NUnit.Framework;
using Procon.Core.Security;
using Procon.Core.Shared;
using Procon.Core.Shared.Models;
using Procon.Net.Shared.Protocols;

namespace Procon.Core.Test.Security.Account {
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
    }
}