using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Procon.Core.Security;

namespace Procon.Core.Test.Plugins {
    using Procon.Core.Connections.Plugins;
    [TestFixture]
    public class TestPluginsDisabled {

        /// <summary>
        /// Tests that a plugin can be Disabled if the parameter matches up to an existing plugin
        /// and the user has permission
        /// </summary>
        [Test]
        public void TestPluginDisable() {
            SecurityController security = new SecurityController();
            PluginController plugins = new PluginController() {
                Security = security
            }.Execute() as PluginController;

            plugins.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.PluginsEnable,
                Scope = {
                    PluginGuid = plugins.Plugins.First().PluginGuid
                }
            });

            CommandResultArgs result = plugins.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.PluginsDisable,
                Scope = {
                    PluginGuid = plugins.Plugins.First().PluginGuid
                }
            });

            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Success, result.Status);
        }

        /// <summary>
        /// Tests that a plugin will return false if it is already disabled
        /// </summary>
        [Test]
        public void TestPluginDisableAlreadyDisabled() {
            SecurityController security = new SecurityController();
            PluginController plugins = new PluginController() {
                Security = security
            }.Execute() as PluginController;

            CommandResultArgs result = plugins.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.PluginsDisable,
                Scope = {
                    PluginGuid = plugins.Plugins.First().PluginGuid
                }
            });

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.Failed, result.Status);
        }

        /// <summary>
        /// Tests that a plugin will return a DoesNotExist message if the player guid does not
        /// match up to any loaded plugin
        /// </summary>
        [Test]
        public void TestPluginDisableDoesNotExist() {
            SecurityController security = new SecurityController();
            PluginController plugins = new PluginController() {
                Security = security
            }.Execute() as PluginController;

            CommandResultArgs result = plugins.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.PluginsDisable,
                Scope = {
                    PluginGuid = Guid.NewGuid()
                }
            });

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.DoesNotExists, result.Status);
        }
        
        /// <summary>
        /// Tests that a plugin cannot be disabled if the user has insufficient permissions to do so.
        /// </summary>
        [Test]
        public void TestPluginDisableInsufficientPermission() {
            SecurityController security = new SecurityController();
            PluginController plugins = new PluginController() {
                Security = security
            }.Execute() as PluginController;

            CommandResultArgs result = plugins.Tunnel(new Command() {
                Origin = CommandOrigin.Remote,
                Username = "Phogue",
                CommandType = CommandType.PluginsDisable,
                Scope = {
                    PluginGuid = plugins.Plugins.First().PluginGuid
                }
            });

            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.InsufficientPermissions, result.Status);
        }

        /// <summary>
        /// Tests that disabling a plugin will allow commands within that plugin to be executed.
        /// </summary>
        [Test]
        public void TestPluginDisableCommandSuccessful() {
            SecurityController security = new SecurityController();
            PluginController plugins = new PluginController() {
                Security = security
            }.Execute() as PluginController;

            // The plugin will be disabled right now.
            CommandResultArgs result = plugins.Tunnel(new Command() {
                Name = "TestPluginsDisabledCommandResult",
                Username = "Phogue",
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "Return Message"
                })
            });

            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Continue, result.Status);
            Assert.AreEqual("", result.Message);
        }
    }
}
