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
using System.IO;
using System.Linq;
using NUnit.Framework;
using Potato.Core.Connections;
using Potato.Core.Connections.Plugins;
using Potato.Net.Shared.Utils;

namespace Potato.Examples.Plugins.Configs.Test {
    /// <summary>
    /// We don't actually assert on any of these tests, since config saving/loading is tested
    /// elsewhere 
    /// </summary>
    [TestFixture]
    public class TestConfigSerialization {
        /// <summary>
        /// Test for you to debug.
        /// Set a breakpoint within Potato.Examples.Configs.Program.WriteConfig
        /// </summary>
        [Test]
        public void TestConfigSaving() {
            // A guid for this connection based on an md5 hash. If none is supplied
            // then the configs directory will have an empty guid directory name in it.
            // I added this here just so you know what's used to name the directory.
            var connectionGuid = MD5.Guid("not a zero guid");

            var pluginsSaving = new CorePluginController() {
                Connection = new ConnectionController() {
                    ConnectionModel = {
                        ConnectionGuid = connectionGuid
                    }
                }
            }.Execute() as CorePluginController;

            var pluginGuid = pluginsSaving.LoadedPlugins.First().PluginGuid;

            // Now shut it down..
            pluginsSaving.Dispose();

            var configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configs", connectionGuid.ToString(), pluginGuid.ToString(), "Potato.Examples.Plugins.Configs.json");

            // See [execution path]/Configs/477b1278-7b48-f5ae-4f91-f9ba12e204e7/bf3b9c62-050a-4d25-bb2e-0bb48a394eb5/Potato.Examples.Configs.xml
            Assert.IsTrue(File.Exists(configPath));

            File.Delete(configPath);
        }

        /// <summary>
        /// Test for you to debug.
        /// Set a breakpoint within Potato.Examples.Configs.Program.ThisIsJustACommand
        /// </summary>
        [Test]
        public void TestConfigLoading() {
            // A guid for this connection based on an md5 hash. If none is supplied
            // then the configs directory will have an empty guid directory name in it.
            // I added this here just so you know what's used to name the directory.
            var connectionGuid = MD5.Guid("not a zero guid");

            var pluginsSaving = new CorePluginController() {
                Connection = new ConnectionController() {
                    ConnectionModel = {
                        ConnectionGuid = connectionGuid
                    }
                }
            }.Execute() as CorePluginController;

            var pluginGuid = pluginsSaving.LoadedPlugins.First().PluginGuid;

            // Now shut it down..
            pluginsSaving.Dispose();

            var configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configs", connectionGuid.ToString(), pluginGuid.ToString(), "Potato.Examples.Plugins.Configs.json");

            // See [execution path]/Configs/477b1278-7b48-f5ae-4f91-f9ba12e204e7/bf3b9c62-050a-4d25-bb2e-0bb48a394eb5/Potato.Examples.Configs.xml
            Assert.IsTrue(File.Exists(configPath));

            // Now the config is saved we can load up the config.
            // ---------- 
            new CorePluginController() {
                Connection = new ConnectionController() {
                    ConnectionModel = {
                        ConnectionGuid = connectionGuid
                    }
                }
            }.Execute();

            // that's it.

            File.Delete(configPath);
        }
    }
}
