using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Procon.Core.Test.Security {
    using Procon.Core.Security;
    using Procon.Net.Protocols;
    using Procon.Net.Utils;

    [TestClass]
    public class TestSecurityAccount {

        #region Adding Accounts

        /// <summary>
        /// Tests that adding a simpel account can be completed.
        /// </summary>
        [TestMethod]
        public void TestSecurityGroupsAddAccount() {
            SecurityController security = new SecurityController();
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAddGroup, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "GroupName" }) });

            // Test that the group was initially added.
            Assert.AreEqual<String>(security.Groups.First().Name, "GroupName");

            // Now add the user.
            CommandResultArgs result = security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityGroupAddAccount, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "GroupName", "Phogue" }) });

            // Make sure the account was successfully created.
            Assert.IsTrue(result.Success);
            Assert.AreEqual<CommandResultType>(result.Status, CommandResultType.Success);
            Assert.AreEqual<String>(security.Groups.SelectMany(group => group.Accounts).First().Username, "Phogue");
        }

        /// <summary>
        /// Tests the command to add an account failes if the user has insufficient privileges.
        /// </summary>
        [TestMethod]
        public void TestSecurityGroupsAddAccountInsufficientPermission() {
            SecurityController security = new SecurityController();
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAddGroup, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "GroupName" }) });

            CommandResultArgs result = security.Execute(new Command() {
                CommandType = CommandType.SecurityGroupAddAccount,
                Username = "Phogue",
                Origin = CommandOrigin.Remote,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName",
                    "Phogue"
                })
            });

            Assert.IsFalse(result.Success);
            Assert.AreEqual<CommandResultType>(CommandResultType.InsufficientPermissions, result.Status);
        }

        /// <summary>
        /// Tests that adding a simpel account can be completed.
        /// </summary>
        [TestMethod]
        public void TestSecurityGroupsAddAccountGroupDoesNotExist() {
            SecurityController security = new SecurityController();

            // Add the user.
            CommandResultArgs result = security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityGroupAddAccount, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "NonExistentGroup", "Phogue" }) });

            // Make sure the command returned nothing
            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Continue, result.Status);
        }

        /// <summary>
        /// Tests that an account cannot be added if the username is empty.
        /// </summary>
        [TestMethod]
        public void TestSecurityGroupsAddAccountEmptyUsername() {
            SecurityController security = new SecurityController();
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAddGroup, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "GroupName" }) });

            // Test that the group was initially added.
            Assert.AreEqual<String>(security.Groups.First().Name, "GroupName");

            // Now add the user.
            CommandResultArgs result = security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityGroupAddAccount, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "GroupName", String.Empty }) });

            // Make sure the account was successfully created.
            Assert.IsFalse(result.Success);
            Assert.AreEqual<CommandResultType>(result.Status, CommandResultType.InvalidParameter);
        }

        /// <summary>
        /// Tests that adding an account with a duplicate username will move the user to the new group.
        /// </summary>
        [TestMethod]
        public void TestSecurityGroupsAddAccountExistingName() {
            SecurityController security = new SecurityController();
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAddGroup, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "FirstGroupName" }) });
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAddGroup, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "SecondGroupName" }) });

            // Test that the groups were initially added.
            Assert.IsNotNull(security.Groups.Where(group => group.Name == "FirstGroupName").FirstOrDefault());
            Assert.IsNotNull(security.Groups.Where(group => group.Name == "SecondGroupName").FirstOrDefault());

            // Now add the user.
            CommandResultArgs result = security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityGroupAddAccount, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "FirstGroupName", "Phogue" }) });

            // Now make sure the user was initially added.
            Assert.IsTrue(result.Success);
            Assert.AreEqual<CommandResultType>(result.Status, CommandResultType.Success);
            Assert.AreEqual<String>(security.Groups.Where(group => group.Name == "FirstGroupName").SelectMany(group => group.Accounts).First().Username, "Phogue");
            Assert.IsNull(security.Groups.Where(group => group.Name == "SecondGroupName").SelectMany(group => group.Accounts).FirstOrDefault());

            // Now move the user to the second group.
            result = security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityGroupAddAccount, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "SecondGroupName", "Phogue" }) });

            // Make sure setting the kick permission was successfull.
            Assert.IsTrue(result.Success);
            Assert.AreEqual<CommandResultType>(result.Status, CommandResultType.Success);
            Assert.IsNull(security.Groups.Where(group => group.Name == "FirstGroupName").SelectMany(group => group.Accounts).FirstOrDefault());
            Assert.AreEqual<String>(security.Groups.Where(group => group.Name == "SecondGroupName").SelectMany(group => group.Accounts).First().Username, "Phogue");
        }

        #endregion

        #region Removing Accounts

        /// <summary>
        /// Testing that a group can be removed by its name.
        /// </summary>
        [TestMethod]
        public void TestSecurityRemoveAccount() {
            SecurityController security = new SecurityController();
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAddGroup, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "GroupName" }) });
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityGroupAddAccount, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "GroupName", "Phogue" }) });

            // Test that the group was initially added.
            Assert.AreEqual<String>(security.Groups.First().Accounts.First().Username, "Phogue");

            CommandResultArgs result = security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityRemoveAccount, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "Phogue" }) });

            // Make sure it was successful.
            Assert.IsTrue(result.Success);
            Assert.AreEqual<int>(0, security.Groups.First().Accounts.Count);
        }

        /// <summary>
        /// Tests the command to add a player to an account fails if the user has insufficient privileges.
        /// </summary>
        [TestMethod]
        public void TestSecurityRemoveAccountInsufficientPermission() {
            SecurityController security = new SecurityController();
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAddGroup, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "GroupName" }) });
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityGroupAddAccount, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "GroupName", "Phogue" }) });

            // Test that the group was initially added.
            Assert.AreEqual<String>(security.Groups.First().Accounts.First().Username, "Phogue");

            CommandResultArgs result = security.Execute(new Command() {
                CommandType = CommandType.SecurityRemoveAccount,
                Username = "Phogue",
                Origin = CommandOrigin.Remote,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "Phogue"
                })
            });

            // Make sure it was successful.
            Assert.IsFalse(result.Success);
            Assert.AreEqual<CommandResultType>(CommandResultType.InsufficientPermissions, result.Status);
        }

        /// <summary>
        /// Testing that you can't remove a group name that is empty.
        /// </summary>
        [TestMethod]
        public void TestSecurityRemoveAccountEmptyAccountUsername() {
            SecurityController security = new SecurityController();

            // Add a group with an empty name.
            CommandResultArgs result = security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityRemoveAccount, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { String.Empty }) });

            // Make sure adding an empty group fails.
            Assert.IsFalse(result.Success);
            Assert.AreEqual<CommandResultType>(result.Status, CommandResultType.InvalidParameter);
        }

        /// <summary>
        /// Testing that you can't remove a group that does not exist and provides correct errors.
        /// </summary>
        [TestMethod]
        public void TestSecurityRemoveAccountNotExists() {
            SecurityController security = new SecurityController();
            CommandResultArgs result = security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityRemoveAccount, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "Phogue" }) });

            // Make sure it was not successful.
            Assert.IsFalse(result.Success);
            Assert.AreEqual<CommandResultType>(result.Status, CommandResultType.DoesNotExists);
        }

        #endregion

        #region Language

        /// <summary>
        /// Test setting the preferred language for an account works.
        /// </summary>
        [TestMethod]
        public void TestSecurityAccountSetPreferredLanguageCode() {
            SecurityController security = new SecurityController();
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAddGroup, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "GroupName" }) });

            // Test that the group was initially added.
            Assert.AreEqual<String>(security.Groups.First().Name, "GroupName");

            // Now add a user.
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityGroupAddAccount, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "GroupName", "Phogue" }) });

            // Test that the account was added to the group.
            Assert.AreEqual<String>(security.Groups.SelectMany(group => group.Accounts).First().Username, "Phogue");

            // Now change the language of the account.
            CommandResultArgs result = security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAccountSetPreferredLanguageCode, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "Phogue", "de-DE" }) });

            // Make sure it was successful.
            Assert.IsTrue(result.Success);
            Assert.AreEqual<CommandResultType>(result.Status, CommandResultType.Success);
            Assert.AreEqual<String>(security.Groups.First().Accounts.First().PreferredLanguageCode, "de-DE");
        }

        /// <summary>
        /// Test setting the preferred language for an account works.
        /// </summary>
        [TestMethod]
        public void TestSecurityAccountSetPreferredLanguageCodeLanguageDoesNotExist() {
            SecurityController security = new SecurityController();
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAddGroup, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "GroupName" }) });

            // Test that the group was initially added.
            Assert.AreEqual<String>(security.Groups.First().Name, "GroupName");

            // Now add a user.
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityGroupAddAccount, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "GroupName", "Phogue" }) });

            // Test that the account was added to the group.
            Assert.AreEqual<String>(security.Groups.SelectMany(group => group.Accounts).First().Username, "Phogue");

            // Now change the language of the account.
            CommandResultArgs result = security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAccountSetPreferredLanguageCode, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "Phogue", "zu-ZU" }) });

            // Make sure it was successful.
            Assert.IsFalse(result.Success);
            Assert.AreEqual<CommandResultType>(result.Status, CommandResultType.DoesNotExists);
            Assert.AreEqual<String>(security.Groups.First().Accounts.First().PreferredLanguageCode, String.Empty);
        }

        /// <summary>
        /// Test that we get nothing back if the account does not exist.
        /// </summary>
        [TestMethod]
        public void TestSecurityAccountSetPreferredLanguageCodeAccountDoesNotExist() {
            SecurityController security = new SecurityController();
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAddGroup, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "GroupName" }) });
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityGroupAddAccount, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "GroupName", "ThisExists" }) });

            // Now change the language of the account.
            CommandResultArgs result = security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAccountSetPreferredLanguageCode, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "ThisDoesNotExist", "en-UK" }) });

            // Make sure we get nothing back if we try to change the language code of
            // an account that does not exist.
            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Continue, result.Status);
        }

        [TestMethod]
        public void TestSecurityAccountSetPreferredLanguageAccountInsufficientPermission() {
            SecurityController security = new SecurityController();
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAddGroup, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "GroupName" }) });
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityGroupAddAccount, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "GroupName", "Phogue" }) });

            // Now change the language of the account.
            CommandResultArgs result = security.Execute(new Command() {
                CommandType = CommandType.SecurityAccountSetPreferredLanguageCode,
                Username = "Phogue",
                Origin = CommandOrigin.Remote,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "Phogue",
                    "en-UK"
                })
            });

            Assert.IsFalse(result.Success);
            Assert.AreEqual<CommandResultType>(CommandResultType.InsufficientPermissions, result.Status);
        }

        #endregion

        #region Passwords

        /// <summary>
        /// Test that setting a new password can be authenticated against
        /// </summary>
        [TestMethod]
        public void TestSecurityAccountSetPassword() {

            String generatedPassword = StringExtensions.RandomString(10);

            SecurityController security = new SecurityController();
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAddGroup, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "GroupName" }) });

            // Test that the group was initially added.
            Assert.AreEqual<String>(security.Groups.First().Name, "GroupName");

            // Now add a user.
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityGroupAddAccount, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "GroupName", "Phogue" }) });

            // Test that the account was added to the group.
            Assert.AreEqual<String>(security.Groups.SelectMany(group => group.Accounts).First().Username, "Phogue");

            // Now change the password of the account.
            CommandResultArgs result = security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAccountSetPassword, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "Phogue", generatedPassword }) });

            // Make sure setting the password was successful.
            Assert.IsTrue(result.Success);
            Assert.AreEqual<CommandResultType>(result.Status, CommandResultType.Success);

            // Now validate that we can authenticate against the newly set password.
            result = security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAccountAuthenticate, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "Phogue", generatedPassword }) });

            // Validate that we could authenticate with our new password.
            Assert.IsTrue(result.Success);
            Assert.AreEqual<CommandResultType>(result.Status, CommandResultType.Success);
        }

        /// <summary>
        /// Test that setting a new password can be authenticated against
        /// </summary>
        [TestMethod]
        public void TestSecurityAccountAuthenticateIncorrectPassword() {

            String generatedSetPassword = StringExtensions.RandomString(10);
            String generatedAuthenticatePassword = StringExtensions.RandomString(10);

            SecurityController security = new SecurityController();
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAddGroup, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "GroupName" }) });

            // Test that the group was initially added.
            Assert.AreEqual<String>(security.Groups.First().Name, "GroupName");

            // Now add a user.
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityGroupAddAccount, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "GroupName", "Phogue" }) });

            // Test that the account was added to the group.
            Assert.AreEqual<String>(security.Groups.SelectMany(group => group.Accounts).First().Username, "Phogue");

            // Now change the password of the account.
            CommandResultArgs result = security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAccountSetPassword, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "Phogue", generatedSetPassword }) });

            // Make sure setting the password was successful.
            Assert.IsTrue(result.Success);
            Assert.AreEqual<CommandResultType>(result.Status, CommandResultType.Success);

            // Now validate that we can authenticate against the newly set password.
            result = security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAccountAuthenticate, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "Phogue", generatedAuthenticatePassword }) });

            // Validate that we could authenticate with our new password.
            Assert.IsFalse(result.Success);
            Assert.AreEqual<CommandResultType>(result.Status, CommandResultType.Failed);
        }

        /// <summary>
        /// Tests that authentication returns nothing if an account does not exist.
        /// </summary>
        [TestMethod]
        public void TestSecurityAccountAuthenticateAccountDoesNotExist() {
            String generatedAuthenticatePassword = StringExtensions.RandomString(10);

            SecurityController security = new SecurityController();
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAddGroup, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "GroupName" }) });
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityGroupAddAccount, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "GroupName", "ThisExists" }) });

            // Now authenticate against an empty security object which has no accounts.
            CommandResultArgs result = security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAccountAuthenticate, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "DoesNotExist", generatedAuthenticatePassword }) });

            // Validate that we get nothing back.
            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Continue, result.Status);
        }

        /// <summary>
        /// Tests that the account cannot be authenticated against if the account does not have permissions
        /// to authenticate in the first place.
        /// </summary>
        [TestMethod]
        public void TestSecurityAccountAuthenticateInsufficientPermission() {
            SecurityController security = new SecurityController();
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAddGroup, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "GroupName" }) });
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityGroupAddAccount, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "GroupName", "Phogue" }) });
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAccountSetPassword, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "Phogue", "password" }) });

            CommandResultArgs result = security.Execute(new Command() {
                CommandType = CommandType.SecurityAccountAuthenticate,
                Username = "Phogue",
                Origin = CommandOrigin.Remote,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "Phogue",
                    "password"
                })
            });

            Assert.IsFalse(result.Success);
            Assert.AreEqual<CommandResultType>(CommandResultType.InsufficientPermissions, result.Status);
        }

        /// <summary>
        /// Validates that we can't set an empty password.
        /// </summary>
        [TestMethod]
        public void TestSecurityAccountSetPasswordEmptyPassword() {
            SecurityController security = new SecurityController();
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAddGroup, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "GroupName" }) });

            // Test that the group was initially added.
            Assert.AreEqual<String>(security.Groups.First().Name, "GroupName");

            // Now add a user.
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityGroupAddAccount, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "GroupName", "Phogue" }) });

            // Test that the account was added to the group.
            Assert.AreEqual<String>(security.Groups.SelectMany(group => group.Accounts).First().Username, "Phogue");

            // Now change the password of the account.
            CommandResultArgs result = security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAccountSetPassword, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "Phogue", String.Empty }) });

            // Validate that we could not set a password and the result returned false.
            Assert.IsFalse(result.Success);
            Assert.AreEqual<CommandResultType>(result.Status, CommandResultType.InvalidParameter);
        }

        /// <summary>
        /// Validates that we can't login to an account that has not had a password setup for it yet.
        /// </summary>
        [TestMethod]
        public void TestSecurityAccountAuthenticateUnsetPassword() {
            SecurityController security = new SecurityController();
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAddGroup, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "GroupName" }) });

            // Test that the group was initially added.
            Assert.AreEqual<String>(security.Groups.First().Name, "GroupName");

            // Now add a user.
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityGroupAddAccount, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "GroupName", "Phogue" }) });

            // Test that the account was added to the group.
            Assert.AreEqual<String>(security.Groups.SelectMany(group => group.Accounts).First().Username, "Phogue");

            // Now send an empty password through to authenticate against.
            CommandResultArgs result = security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAccountAuthenticate, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "Phogue", String.Empty }) });

            // Validate that we couldn't login because the server does not have a password set for it yet.
            Assert.IsFalse(result.Success);
            Assert.AreEqual<CommandResultType>(result.Status, CommandResultType.DoesNotExists);
        }

        /// <summary>
        /// Tests we get an empty command result back if the account we try to set a password on
        /// does not exist.
        /// </summary>
        [TestMethod]
        public void TestSecurityAccountSetPasswordAccountDoesNotExist() {
            SecurityController security = new SecurityController();
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAddGroup, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "GroupName" }) });
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityGroupAddAccount, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "GroupName", "ThisExists" }) });

            // Now change the password of the account.
            CommandResultArgs result = security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAccountSetPassword, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "DoesNotExist", "password" }) });

            // Validate that we could not set a password and the result returned false.
            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Continue, result.Status);
        }

        /// <summary>
        /// Tests that we cannot set the password of an account if we do not have permission to do so.
        /// 
        /// Thinking about this, we may need to write this to allow users to set their own passwords. This should be
        /// done within the SecurityController to determine if a CommandName can edit it's own account details. 
        /// </summary>
        [TestMethod]
        public void TestSecurityAccountSetPasswordAccountInsufficientPermission() {
            SecurityController security = new SecurityController();
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAddGroup, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "GroupName" }) });
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityGroupAddAccount, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "GroupName", "Phogue" }) });

            CommandResultArgs result = security.Execute(new Command() {
                CommandType = CommandType.SecurityAccountSetPassword,
                Username = "Phogue",
                Origin = CommandOrigin.Remote,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "Phogue",
                    "password"
                })
            });

            Assert.IsFalse(result.Success);
            Assert.AreEqual<CommandResultType>(CommandResultType.InsufficientPermissions, result.Status);
        }

        #endregion

        #region Password Hashes

        /// <summary>
        /// Test that setting a new password can be authenticated against
        /// </summary>
        [TestMethod]
        public void TestSecurityAccountSetPasswordHash() {

            String generatedPassword = StringExtensions.RandomString(10);
            String generatedPasswordHash = BCrypt.HashPassword(generatedPassword, BCrypt.GenerateSalt());

            SecurityController security = new SecurityController();
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAddGroup, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "GroupName" }) });

            // Test that the group was initially added.
            Assert.AreEqual<String>(security.Groups.First().Name, "GroupName");

            // Now add a user.
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityGroupAddAccount, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "GroupName", "Phogue" }) });

            // Test that the account was added to the group.
            Assert.AreEqual<String>(security.Groups.SelectMany(group => group.Accounts).First().Username, "Phogue");

            // Now change the password of the account.
            CommandResultArgs result = security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAccountSetPasswordHash, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "Phogue", generatedPasswordHash }) });

            // Make sure setting the password was successful.
            Assert.IsTrue(result.Success);
            Assert.AreEqual<CommandResultType>(result.Status, CommandResultType.Success);

            // Now validate that we can authenticate against the newly set password.
            result = security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAccountAuthenticate, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "Phogue", generatedPassword }) });

            // Validate that we could authenticate with our new password.
            Assert.IsTrue(result.Success);
            Assert.AreEqual<CommandResultType>(result.Status, CommandResultType.Success);
        }

        /// <summary>
        /// Validates that we can't set an empty password.
        /// </summary>
        [TestMethod]
        public void TestSecurityAccountSetPasswordHashEmptyPassword() {
            SecurityController security = new SecurityController();
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAddGroup, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "GroupName" }) });

            // Test that the group was initially added.
            Assert.AreEqual<String>(security.Groups.First().Name, "GroupName");

            // Now add a user.
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityGroupAddAccount, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "GroupName", "Phogue" }) });

            // Test that the account was added to the group.
            Assert.AreEqual<String>(security.Groups.SelectMany(group => group.Accounts).First().Username, "Phogue");

            // Now change the password of the account.
            CommandResultArgs result = security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAccountSetPasswordHash, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "Phogue", String.Empty }) });

            // Validate that we could not set a password and the result returned false.
            Assert.IsFalse(result.Success);
            Assert.AreEqual<CommandResultType>(result.Status, CommandResultType.InvalidParameter);
        }

        /// <summary>
        /// Tests we get an empty command result back if the account we try to set a password on
        /// does not exist.
        /// </summary>
        [TestMethod]
        public void TestSecurityAccountSetPasswordHashAccountDoesNotExist() {
            SecurityController security = new SecurityController();
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAddGroup, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "GroupName" }) });
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityGroupAddAccount, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "GroupName", "ThisExists" }) });

            // Now change the password of the account.
            CommandResultArgs result = security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAccountSetPasswordHash, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "DoesNotExist", BCrypt.HashPassword("password", BCrypt.GenerateSalt()) }) });

            // Validate that we could not set a password and the result returned false.
            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Continue, result.Status);
        }

        /// <summary>
        /// Tests that we cannot set the password of an account if we do not have permission to do so.
        /// 
        /// Thinking about this, we may need to write this to allow users to set their own passwords. This should be
        /// done within the SecurityController to determine if a CommandName can edit it's own account details. 
        /// </summary>
        [TestMethod]
        public void TestSecurityAccountSetPasswordAccountHashInsufficientPermission() {
            SecurityController security = new SecurityController();
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityAddGroup, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "GroupName" }) });
            security.Execute(new Command() { Origin = CommandOrigin.Local, CommandType = CommandType.SecurityGroupAddAccount, Parameters = TestHelpers.ObjectListToContentList(new List<Object>() { "GroupName", "Phogue" }) });
            
            CommandResultArgs result = security.Execute(new Command() {
                CommandType = CommandType.SecurityAccountSetPasswordHash,
                Username = "Phogue",
                Origin = CommandOrigin.Remote,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "Phogue",
                    BCrypt.HashPassword("password", BCrypt.GenerateSalt())
                })
            });

            Assert.IsFalse(result.Success);
            Assert.AreEqual<CommandResultType>(CommandResultType.InsufficientPermissions, result.Status);
        }

        #endregion
    }
}
