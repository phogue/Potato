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
using System.Linq;
using NUnit.Framework;
using Potato.Core.Connections.Plugins;
using Potato.Core.Security;
using Potato.Core.Shared;

#endregion

namespace Potato.Core.Test.Plugins {
    [TestFixture]
    public class TestPluginsEnabled {
        [SetUp]
        public void Initialize() {
            SharedReferences.Setup();
        }

        /// <summary>
        ///     Tests that a plugin can be enabled if the parameter matches up to an existing plugin
        ///     and the user has permission
        /// </summary>
        [Test]
        public void TestPluginEnable() {
            var security = new SecurityController();
            var plugins = (CorePluginController)new CorePluginController() {
                Shared = {
                    Security = security
                }
            }.Execute();

            var result = plugins.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.PluginsEnable,
                Scope = {
                    PluginGuid = plugins.LoadedPlugins.First().PluginGuid
                }
            });

            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Success, result.CommandResultType);

            //plugins.Dispose();
        }

        /// <summary>
        ///     Tests that a plugin will return false if it is already enabled
        /// </summary>
        [Test]
        public void TestPluginEnableAlreadyEnabled() {
            var security = new SecurityController();
            var plugins = (CorePluginController)new CorePluginController() {
                Shared = {
                    Security = security
                }
            }.Execute();

            plugins.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.PluginsEnable,
                Scope = {
                    PluginGuid = plugins.LoadedPlugins.First().PluginGuid
                }
            });

            var result = plugins.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.PluginsEnable,
                Scope = {
                    PluginGuid = plugins.LoadedPlugins.First().PluginGuid
                }
            });

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.Failed, result.CommandResultType);
        }

        /// <summary>
        ///     Tests that enabling a plugin will allow commands within that plugin to be executed.
        /// </summary>
        [Test]
        public void TestPluginEnableCommandSuccessful() {
            var security = new SecurityController();
            var plugins = (CorePluginController)new CorePluginController() {
                Shared = {
                    Security = security
                }
            }.Execute();

            var result = plugins.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.PluginsEnable,
                Scope = {
                    PluginGuid = plugins.LoadedPlugins.First().PluginGuid
                }
            });

            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Success, result.CommandResultType);

            result = plugins.Tunnel(new Command() {
                Name = "TestPluginsEnabledCommandResult",
                Authentication = {
                    Username = "Phogue"
                },
                Parameters = TestHelpers.ObjectListToContentList(new List<object>() {
                    "Return Message"
                })
            });

            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Success, result.CommandResultType);
            Assert.AreEqual("Return Message", result.Message);
        }

        /// <summary>
        ///     Tests that a plugin will return a DoesNotExist message if the player guid does not
        ///     match up to any loaded plugin
        /// </summary>
        [Test]
        public void TestPluginEnableDoesNotExist() {
            var security = new SecurityController();
            var plugins = (CorePluginController)new CorePluginController() {
                Shared = {
                    Security = security
                }
            }.Execute();

            var result = plugins.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.PluginsEnable,
                Scope = {
                    PluginGuid = Guid.NewGuid()
                }
            });

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.DoesNotExists, result.CommandResultType);
        }

        /// <summary>
        ///     Tests that a plugin cannot be enabled if the user has insufficient permissions to do so.
        /// </summary>
        [Test]
        public void TestPluginEnableInsufficientPermission() {
            var security = new SecurityController();
            var plugins = (CorePluginController)new CorePluginController() {
                Shared = {
                    Security = security
                }
            }.Execute();

            var result = plugins.Tunnel(new Command() {
                Origin = CommandOrigin.Remote,
                Authentication = {
                    Username = "Phogue"
                },
                CommandType = CommandType.PluginsEnable,
                Scope = {
                    PluginGuid = plugins.LoadedPlugins.First().PluginGuid
                }
            });

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.InsufficientPermissions, result.CommandResultType);
        }
    }
}