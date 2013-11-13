using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Procon.Core.Test.Plugins {
    using Procon.Core.Connections.Plugins;
    [TestFixture]
    public class TestPluginsSerialization {

        /// <summary>
        /// Makes sure executing a command across the appdomain will serialize
        /// the basic command result across the app domain.
        /// </summary>
        [Test]
        public void TestPluginsSerializationCommandResult() {
            PluginController plugins = new PluginController().Execute() as PluginController;

            plugins.Execute(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.PluginsEnable,
                Scope = {
                    PluginGuid = plugins.Plugins.First().PluginGuid
                }
            });

            CommandResultArgs result = plugins.Execute(new Command() {
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
