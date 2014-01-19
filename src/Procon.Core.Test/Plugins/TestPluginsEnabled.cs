#region

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Procon.Core.Connections.Plugins;
using Procon.Core.Security;
using Procon.Core.Shared;

#endregion

namespace Procon.Core.Test.Plugins {
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

            CommandResult result = plugins.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.PluginsEnable,
                Scope = {
                    PluginGuid = plugins.LoadedPlugins.First().PluginGuid
                }
            });

            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Success, result.Status);

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

            CommandResult result = plugins.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.PluginsEnable,
                Scope = {
                    PluginGuid = plugins.LoadedPlugins.First().PluginGuid
                }
            });

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.Failed, result.Status);
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

            CommandResult result = plugins.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.PluginsEnable,
                Scope = {
                    PluginGuid = plugins.LoadedPlugins.First().PluginGuid
                }
            });

            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Success, result.Status);

            result = plugins.Tunnel(new Command() {
                Name = "TestPluginsEnabledCommandResult",
                Authentication = {
                    Username = "Phogue"
                },
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "Return Message"
                })
            });

            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Success, result.Status);
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

            CommandResult result = plugins.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.PluginsEnable,
                Scope = {
                    PluginGuid = Guid.NewGuid()
                }
            });

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.DoesNotExists, result.Status);
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

            CommandResult result = plugins.Tunnel(new Command() {
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
            Assert.AreEqual(CommandResultType.InsufficientPermissions, result.Status);
        }
    }
}