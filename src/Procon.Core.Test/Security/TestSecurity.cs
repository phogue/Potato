#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Newtonsoft.Json.Linq;
using Procon.Core.Security;
using Procon.Core.Shared;
using Procon.Core.Shared.Models;
using Procon.Net.Shared.Protocols;

#endregion

namespace Procon.Core.Test.Security {
    [TestFixture]
    public class TestSecurity {
        [SetUp]
        public void Initialize() {
            SharedReferences.Setup();

            if (File.Exists(ConfigFileInfo.FullName)) {
                File.Delete(ConfigFileInfo.FullName);
            }
        }

        protected static FileInfo ConfigFileInfo = new FileInfo("Procon.Core.Test.Security.xml");

        /// <summary>
        ///     Tests that when disposing of the security object, all other items are cleaned up.
        /// </summary>
        [Test]
        public void TestSecurityDispose() {
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
                CommandType = CommandType.SecurityGroupSetPermission,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName",
                    CommandType.VariablesSet,
                    77
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
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAccountSetPreferredLanguageCode,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "Phogue",
                    "de-DE"
                })
            });
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAccountAddPlayer,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "Phogue",
                    CommonGameType.DiceBattlefield3,
                    "ABCDEF"
                })
            });

            GroupModel group = security.Groups.First();
            AccountModel account = group.Accounts.First();
            PermissionModel permission = group.Permissions.First(p => p.Name == CommandType.VariablesSet.ToString());
            AccountPlayerModel accountPlayer = account.Players.First();

            security.Dispose();

            // Test that all the lists and data within each item has been nulled.
            Assert.IsNull(security.Groups);

            Assert.IsNull(group.Name);
            Assert.IsNull(group.Permissions);
            Assert.IsNull(group.Accounts);

            Assert.IsNull(account.Username);
            Assert.IsNull(account.PreferredLanguageCode);
            Assert.IsNull(account.PasswordHash);
            Assert.IsNull(account.Players);
            Assert.IsNull(account.Group);

            Assert.AreEqual(CommandType.None, permission.CommandType);
            Assert.IsNull(permission.Name);
            Assert.IsNull(permission.Authority);

            Assert.AreEqual(CommonGameType.None, accountPlayer.GameType);
            Assert.IsNull(accountPlayer.Uid);
            Assert.IsNull(accountPlayer.Account);
        }

        /// <summary>
        ///     Tests that a config can be successfully loaded
        /// </summary>
        [Test]
        public void TestSecurityLoadConfig() {
            var saveSecurity = new SecurityController();
            saveSecurity.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAddGroup,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName"
                })
            });
            saveSecurity.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupSetPermission,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName",
                    "CustomPermission",
                    22
                })
            });
            saveSecurity.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupSetPermission,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName",
                    CommandType.VariablesSet,
                    77
                })
            });
            saveSecurity.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupSetPermission,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName",
                    CommandType.VariablesSetA,
                    88
                })
            });
            saveSecurity.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupAddAccount,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName",
                    "Phogue"
                })
            });
            saveSecurity.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAccountSetPassword,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "Phogue",
                    "password"
                })
            });
            saveSecurity.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAccountSetPreferredLanguageCode,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "Phogue",
                    "de-DE"
                })
            });
            saveSecurity.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAccountAddPlayer,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "Phogue",
                    CommonGameType.DiceBattlefield3,
                    "ABCDEF"
                })
            });

            // Save a config of the security controller
            var saveConfig = new Config();
            saveConfig.Create(typeof (SecurityController));
            saveSecurity.WriteConfig(saveConfig);
            saveConfig.Save(ConfigFileInfo);

            // Load the config in a new config.
            var loadSecurity = (SecurityController)new SecurityController().Execute();
            var loadConfig = new Config();
            loadConfig.Load(ConfigFileInfo);
            loadSecurity.Execute(loadConfig);

            Assert.AreEqual("GroupName", loadSecurity.Groups.First().Name);
            Assert.AreEqual(22, loadSecurity.Groups.FirstOrDefault(group => group.Name == "GroupName").Permissions.Where(permission => permission.Name == "CustomPermission").First().Authority);
            Assert.AreEqual(77, loadSecurity.Groups.FirstOrDefault(group => group.Name == "GroupName").Permissions.Where(permission => permission.Name == CommandType.VariablesSet.ToString()).First().Authority);
            Assert.AreEqual(88, loadSecurity.Groups.FirstOrDefault(group => group.Name == "GroupName").Permissions.Where(permission => permission.Name == CommandType.VariablesSetA.ToString()).First().Authority);
            Assert.AreEqual("Phogue", loadSecurity.Groups.SelectMany(group => group.Accounts).First().Username);
            Assert.AreEqual("de-DE", loadSecurity.Groups.First().Accounts.First().PreferredLanguageCode);
            Assert.AreEqual(CommonGameType.DiceBattlefield3, loadSecurity.Groups.SelectMany(group => group.Accounts).SelectMany(account => account.Players).First().GameType);
            Assert.AreEqual("ABCDEF", loadSecurity.Groups.SelectMany(group => group.Accounts).SelectMany(account => account.Players).First().Uid);

            // Now validate that we can authenticate against the loaded in password
            CommandResult result = loadSecurity.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAccountAuthenticate,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "Phogue",
                    "password"
                })
            });

            // Validate that we could authenticate with our new password.
            Assert.IsTrue(result.Success);
            Assert.AreEqual(result.Status, CommandResultType.Success);
        }

        /// <summary>
        ///     Tests that a config can be written in a specific format.
        /// </summary>
        [Test]
        public void TestSecurityWriteConfig() {
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
                CommandType = CommandType.SecurityGroupSetPermission,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName",
                    "CustomPermission",
                    22
                })
            });
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupSetPermission,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName",
                    CommandType.VariablesSet,
                    77
                })
            });
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupSetPermission,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "GroupName",
                    CommandType.VariablesSetA,
                    88
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
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAccountSetPreferredLanguageCode,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "Phogue",
                    "de-DE"
                })
            });
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAccountAddPlayer,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "Phogue",
                    CommonGameType.DiceBattlefield3,
                    "ABCDEF"
                })
            });

            // Save a config of the language controller
            var saveConfig = new Config();
            saveConfig.Create(typeof (SecurityController));
            security.WriteConfig(saveConfig);
            saveConfig.Save(ConfigFileInfo);

            // Load the config in a new config.
            var loadConfig = new Config();
            loadConfig.Load(ConfigFileInfo);

            var commands = loadConfig.RootOf<SecurityController>().Children<JObject>().Select(item => item.ToObject<Command>()).ToList();

            Assert.AreEqual("SecurityAddGroup", commands[0].Name);
            Assert.AreEqual("GroupName", commands[0].Parameters[0].First<String>());

            Assert.AreEqual("SecurityGroupSetPermission", commands[1].Name);
            Assert.AreEqual("GroupName", commands[1].Parameters[0].First<String>());
            Assert.AreEqual(CommandType.VariablesSet.ToString(), commands[1].Parameters[1].First<String>());
            Assert.AreEqual("77", commands[1].Parameters[2].First<String>());

            Assert.AreEqual("SecurityGroupSetPermission", commands[2].Name);
            Assert.AreEqual("GroupName", commands[2].Parameters[0].First<String>());
            Assert.AreEqual(CommandType.VariablesSetA.ToString(), commands[2].Parameters[1].First<String>());
            Assert.AreEqual("88", commands[2].Parameters[2].First<String>());

            Assert.AreEqual("SecurityGroupSetPermission", commands[3].Name);
            Assert.AreEqual("GroupName", commands[3].Parameters[0].First<String>());
            Assert.AreEqual("CustomPermission", commands[3].Parameters[1].First<String>());
            Assert.AreEqual("22", commands[3].Parameters[2].First<String>());

            Assert.AreEqual("SecurityGroupAddAccount", commands[4].Name);
            Assert.AreEqual("GroupName", commands[4].Parameters[0].First<String>());
            Assert.AreEqual("Phogue", commands[4].Parameters[1].First<String>());

            Assert.AreEqual("SecurityAccountSetPasswordHash", commands[5].Name);
            Assert.AreEqual("Phogue", commands[5].Parameters[0].First<String>());
            // We can only test if this isn't null as it contains a random salt and resulting hash.
            Assert.IsNotNull(commands[5].Parameters[1].First<String>());

            Assert.AreEqual("SecurityAccountSetPreferredLanguageCode", commands[6].Name);
            Assert.AreEqual("Phogue", commands[6].Parameters[0].First<String>());
            Assert.AreEqual("de-DE", commands[6].Parameters[1].First<String>());

            Assert.AreEqual("SecurityAccountAddPlayer", commands[7].Name);
            Assert.AreEqual("Phogue", commands[7].Parameters[0].First<String>());
            Assert.AreEqual(CommonGameType.DiceBattlefield3, commands[7].Parameters[1].First<String>());
            Assert.AreEqual("ABCDEF", commands[7].Parameters[2].First<String>());
        }
    }
}