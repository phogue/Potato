using System;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Procon.Core.Connections;
using Procon.Core.Connections.Plugins;
using Procon.Core.Shared;
using Procon.Core.Shared.Models;

namespace Procon.Core.Test.Plugins {
    [TestFixture]
    public class TestPluginConfig {
        /// <summary>
        /// Test file for us to save/load from.
        /// </summary>
        public FileInfo ConfigFile = new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configs", "Procon.Core.Test.Plugins.TestPluginConfig.json"));

        [SetUp]
        public void ClearConfigDirectory() {
            this.ConfigFile.Refresh();

            if (this.ConfigFile.Directory != null) {
                if (this.ConfigFile.Directory.Exists == true) {
                    this.ConfigFile.Directory.Delete(true);
                }

                this.ConfigFile.Directory.Create();
            }
        }

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

            config.Save(this.ConfigFile);

            // Now load up the config and ensure it saved what we wanted it too.

            JObject deseralized = JsonConvert.DeserializeObject(File.ReadAllText(this.ConfigFile.FullName)) as JObject;

            Assert.IsNotNull(deseralized);
            Assert.AreEqual("PluginsEnable", deseralized["Procon.Core.Connections.Plugins.CorePluginController"].First.Value<String>("Name"));


            Assert.AreEqual(connectionGuid.ToString(), deseralized["Procon.Core.Connections.Plugins.CorePluginController"].First["ScopeModel"].Value<String>("ConnectionGuid"));
            Assert.AreEqual(twoPluginGuid.ToString(), deseralized["Procon.Core.Connections.Plugins.CorePluginController"].First["ScopeModel"].Value<String>("PluginGuid"));
        }
    }
}
