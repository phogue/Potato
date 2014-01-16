#region

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Procon.Core.Connections.Plugins;
using Procon.Core.Shared;

#endregion

namespace Procon.Core.Test.Plugins {
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
            var plugins = new CorePluginController().Execute() as CorePluginController;

            plugins.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.PluginsEnable,
                Scope = {
                    PluginGuid = plugins.LoadedPlugins.First().PluginGuid
                }
            });

            CommandResult result = plugins.Tunnel(new Command() {
                Name = "TestPluginsSerializationCommandResult",
                Username = "Phogue",
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "Return Message"
                })
            });

            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Success, result.Status);
            Assert.AreEqual("Return Message", result.Message);
        }
    }
}