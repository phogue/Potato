using System;
using System.Linq;
using NUnit.Framework;
using Procon.Core.Security;
using Procon.Core.Shared;
using Procon.Core.Shared.Models;

namespace Procon.Core.Test.Security.Account {
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
            Assert.AreEqual(CommandResultType.InsufficientPermissions, result.Status);
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
            Assert.AreEqual(result.Status, CommandResultType.Success);
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
            Assert.AreEqual(result.Status, CommandResultType.Success);
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
            Assert.AreEqual(CommandResultType.DoesNotExists, result.Status);
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
            Assert.AreEqual(result.Status, CommandResultType.DoesNotExists);
            Assert.AreEqual(security.Groups.Last().Accounts.First().PreferredLanguageCode, String.Empty);
        }
    }
}