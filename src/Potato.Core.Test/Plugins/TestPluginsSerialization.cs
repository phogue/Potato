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
using Potato.Core.Shared;

#endregion

namespace Potato.Core.Test.Plugins {
    [TestFixture]
    public class TestPluginsSerialization {
        [SetUp]
        public void Initialize() {
            SharedReferences.Setup();
        }

        /// <summary>
        ///     Makes sure executing a command across the appdomain will serialize
        ///     the basic command result across the app domain.
        /// </summary>
        [Test]
        public void TestPluginsSerializationCommandResult() {
            var plugins = (CorePluginController)new CorePluginController().Execute();

            plugins.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.PluginsEnable,
                Scope = {
                    PluginGuid = plugins.LoadedPlugins.First().PluginGuid
                }
            });

            var result = plugins.Tunnel(new Command() {
                Name = "TestPluginsSerializationCommandResult",
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
    }
}