#region Copyright
// Copyright 2015 Geoff Green.
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
#region

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Newtonsoft.Json.Linq;
using Potato.Core.Security;
using Potato.Core.Shared;
using Potato.Core.Shared.Models;
using Potato.Core.Shared.Serialization;
using Potato.Net.Shared.Protocols;

#endregion

namespace Potato.Core.Test.Security {
    [TestFixture]
    public class TestSecurity {
        [SetUp]
        public void Initialize() {
            SharedReferences.Setup();

            if (File.Exists(ConfigFileInfo.FullName)) {
                File.Delete(ConfigFileInfo.FullName);
            }
        }

        protected static FileInfo ConfigFileInfo = new FileInfo("Potato.Core.Test.Security.xml");

        /// <summary>
        ///     Tests that when disposing of the security object, all other items are cleaned up.
        /// </summary>
        [Test]
        public void TestSecurityDispose() {
            var security = new SecurityController();
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAddGroup,
                Parameters = TestHelpers.ObjectListToContentList(new List<object>() {
                    "GroupName"
                })
            });
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupSetPermission,
                Parameters = TestHelpers.ObjectListToContentList(new List<object>() {
                    "GroupName",
                    CommandType.VariablesSet,
                    77
                })
            });

            security.Tunnel(CommandBuilder.SecurityGroupAppendPermissionTrait("GroupName", CommandType.VariablesSet.ToString(), PermissionTraitsType.Boolean).SetOrigin(CommandOrigin.Local));
            security.Tunnel(CommandBuilder.SecurityGroupSetPermissionDescription("GroupName", CommandType.VariablesSet.ToString(), "Description!").SetOrigin(CommandOrigin.Local));

            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupAddAccount,
                Parameters = TestHelpers.ObjectListToContentList(new List<object>() {
                    "GroupName",
                    "Phogue"
                })
            });
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAccountSetPassword,
                Parameters = TestHelpers.ObjectListToContentList(new List<object>() {
                    "Phogue",
                    "password"
                })
            });
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAccountSetPreferredLanguageCode,
                Parameters = TestHelpers.ObjectListToContentList(new List<object>() {
                    "Phogue",
                    "de-DE"
                })
            });
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAccountAddPlayer,
                Parameters = TestHelpers.ObjectListToContentList(new List<object>() {
                    "Phogue",
                    CommonProtocolType.DiceBattlefield3,
                    "ABCDEF"
                })
            });

            var group = security.Groups.Last();
            var account = group.Accounts.First();
            var permission = group.Permissions.First(p => p.Name == CommandType.VariablesSet.ToString());
            var accountPlayer = account.Players.First();

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
            Assert.IsNull(permission.Traits);
            Assert.IsNull(permission.Description);

            Assert.AreEqual(CommonProtocolType.None, accountPlayer.ProtocolType);
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
                Parameters = TestHelpers.ObjectListToContentList(new List<object>() {
                    "GroupName"
                })
            });
            saveSecurity.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupSetPermission,
                Parameters = TestHelpers.ObjectListToContentList(new List<object>() {
                    "GroupName",
                    "CustomPermission",
                    22
                })
            });
            saveSecurity.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupSetPermission,
                Parameters = TestHelpers.ObjectListToContentList(new List<object>() {
                    "GroupName",
                    CommandType.VariablesSet,
                    77
                })
            });
            saveSecurity.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupSetPermission,
                Parameters = TestHelpers.ObjectListToContentList(new List<object>() {
                    "GroupName",
                    CommandType.VariablesSetA,
                    88
                })
            });
            saveSecurity.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupAddAccount,
                Parameters = TestHelpers.ObjectListToContentList(new List<object>() {
                    "GroupName",
                    "Phogue"
                })
            });
            saveSecurity.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAccountSetPassword,
                Parameters = TestHelpers.ObjectListToContentList(new List<object>() {
                    "Phogue",
                    "password"
                })
            });

            saveSecurity.Tunnel(CommandBuilder.SecurityAccountAppendAccessToken("Phogue", Guid.Parse("f380eb1e-1438-48c0-8c3d-ad55f2d40538"), "Token Hash", DateTime.Parse("2024-04-14 20:51:00 PM")).SetOrigin(CommandOrigin.Local));

            saveSecurity.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAccountSetPreferredLanguageCode,
                Parameters = TestHelpers.ObjectListToContentList(new List<object>() {
                    "Phogue",
                    "de-DE"
                })
            });
            saveSecurity.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAccountAddPlayer,
                Parameters = TestHelpers.ObjectListToContentList(new List<object>() {
                    "Phogue",
                    CommonProtocolType.DiceBattlefield3,
                    "ABCDEF"
                })
            });

            // Save a config of the security controller
            var saveConfig = new Config();
            saveConfig.Create(typeof(SecurityController));
            saveSecurity.WriteConfig(saveConfig);
            saveConfig.Save(ConfigFileInfo);

            // Load the config in a new config.
            var loadSecurity = (SecurityController)new SecurityController().Execute();
            var loadConfig = new Config();
            loadConfig.Load(ConfigFileInfo);
            loadSecurity.Execute(loadConfig);

            var lastGroup = loadSecurity.Groups.LastOrDefault(group => @group.Name == "GroupName") ?? new GroupModel();

            Assert.AreEqual("Guest", loadSecurity.Groups.First().Name);
            Assert.AreEqual("GroupName", loadSecurity.Groups.Last().Name);
            Assert.AreEqual(22, lastGroup.Permissions.First(permission => permission.Name == "CustomPermission").Authority);
            Assert.AreEqual(77, lastGroup.Permissions.First(permission => permission.Name == CommandType.VariablesSet.ToString()).Authority);
            Assert.AreEqual(88, lastGroup.Permissions.First(permission => permission.Name == CommandType.VariablesSetA.ToString()).Authority);
            Assert.AreEqual("Phogue", loadSecurity.Groups.SelectMany(group => group.Accounts).First().Username);
            Assert.AreEqual(Guid.Parse("f380eb1e-1438-48c0-8c3d-ad55f2d40538"), loadSecurity.Groups.SelectMany(group => group.Accounts).First().AccessTokens.First().Value.Id);
            Assert.AreEqual("Token Hash", loadSecurity.Groups.SelectMany(group => group.Accounts).First().AccessTokens.First().Value.TokenHash);
            Assert.AreEqual(DateTime.Parse("2024-04-14 20:51:00 PM"), loadSecurity.Groups.SelectMany(group => group.Accounts).First().AccessTokens.First().Value.LastTouched);
            Assert.AreEqual("de-DE", loadSecurity.Groups.Last().Accounts.First().PreferredLanguageCode);
            Assert.AreEqual(CommonProtocolType.DiceBattlefield3, loadSecurity.Groups.SelectMany(group => group.Accounts).SelectMany(account => account.Players).First().ProtocolType);
            Assert.AreEqual("ABCDEF", loadSecurity.Groups.SelectMany(group => group.Accounts).SelectMany(account => account.Players).First().Uid);

            // Now validate that we can authenticate against the loaded in password
            var result = loadSecurity.Tunnel(CommandBuilder.SecurityAccountAuthenticate("Phogue", "password", string.Empty).SetOrigin(CommandOrigin.Local));

            // Validate that we could authenticate with our new password.
            Assert.IsTrue(result.Success);
            Assert.AreEqual(result.CommandResultType, CommandResultType.Success);
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
                Parameters = TestHelpers.ObjectListToContentList(new List<object>() {
                    "GroupName"
                })
            });
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupSetPermission,
                Parameters = TestHelpers.ObjectListToContentList(new List<object>() {
                    "GroupName",
                    "CustomPermission",
                    22
                })
            });
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupSetPermission,
                Parameters = TestHelpers.ObjectListToContentList(new List<object>() {
                    "GroupName",
                    CommandType.VariablesSet,
                    77
                })
            });
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupSetPermission,
                Parameters = TestHelpers.ObjectListToContentList(new List<object>() {
                    "GroupName",
                    CommandType.VariablesSetA,
                    88
                })
            });
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityGroupAddAccount,
                Parameters = TestHelpers.ObjectListToContentList(new List<object>() {
                    "GroupName",
                    "Phogue"
                })
            });
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAccountSetPassword,
                Parameters = TestHelpers.ObjectListToContentList(new List<object>() {
                    "Phogue",
                    "password"
                })
            });

            security.Tunnel(CommandBuilder.SecurityAccountAppendAccessToken("Phogue", Guid.Parse("f380eb1e-1438-48c0-8c3d-ad55f2d40538"), "Token Hash", DateTime.Parse("2024-04-14 20:51:00 PM")).SetOrigin(CommandOrigin.Local));

            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAccountSetPreferredLanguageCode,
                Parameters = TestHelpers.ObjectListToContentList(new List<object>() {
                    "Phogue",
                    "de-DE"
                })
            });
            security.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.SecurityAccountAddPlayer,
                Parameters = TestHelpers.ObjectListToContentList(new List<object>() {
                    "Phogue",
                    CommonProtocolType.DiceBattlefield3,
                    "ABCDEF"
                })
            });

            // Save a config of the language controller
            var saveConfig = new Config();
            saveConfig.Create(typeof(SecurityController));
            security.WriteConfig(saveConfig);
            saveConfig.Save(ConfigFileInfo);

            // Load the config in a new config.
            var loadConfig = new Config();
            loadConfig.Load(ConfigFileInfo);

            var commands = loadConfig.RootOf<SecurityController>().Children<JObject>().Select(item => item.ToObject<IConfigCommand>(JsonSerialization.Minimal)).ToList();

            Assert.AreEqual("SecurityAddGroup", commands[0].Command.Name);
            Assert.AreEqual("Guest", commands[0].Command.Parameters[0].First<string>());

            Assert.AreEqual("SecurityAddGroup", commands[1].Command.Name);
            Assert.AreEqual("GroupName", commands[1].Command.Parameters[0].First<string>());

            Assert.AreEqual("SecurityGroupSetPermission", commands[2].Command.Name);
            Assert.AreEqual("GroupName", commands[2].Command.Parameters[0].First<string>());
            Assert.AreEqual(CommandType.VariablesSet.ToString(), commands[2].Command.Parameters[1].First<string>());
            Assert.AreEqual("77", commands[2].Command.Parameters[2].First<string>());

            Assert.AreEqual("SecurityGroupSetPermission", commands[3].Command.Name);
            Assert.AreEqual("GroupName", commands[3].Command.Parameters[0].First<string>());
            Assert.AreEqual(CommandType.VariablesSetA.ToString(), commands[3].Command.Parameters[1].First<string>());
            Assert.AreEqual("88", commands[3].Command.Parameters[2].First<string>());

            Assert.AreEqual("SecurityGroupSetPermission", commands[4].Command.Name);
            Assert.AreEqual("GroupName", commands[4].Command.Parameters[0].First<string>());
            Assert.AreEqual("CustomPermission", commands[4].Command.Parameters[1].First<string>());
            Assert.AreEqual("22", commands[4].Command.Parameters[2].First<string>());

            Assert.AreEqual("SecurityGroupAddAccount", commands[5].Command.Name);
            Assert.AreEqual("GroupName", commands[5].Command.Parameters[0].First<string>());
            Assert.AreEqual("Phogue", commands[5].Command.Parameters[1].First<string>());

            Assert.AreEqual("SecurityAccountSetPasswordHash", commands[6].Command.Name);
            Assert.AreEqual("Phogue", commands[6].Command.Parameters[0].First<string>());
            // We can only test if this isn't null as it contains a random salt and resulting hash.
            Assert.IsNotNull(commands[6].Command.Parameters[1].First<string>());

            Assert.AreEqual("SecurityAccountSetPreferredLanguageCode", commands[7].Command.Name);
            Assert.AreEqual("Phogue", commands[7].Command.Parameters[0].First<string>());
            Assert.AreEqual("de-DE", commands[7].Command.Parameters[1].First<string>());

            Assert.AreEqual("SecurityAccountAddPlayer", commands[8].Command.Name);
            Assert.AreEqual("Phogue", commands[8].Command.Parameters[0].First<string>());
            Assert.AreEqual(CommonProtocolType.DiceBattlefield3, commands[8].Command.Parameters[1].First<string>());
            Assert.AreEqual("ABCDEF", commands[8].Command.Parameters[2].First<string>());

            Assert.AreEqual("SecurityAccountAppendAccessToken", commands[9].Command.Name);
            Assert.AreEqual("Phogue", commands[9].Command.Parameters[0].First<string>());
            Assert.AreEqual(Guid.Parse("f380eb1e-1438-48c0-8c3d-ad55f2d40538"), commands[9].Command.Parameters[1].First<Guid>());
            Assert.AreEqual("Token Hash", commands[9].Command.Parameters[2].First<string>());
            Assert.AreEqual(DateTime.Parse("2024-04-14 20:51:00 PM"), commands[9].Command.Parameters[3].First<DateTime>());
        }
    }
}