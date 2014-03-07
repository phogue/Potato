using System;
using System.Collections.Generic;
using NUnit.Framework;
using Procon.Core.Connections;
using Procon.Core.Connections.Plugins;
using Procon.Core.Shared;
using Procon.Core.Shared.Models;

namespace Procon.Core.Test.Plugins {
    [TestFixture]
    public class TestPluginConfig {
        /// <summary>
        /// Tests that an enabled plugin will have the enabled command saved to the config
        /// </summary>
        [Test]
        public void TestEnabledPluginSavedToConfig() {
            Guid connectionGuid = Guid.NewGuid();
            Guid onePluginGuid = Guid.NewGuid();
            Guid twoPluginGuid = Guid.NewGuid();

            ICorePluginController plugins = new CorePluginController() {
                Connection = new ConnectionController() {
                    ConnectionModel = new ConnectionModel() {
                        ConnectionGuid = connectionGuid
                    }
                },
                LoadedPlugins = new List<PluginModel>() {
                    new PluginModel() {
                        Name = "One",
                        IsEnabled = false,
                        PluginGuid = onePluginGuid
                    },
                    new PluginModel() {
                        Name = "Two",
                        IsEnabled = true,
                        PluginGuid = twoPluginGuid
                    }
                }
            };

            IConfig config = new Config().Create<CorePluginController>();

            plugins.WriteConfig(config);

            Assert.AreEqual(config.Root.First.Value<String>("Name"), "PluginsEnable");
            Assert.AreEqual(config.Root.First["ScopeModel"].Value<String>("ConnectionGuid"), connectionGuid.ToString());
            Assert.AreEqual(config.Root.First["ScopeModel"].Value<String>("PluginGuid"), twoPluginGuid.ToString());
        }
    }
}
