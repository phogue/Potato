using System;
using System.Linq;
using NUnit.Framework;
using Potato.Core.Security;
using Potato.Core.Shared;

namespace Potato.Core.Test.Security {
    [TestFixture]
    public class TestPoke {
        /// <summary>
        /// Test no tokens removed if all are above the threshold
        /// </summary>
        [Test]
        public void TestNoneExpiredNoneRemoved() {
            var security = new SecurityController();
            security.Tunnel(CommandBuilder.SecurityAddGroup("GroupName").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupAddAccount("GroupName", "Phogue").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityAccountAppendAccessToken("Phogue", Guid.NewGuid(), "Token Hash One", DateTime.Now).SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityAccountAppendAccessToken("Phogue", Guid.NewGuid(), "Token Hash Two", DateTime.Now).SetOrigin(CommandOrigin.Local));

            security.Poke();

            Assert.AreEqual(2, security.Groups.First(group => group.Name == "GroupName").Accounts.First().AccessTokens.Count);
        }

        /// <summary>
        /// Test a single token is removed if it is expired.
        /// </summary>
        [Test]
        public void TestOneExpiredOneRemoved() {
            var security = new SecurityController();
            security.Tunnel(CommandBuilder.SecurityAddGroup("GroupName").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupAddAccount("GroupName", "Phogue").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityAccountAppendAccessToken("Phogue", Guid.NewGuid(), "Token Hash One", DateTime.Now.AddDays(-3)).SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityAccountAppendAccessToken("Phogue", Guid.NewGuid(), "Token Hash Two", DateTime.Now).SetOrigin(CommandOrigin.Local));

            security.Poke();

            Assert.AreEqual(1, security.Groups.First(group => group.Name == "GroupName").Accounts.First().AccessTokens.Count);
            Assert.AreEqual("Token Hash Two", security.Groups.First(group => group.Name == "GroupName").Accounts.First().AccessTokens.First().Value.TokenHash);
        }

        /// <summary>
        /// Test a single token is removed if it is expired.
        /// </summary>
        [Test]
        public void TestTwoExpiredTwoRemoved() {
            var security = new SecurityController();
            security.Tunnel(CommandBuilder.SecurityAddGroup("GroupName").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupAddAccount("GroupName", "Phogue").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityAccountAppendAccessToken("Phogue", Guid.NewGuid(), "Token Hash One", DateTime.Now.AddDays(-3)).SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityAccountAppendAccessToken("Phogue", Guid.NewGuid(), "Token Hash Two", DateTime.Now.AddDays(-3)).SetOrigin(CommandOrigin.Local));

            security.Poke();

            Assert.IsEmpty(security.Groups.First(group => group.Name == "GroupName").Accounts.First().AccessTokens);
        }
    }
}
