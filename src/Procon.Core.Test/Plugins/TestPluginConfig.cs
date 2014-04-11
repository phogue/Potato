#region Copyright
// Copyright 2014 Myrcon Pty. Ltd.
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
using Procon.Core.Connections;
using Procon.Core.Connections.Plugins;
using Procon.Core.Shared;
using Procon.Core.Shared.Models;
using Procon.Core.Shared.Serialization;

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

            var loadConfig = new Config();
            loadConfig.Load(this.ConfigFile);

            var commands = loadConfig.RootOf<CorePluginController>().Children<JObject>().Select(item => item.ToObject<IConfigCommand>(JsonSerialization.Minimal)).ToList();

            Assert.AreEqual("PluginsEnable", commands[0].Command.Name);
            Assert.AreEqual(connectionGuid, commands[0].Command.ScopeModel.ConnectionGuid);
            Assert.AreEqual(twoPluginGuid, commands[0].Command.ScopeModel.PluginGuid);
        }
    }
}
