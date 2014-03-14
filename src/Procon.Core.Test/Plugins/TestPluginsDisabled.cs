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
    public class TestPluginsDisabled {
        [SetUp]
        public void Initialize() {
            SharedReferences.Setup();
        }

        /// <summary>
        ///     Tests that a plugin can be Disabled if the parameter matches up to an existing plugin
        ///     and the user has permission
        /// </summary>
        [Test]
        public void TestPluginDisable() {
            var security = new SecurityController();
            var plugins = (CorePluginController)new CorePluginController() {
                Shared = {
                    Security = security
                }
            }.Execute();

            plugins.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.PluginsEnable,
                ScopeModel = {
                    PluginGuid = plugins.LoadedPlugins.First().PluginGuid
                }
            });

            ICommandResult result = plugins.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.PluginsDisable,
                ScopeModel = {
                    PluginGuid = plugins.LoadedPlugins.First().PluginGuid
                }
            });

            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Success, result.CommandResultType);
        }

        /// <summary>
        ///     Tests that a plugin will return false if it is already disabled
        /// </summary>
        [Test]
        public void TestPluginDisableAlreadyDisabled() {
            var security = new SecurityController();
            var plugins = (CorePluginController)new CorePluginController() {
                Shared = {
                    Security = security
                }
            }.Execute();

            ICommandResult result = plugins.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.PluginsDisable,
                ScopeModel = {
                    PluginGuid = plugins.LoadedPlugins.First().PluginGuid
                }
            });

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.Failed, result.CommandResultType);
        }

        /// <summary>
        ///     Tests that disabling a plugin will allow commands within that plugin to be executed.
        /// </summary>
        [Test]
        public void TestPluginDisableCommandSuccessful() {
            var security = new SecurityController();
            var plugins = (CorePluginController)new CorePluginController() {
                Shared = {
                    Security = security
                }
            }.Execute();

            // The plugin will be disabled right now.
            ICommandResult result = plugins.Tunnel(new Command() {
                Name = "TestPluginsDisabledCommandResult",
                Authentication = {
                    Username = "Phogue"
                },
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "Return Message"
                })
            });

            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Continue, result.CommandResultType);
            Assert.AreEqual("", result.Message);
        }

        /// <summary>
        ///     Tests that a plugin will return a DoesNotExist message if the player guid does not
        ///     match up to any loaded plugin
        /// </summary>
        [Test]
        public void TestPluginDisableDoesNotExist() {
            var security = new SecurityController();
            var plugins = (CorePluginController)new CorePluginController() {
                Shared = {
                    Security = security
                }
            }.Execute();

            ICommandResult result = plugins.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.PluginsDisable,
                ScopeModel = {
                    PluginGuid = Guid.NewGuid()
                }
            });

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.DoesNotExists, result.CommandResultType);
        }

        /// <summary>
        ///     Tests that a plugin cannot be disabled if the user has insufficient permissions to do so.
        /// </summary>
        [Test]
        public void TestPluginDisableInsufficientPermission() {
            var security = new SecurityController();
            var plugins = (CorePluginController)new CorePluginController() {
                Shared = {
                    Security = security
                }
            }.Execute();

            ICommandResult result = plugins.Tunnel(new Command() {
                Origin = CommandOrigin.Remote,
                Authentication = {
                    Username = "Phogue"
                },
                CommandType = CommandType.PluginsDisable,
                ScopeModel = {
                    PluginGuid = plugins.LoadedPlugins.First().PluginGuid
                }
            });

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.InsufficientPermissions, result.CommandResultType);
        }
    }
}