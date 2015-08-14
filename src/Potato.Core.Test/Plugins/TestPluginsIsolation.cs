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
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Potato.Core.Connections.Plugins;
using Potato.Core.Shared;
using Potato.Service.Shared;

namespace Potato.Core.Test.Plugins {
    [TestFixture]
    public class TestPluginsIsolation {
        [SetUp]
        public void Initialize() {
            SharedReferences.Setup();
        }

        /// <summary>
        /// Helper to test writing files to various directories and testing the output.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="expectedSuccessFlag"></param>
        /// <param name="resultType"></param>
        protected void TestPluginsIsolationWriteToDirectory(string path, bool expectedSuccessFlag, CommandResultType resultType) {
            var plugins = (CorePluginController)new CorePluginController().Execute();

            plugins.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.PluginsEnable,
                Scope = {
                    PluginGuid = plugins.LoadedPlugins.First().PluginGuid
                }
            });

            var result = plugins.Tunnel(new Command() {
                Name = "TestPluginsIsolationWriteToDirectory",
                Authentication = {
                    Username = "Phogue"
                },
                Parameters = TestHelpers.ObjectListToContentList(new List<object>() {
                    path
                })
            });

            Assert.IsTrue(result.Success == expectedSuccessFlag);
            Assert.AreEqual(resultType, result.CommandResultType);
        }

        /// <summary>
        /// Tests that a plugin can write to the plugins directory.
        /// </summary>
        [Test]
        public void TestPluginsIsolationAllowedWriteAccessToLogsDirectory() {
            TestPluginsIsolationWriteToDirectory(Defines.LogsDirectory.FullName, true, CommandResultType.Success);
        }

        /// <summary>
        /// Makes sure the plugin is not loaded into the current appdomain.
        /// </summary>
        [Test]
        public void TestPluginsIsolationCleanCurrentAppDomain() {
            var plugins = (CorePluginController)new CorePluginController().Execute();

            plugins.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.PluginsEnable,
                Scope = {
                    PluginGuid = plugins.LoadedPlugins.First().PluginGuid
                }
            });

            // Send a command to ensure the appdomain actually has a functional copy of the TestPlugin
            // assembly loaded.
            var result = plugins.Tunnel(new Command() {
                Name = "TestPluginsIsolationCleanCurrentAppDomain",
                Authentication = {
                    Username = "Phogue"
                },
                Parameters = TestHelpers.ObjectListToContentList(new List<object>() {
                    "Return Message"
                })
            });

            // Validate the return, this will assert if the command wasn't executed implying the 
            // assembly isn't loaded in the other app domain.
            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Success, result.CommandResultType);
            Assert.AreEqual("Return Message", result.Message);

            // Now make sure our current appdomain is clean of the test plugin
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies()) {
                Assert.IsFalse(assembly.FullName.Contains("Myrcon.Plugins.Test"));
            }
        }

        /// <summary>
        /// Tests that a plugin in the AppDomain cannot write to the root directory of Potato
        /// </summary>
        [Test]
        public void TestPluginsIsolationProhibtedWriteAccessToRootDirectory() {
            TestPluginsIsolationWriteToDirectory(AppDomain.CurrentDomain.BaseDirectory, false, CommandResultType.Failed);
        }
    }
}