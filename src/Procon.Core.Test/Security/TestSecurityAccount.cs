#region

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Procon.Core.Security;
using Procon.Core.Shared;
using Procon.Core.Shared.Models;
using Procon.Net.Shared.Utils;

#endregion

namespace Procon.Core.Test.Security {
    [TestFixture]
    public class TestSecurityAccount {
        [SetUp]
        public void Initialize() {
            SharedReferences.Setup();
        }

        /// <summary>
        ///     Tests that authentication returns nothing if an account does not exist.
        /// </summary>
        [Test]
        public void TestSecurityAccountAuthenticateAccountDoesNotExist() {
            String generatedAuthenticatePassword = StringExtensions.RandomString(10);

            var security = new SecurityController();
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAddGroup,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName"
                })
            });
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupAddAccount,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName",
                    "ThisExists"
                })
            });

            // Now authenticate against an empty security object which has no accounts.
            ICommandResult result = security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAccountAuthenticate,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "DoesNotExist",
                    generatedAuthenticatePassword
                })
            });

            // Validate that we get nothing back.
            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.DoesNotExists, result.Status);
        }

        /// <summary>
        ///     Test that setting a new password can be authenticated against
        /// </summary>
        [Test]
        public void TestSecurityAccountAuthenticateIncorrectPassword() {
            String generatedSetPassword = StringExtensions.RandomString(10);
            String generatedAuthenticatePassword = StringExtensions.RandomString(10);

            var security = new SecurityController();
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAddGroup,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName"
                })
            });

            // Test that the group was initially added.
            Assert.AreEqual(security.Groups.Last().Name, "GroupName");

            // Now add a user.
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupAddAccount,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName",
                    "Phogue"
                })
            });

            // Test that the account was added to the group.
            Assert.AreEqual(security.Groups.SelectMany(group => group.Accounts).First().Username, "Phogue");

            // Now change the password of the account.
            ICommandResult result = security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAccountSetPassword,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "Phogue",
                    generatedSetPassword
                })
            });

            // Make sure setting the password was successful.
            Assert.IsTrue(result.Success);
            Assert.AreEqual(result.Status, CommandResultType.Success);

            // Now validate that we can authenticate against the newly set password.
            result = security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAccountAuthenticate,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "Phogue",
                    generatedAuthenticatePassword
                })
            });

            // Validate that we could authenticate with our new password.
            Assert.IsFalse(result.Success);
            Assert.AreEqual(result.Status, CommandResultType.Failed);
        }

        /// <summary>
        ///     Tests that the account cannot be authenticated against if the account does not have permissions
        ///     to authenticate in the first place.
        /// </summary>
        [Test]
        public void TestSecurityAccountAuthenticateInsufficientPermission() {
            var security = new SecurityController();
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAddGroup,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName"
                })
            });
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupAddAccount,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName",
                    "Phogue"
                })
            });
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAccountSetPassword,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "Phogue",
                    "password"
                })
            });

            ICommandResult result = security.Tunnel(new Command() {
                CommandType = CommandType.SecurityAccountAuthenticate,
                Authentication = {
                    Username = "Phogue"
                },
                Origin = CommandOrigin.Remote,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "Phogue",
                    "password"
                })
            });

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.InsufficientPermissions, result.Status);
        }

        /// <summary>
        ///     Validates that we can't login to an account that has not had a password setup for it yet.
        /// </summary>
        [Test]
        public void TestSecurityAccountAuthenticateUnsetPassword() {
            var security = new SecurityController();
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAddGroup,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName"
                })
            });

            // Test that the group was initially added.
            Assert.AreEqual(security.Groups.Last().Name, "GroupName");

            // Now add a user.
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupAddAccount,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName",
                    "Phogue"
                })
            });

            // Test that the account was added to the group.
            Assert.AreEqual(security.Groups.SelectMany(group => group.Accounts).First().Username, "Phogue");

            // Now send an empty password through to authenticate against.
            ICommandResult result = security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAccountAuthenticate,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "Phogue",
                    String.Empty
                })
            });

            // Validate that we couldn't login because the server does not have a password set for it yet.
            Assert.IsFalse(result.Success);
            Assert.AreEqual(result.Status, CommandResultType.DoesNotExists);
        }

        /// <summary>
        ///     Test that setting a new password can be authenticated against
        /// </summary>
        [Test]
        public void TestSecurityAccountSetPassword() {
            String generatedPassword = StringExtensions.RandomString(10);

            var security = new SecurityController();
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAddGroup,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName"
                })
            });

            // Test that the group was initially added.
            Assert.AreEqual(security.Groups.Last().Name, "GroupName");

            // Now add a user.
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupAddAccount,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName",
                    "Phogue"
                })
            });

            // Test that the account was added to the group.
            Assert.AreEqual(security.Groups.SelectMany(group => group.Accounts).First().Username, "Phogue");

            // Now change the password of the account.
            ICommandResult result = security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAccountSetPassword,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "Phogue",
                    generatedPassword
                })
            });

            // Make sure setting the password was successful.
            Assert.IsTrue(result.Success);
            Assert.AreEqual(result.Status, CommandResultType.Success);

            // Now validate that we can authenticate against the newly set password.
            result = security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAccountAuthenticate,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "Phogue",
                    generatedPassword
                })
            });

            // Validate that we could authenticate with our new password.
            Assert.IsTrue(result.Success);
            Assert.AreEqual(result.Status, CommandResultType.Success);
        }

        /// <summary>
        ///     Tests we get an empty command result back if the account we try to set a password on
        ///     does not exist.
        /// </summary>
        [Test]
        public void TestSecurityAccountSetPasswordAccountDoesNotExist() {
            var security = new SecurityController();
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAddGroup,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName"
                })
            });
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupAddAccount,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName",
                    "ThisExists"
                })
            });

            // Now change the password of the account.
            ICommandResult result = security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAccountSetPassword,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "DoesNotExist",
                    "password"
                })
            });

            // Validate that we could not set a password and the result returned false.
            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.DoesNotExists, result.Status);
        }

        /// <summary>
        ///     Tests that we cannot set the password of an account if we do not have permission to do so.
        ///     Thinking about this, we may need to write this to allow users to set their own passwords. This should be
        ///     done within the SecurityController to determine if a CommandName can edit it's own account details.
        /// </summary>
        [Test]
        public void TestSecurityAccountSetPasswordAccountHashInsufficientPermission() {
            var security = new SecurityController();
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAddGroup,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName"
                })
            });
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupAddAccount,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName",
                    "Phogue"
                })
            });

            ICommandResult result = security.Tunnel(new Command() {
                CommandType = CommandType.SecurityAccountSetPasswordHash,
                Authentication = {
                    Username = "Phogue"
                },
                Origin = CommandOrigin.Remote,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "Phogue",
                    BCrypt.HashPassword("password", BCrypt.GenerateSalt())
                })
            });

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.InsufficientPermissions, result.Status);
        }

        /// <summary>
        ///     Tests that we cannot set the password of an account if we do not have permission to do so.
        ///     Thinking about this, we may need to write this to allow users to set their own passwords. This should be
        ///     done within the SecurityController to determine if a CommandName can edit it's own account details.
        /// </summary>
        [Test]
        public void TestSecurityAccountSetPasswordAccountInsufficientPermission() {
            var security = new SecurityController();
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAddGroup,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName"
                })
            });
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupAddAccount,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName",
                    "Phogue"
                })
            });

            ICommandResult result = security.Tunnel(new Command() {
                CommandType = CommandType.SecurityAccountSetPassword,
                Authentication = {
                    Username = "Phogue"
                },
                Origin = CommandOrigin.Remote,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "Phogue",
                    "password"
                })
            });

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.InsufficientPermissions, result.Status);
        }

        /// <summary>
        ///     Validates that we can't set an empty password.
        /// </summary>
        [Test]
        public void TestSecurityAccountSetPasswordEmptyPassword() {
            var security = new SecurityController();
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAddGroup,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName"
                })
            });

            // Test that the group was initially added.
            Assert.AreEqual(security.Groups.Last().Name, "GroupName");

            // Now add a user.
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupAddAccount,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName",
                    "Phogue"
                })
            });

            // Test that the account was added to the group.
            Assert.AreEqual(security.Groups.SelectMany(group => group.Accounts).First().Username, "Phogue");

            // Now change the password of the account.
            ICommandResult result = security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAccountSetPassword,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "Phogue",
                    String.Empty
                })
            });

            // Validate that we could not set a password and the result returned false.
            Assert.IsFalse(result.Success);
            Assert.AreEqual(result.Status, CommandResultType.InvalidParameter);
        }

        /// <summary>
        ///     Test that setting a new password can be authenticated against
        /// </summary>
        [Test]
        public void TestSecurityAccountSetPasswordHash() {
            String generatedPassword = StringExtensions.RandomString(10);
            String generatedPasswordHash = BCrypt.HashPassword(generatedPassword, BCrypt.GenerateSalt());

            var security = new SecurityController();
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAddGroup,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName"
                })
            });

            // Test that the group was initially added.
            Assert.AreEqual(security.Groups.Last().Name, "GroupName");

            // Now add a user.
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupAddAccount,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName",
                    "Phogue"
                })
            });

            // Test that the account was added to the group.
            Assert.AreEqual(security.Groups.SelectMany(group => group.Accounts).First().Username, "Phogue");

            // Now change the password of the account.
            ICommandResult result = security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAccountSetPasswordHash,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "Phogue",
                    generatedPasswordHash
                })
            });

            // Make sure setting the password was successful.
            Assert.IsTrue(result.Success);
            Assert.AreEqual(result.Status, CommandResultType.Success);

            // Now validate that we can authenticate against the newly set password.
            result = security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAccountAuthenticate,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "Phogue",
                    generatedPassword
                })
            });

            // Validate that we could authenticate with our new password.
            Assert.IsTrue(result.Success);
            Assert.AreEqual(result.Status, CommandResultType.Success);
        }

        /// <summary>
        ///     Tests we get an empty command result back if the account we try to set a password on
        ///     does not exist.
        /// </summary>
        [Test]
        public void TestSecurityAccountSetPasswordHashAccountDoesNotExist() {
            var security = new SecurityController();
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAddGroup,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName"
                })
            });

            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupAddAccount,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName",
                    "ThisExists"
                })
            });

            // Now change the password of the account.
            ICommandResult result = security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAccountSetPasswordHash,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "DoesNotExist",
                    BCrypt.HashPassword("password", BCrypt.GenerateSalt())
                })
            });

            // Validate that we could not set a password and the result returned false.
            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.DoesNotExists, result.Status);
        }

        /// <summary>
        ///     Validates that we can't set an empty password.
        /// </summary>
        [Test]
        public void TestSecurityAccountSetPasswordHashEmptyPassword() {
            var security = new SecurityController();
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAddGroup,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName"
                })
            });

            // Test that the group was initially added.
            Assert.AreEqual(security.Groups.Last().Name, "GroupName");

            // Now add a user.
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupAddAccount,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName",
                    "Phogue"
                })
            });

            // Test that the account was added to the group.
            Assert.AreEqual(security.Groups.SelectMany(group => group.Accounts).First().Username, "Phogue");

            // Now change the password of the account.
            ICommandResult result = security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAccountSetPasswordHash,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "Phogue",
                    String.Empty
                })
            });

            // Validate that we could not set a password and the result returned false.
            Assert.IsFalse(result.Success);
            Assert.AreEqual(result.Status, CommandResultType.InvalidParameter);
        }

        /// <summary>
        /// Tests that an account cannot set the preferred language of another account unless they
        /// have permission to do so.
        /// </summary>
        [Test]
        public void TestSecurityAccountSetPreferredLanguageAccountInsufficientPermission() {
            var security = new SecurityController();
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAddGroup,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName"
                })
            });
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupAddAccount,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName",
                    "Phogue"
                })
            });
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupAddAccount,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName",
                    "Ike"
                })
            });

            // Now change the language of the account.
            ICommandResult result = security.Tunnel(new Command() {
                CommandType = CommandType.SecurityAccountSetPreferredLanguageCode,
                Authentication = {
                    Username = "Phogue"
                },
                Origin = CommandOrigin.Remote,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "Ike",
                    "en-UK"
                })
            });

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.InsufficientPermissions, result.Status);
        }

        /// <summary>
        ///     Test setting the preferred language for an account works.
        /// </summary>
        [Test]
        public void TestSecurityAccountSetPreferredLanguageCode() {
            var security = new SecurityController();
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAddGroup,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName"
                })
            });

            // Test that the group was initially added.
            Assert.AreEqual(security.Groups.Last().Name, "GroupName");

            // Now add a user.
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupAddAccount,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName",
                    "Phogue"
                })
            });

            // Test that the account was added to the group.
            Assert.AreEqual(security.Groups.SelectMany(group => group.Accounts).First().Username, "Phogue");

            // Now change the language of the account.
            ICommandResult result = security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAccountSetPreferredLanguageCode,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "Phogue",
                    "de-DE"
                })
            });

            // Make sure it was successful.
            Assert.IsTrue(result.Success);
            Assert.AreEqual(result.Status, CommandResultType.Success);
            Assert.AreEqual(security.Groups.Last().Accounts.First().PreferredLanguageCode, "de-DE");
        }


        /// <summary>
        /// Tests that an account can set their own preferred language, even if their group does not have permission to do so.
        /// </summary>
        [Test]
        public void TestSecurityAccountSetPreferredLanguageCodeOwnAccount() {
            var security = new SecurityController();
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAddGroup,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName"
                })
            });
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupAddAccount,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName",
                    "Phogue"
                })
            });
            ICommandResult result = security.Tunnel(new Command() {
                Authentication = {
                    Username = "Phogue"
                },
                Origin = CommandOrigin.Remote,
                CommandType = CommandType.SecurityAccountSetPreferredLanguageCode,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "Phogue",
                    "de-DE"
                })
            });

            // Make sure it was successful.
            Assert.IsTrue(result.Success);
            Assert.AreEqual(result.Status, CommandResultType.Success);
            Assert.AreEqual(security.Groups.Last().Accounts.First().PreferredLanguageCode, "de-DE");
        }

        /// <summary>
        ///     Test that we get nothing back if the account does not exist.
        /// </summary>
        [Test]
        public void TestSecurityAccountSetPreferredLanguageCodeAccountDoesNotExist() {
            var security = new SecurityController();
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAddGroup,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName"
                })
            });

            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupAddAccount,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName",
                    "ThisExists"
                })
            });

            // Now change the language of the account.
            ICommandResult result = security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAccountSetPreferredLanguageCode,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "ThisDoesNotExist",
                    "en-UK"
                })
            });

            // Make sure we get nothing back if we try to change the language code of
            // an account that does not exist.
            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.DoesNotExists, result.Status);
        }

        /// <summary>
        ///     Test setting the preferred language for an account works.
        /// </summary>
        [Test]
        public void TestSecurityAccountSetPreferredLanguageCodeLanguageDoesNotExist() {
            var security = new SecurityController();
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAddGroup,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName"
                })
            });

            // Test that the group was initially added.
            Assert.AreEqual(security.Groups.Last().Name, "GroupName");

            // Now add a user.
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupAddAccount,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName",
                    "Phogue"
                })
            });

            // Test that the account was added to the group.
            Assert.AreEqual(security.Groups.SelectMany(group => group.Accounts).First().Username, "Phogue");

            // Now change the language of the account.
            ICommandResult result = security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAccountSetPreferredLanguageCode,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "Phogue",
                    "zu-ZU"
                })
            });

            // Make sure it was successful.
            Assert.IsFalse(result.Success);
            Assert.AreEqual(result.Status, CommandResultType.DoesNotExists);
            Assert.AreEqual(security.Groups.Last().Accounts.First().PreferredLanguageCode, String.Empty);
        }

        /// <summary>
        ///     Tests that adding a simpel account can be completed.
        /// </summary>
        [Test]
        public void TestSecurityGroupsAddAccountModel() {
            var security = new SecurityController();
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAddGroup,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName"
                })
            });

            // Test that the group was initially added.
            Assert.AreEqual(security.Groups.Last().Name, "GroupName");

            // Now add the user.
            ICommandResult result = security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupAddAccount,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName",
                    "Phogue"
                })
            });

            // Make sure the account was successfully created.
            Assert.IsTrue(result.Success);
            Assert.AreEqual(result.Status, CommandResultType.Success);
            Assert.AreEqual(security.Groups.SelectMany(group => group.Accounts).First().Username, "Phogue");
        }

        /// <summary>
        ///     Tests that an account cannot be added if the username is empty.
        /// </summary>
        [Test]
        public void TestSecurityGroupsAddAccountEmptyUsername() {
            var security = new SecurityController();
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAddGroup,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName"
                })
            });

            // Test that the group was initially added.
            Assert.AreEqual(security.Groups.Last().Name, "GroupName");

            // Now add the user.
            ICommandResult result = security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupAddAccount,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName",
                    String.Empty
                })
            });

            // Make sure the account was successfully created.
            Assert.IsFalse(result.Success);
            Assert.AreEqual(result.Status, CommandResultType.InvalidParameter);
        }

        /// <summary>
        ///     Tests that adding an account with a duplicate username will move the user to the new group.
        /// </summary>
        [Test]
        public void TestSecurityGroupsAddAccountExistingName() {
            var security = new SecurityController();
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAddGroup,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "FirstGroupName"
                })
            });

            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAddGroup,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "SecondGroupName"
                })
            });

            // Test that the groups were initially added.
            Assert.IsNotNull(security.Groups.Where(group => group.Name == "FirstGroupName").FirstOrDefault());
            Assert.IsNotNull(security.Groups.Where(group => group.Name == "SecondGroupName").FirstOrDefault());

            // Now add the user.
            ICommandResult result = security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupAddAccount,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "FirstGroupName",
                    "Phogue"
                })
            });

            // Now make sure the user was initially added.
            Assert.IsTrue(result.Success);
            Assert.AreEqual(result.Status, CommandResultType.Success);
            Assert.AreEqual(security.Groups.Where(group => group.Name == "FirstGroupName").SelectMany(group => group.Accounts).First().Username, "Phogue");
            Assert.IsNull(security.Groups.Where(group => group.Name == "SecondGroupName").SelectMany(group => group.Accounts).FirstOrDefault());

            // Now move the user to the second group.
            result = security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupAddAccount,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "SecondGroupName",
                    "Phogue"
                })
            });

            // Make sure setting the kick permission was successfull.
            Assert.IsTrue(result.Success);
            Assert.AreEqual(result.Status, CommandResultType.Success);
            Assert.IsNull(security.Groups.Where(group => group.Name == "FirstGroupName").SelectMany(group => group.Accounts).FirstOrDefault());
            Assert.AreEqual(security.Groups.Where(group => group.Name == "SecondGroupName").SelectMany(group => group.Accounts).First().Username, "Phogue");
        }

        /// <summary>
        ///     Tests that adding a simpel account can be completed.
        /// </summary>
        [Test]
        public void TestSecurityGroupsAddAccountGroupDoesNotExist() {
            var security = new SecurityController();

            // Add the user.
            ICommandResult result = security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupAddAccount,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "NonExistentGroup",
                    "Phogue"
                })
            });

            // Make sure the command returned nothing
            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.DoesNotExists, result.Status);
        }

        /// <summary>
        /// Tests that adding an account to a guest group will fail.
        /// </summary>
        [Test]
        public void TestSecurityGroupsAddAccountGroupGuest() {
            var security = new SecurityController();

            // Add the user.
            ICommandResult result = security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupAddAccount,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "Guest",
                    "Phogue"
                })
            });

            // Make sure the command returned nothing
            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.InvalidParameter, result.Status);
        }

        /// <summary>
        ///     Tests the command to add an account failes if the user has insufficient privileges.
        /// </summary>
        [Test]
        public void TestSecurityGroupsAddAccountInsufficientPermission() {
            var security = new SecurityController();
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAddGroup,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName"
                })
            });

            ICommandResult result = security.Tunnel(new Command() {
                CommandType = CommandType.SecurityGroupAddAccount,
                Authentication = {
                    Username = "Phogue"
                },
                Origin = CommandOrigin.Remote,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName",
                    "Phogue"
                })
            });

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.InsufficientPermissions, result.Status);
        }

        /// <summary>
        ///     Testing that an account can be removed by its name.
        /// </summary>
        [Test]
        public void TestSecurityRemoveAccountModel() {
            var security = new SecurityController();
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAddGroup,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName"
                })
            });
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupAddAccount,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName",
                    "Phogue"
                })
            });

            // Test that the group was initially added.
            Assert.AreEqual(security.Groups.Last().Accounts.First().Username, "Phogue");

            ICommandResult result = security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityRemoveAccount,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "Phogue"
                })
            });

            // Make sure it was successful.
            Assert.IsTrue(result.Success);
            Assert.AreEqual(0, security.Groups.Last().Accounts.Count);
        }

        /// <summary>
        /// Tests that an account can be removed if the command is locally called
        /// </summary>
        [Test]
        public void TestRemoveAccountByLocalSuccess() {
            var security = new SecurityController();
            security.Tunnel(CommandBuilder.SecurityAddGroup("GroupName").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupAddAccount("GroupName", "Phogue").SetOrigin(CommandOrigin.Local));

            ICommandResult result = security.Tunnel(CommandBuilder.SecurityRemoveAccount("Phogue").SetOrigin(CommandOrigin.Local));

            // Make sure the command failed. The user cannot remove their own account.
            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Success, result.Status);
        }

        /// <summary>
        /// Tests that an account will not be removed if the owner is attempting to remove it.
        /// </summary>
        [Test]
        public void TestRemoveOwnAccountFailure() {
            var security = new SecurityController();
            security.Tunnel(CommandBuilder.SecurityAddGroup("GroupName").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupAddAccount("GroupName", "Phogue").SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupSetPermission("GroupName", CommandType.SecurityRemoveAccount, 1).SetOrigin(CommandOrigin.Local));

            ICommandResult result = security.Tunnel(CommandBuilder.SecurityRemoveAccount("Phogue").SetOrigin(CommandOrigin.Remote).SetAuthentication(new CommandAuthenticationModel() {
                Username = "Phogue"
            }));

            // Make sure the command failed. The user cannot remove their own account.
            Assert.IsFalse(result.Success);
            Assert.AreEqual(result.Status, CommandResultType.InvalidParameter);
        }

        /// <summary>
        ///     Testing that you can't remove a group name that is empty.
        /// </summary>
        [Test]
        public void TestSecurityRemoveAccountEmptyAccountUsername() {
            var security = new SecurityController();

            // Add a group with an empty name.
            ICommandResult result = security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityRemoveAccount,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    String.Empty
                })
            });

            // Make sure adding an empty group fails.
            Assert.IsFalse(result.Success);
            Assert.AreEqual(result.Status, CommandResultType.InvalidParameter);
        }

        /// <summary>
        ///     Tests the command to add a player to an account fails if the user has insufficient privileges.
        /// </summary>
        [Test]
        public void TestSecurityRemoveAccountInsufficientPermission() {
            var security = new SecurityController();
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAddGroup,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName"
                })
            });
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupAddAccount,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName",
                    "Phogue"
                })
            });

            // Test that the group was initially added.
            Assert.AreEqual(security.Groups.Last().Accounts.First().Username, "Phogue");

            ICommandResult result = security.Tunnel(new Command() {
                CommandType = CommandType.SecurityRemoveAccount,
                Authentication = {
                    Username = "Phogue"
                },
                Origin = CommandOrigin.Remote,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "Phogue"
                })
            });

            // Make sure it was successful.
            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.InsufficientPermissions, result.Status);
        }

        /// <summary>
        ///     Testing that you can't remove a group that does not exist and provides correct errors.
        /// </summary>
        [Test]
        public void TestSecurityRemoveAccountNotExists() {
            var security = new SecurityController();
            ICommandResult result = security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityRemoveAccount,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "Phogue"
                })
            });

            // Make sure it was not successful.
            Assert.IsFalse(result.Success);
            Assert.AreEqual(result.Status, CommandResultType.DoesNotExists);
        }
    }
}