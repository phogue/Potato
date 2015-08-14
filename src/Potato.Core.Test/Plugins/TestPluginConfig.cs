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
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Potato.Core.Connections;
using Potato.Core.Connections.Plugins;
using Potato.Core.Shared;
using Potato.Core.Shared.Models;
using Potato.Core.Shared.Serialization;

namespace Potato.Core.Test.Plugins {
    [TestFixture]
    public class TestPluginConfig {
        /// <summary>
        /// Test file for us to save/load from.
        /// </summary>
        public FileInfo ConfigFile = new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configs", "Potato.Core.Test.Plugins.TestPluginConfig.json"));

        [SetUp]
        public void ClearConfigDirectory() {
            ConfigFile.Refresh();

            if (ConfigFile.Directory != null) {
                if (ConfigFile.Directory.Exists == true) {
                    ConfigFile.Directory.Delete(true);
                }

                ConfigFile.Directory.Create();
            }
        }

        /// <summary>
        /// Tests that an enabled plugin will have the enabled command saved to the config
        /// </summary>
        [Test]
        public void TestEnabledPluginSavedToConfig() {
            var connectionGuid = Guid.NewGuid();
            var onePluginGuid = Guid.NewGuid();
            var twoPluginGuid = Guid.NewGuid();

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

            var config = new Config().Create<CorePluginController>();

            plugins.WriteConfig(config);

            config.Save(ConfigFile);

            // Now load up the config and ensure it saved what we wanted it too.

            var loadConfig = new Config();
            loadConfig.Load(ConfigFile);

            var commands = loadConfig.RootOf<CorePluginController>().Children<JObject>().Select(item => item.ToObject<IConfigCommand>(JsonSerialization.Minimal)).ToList();

            Assert.AreEqual("PluginsEnable", commands[0].Command.Name);
            Assert.AreEqual(connectionGuid, commands[0].Command.Scope.ConnectionGuid);
            Assert.AreEqual(twoPluginGuid, commands[0].Command.Scope.PluginGuid);
        }
    }
}
