using System;
using System.Reflection;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Procon.Core.Utils;

namespace Procon.Core.Test.Plugins {
    using Procon.Core.Connections;
    using Procon.Core.Connections.Plugins;
    using Procon.Net.Protocols.Frostbite.BF.BF3;

    [TestClass]
    public class TestPluginsIsolation {
        
        /// <summary>
        /// Makes sure the plugin is not loaded into the current appdomain.
        /// </summary>
        [TestMethod]
        public void TestPluginsIsolationCleanCurrentAppDomain() {
            PluginController plugins = new PluginController().Execute() as PluginController;

            // Send a command to ensure the appdomain actually has a functional copy of the TestPlugin
            // assembly loaded.
            CommandResultArgs result = plugins.Execute(new Command() {
                Name = "TestPluginsIsolationCleanCurrentAppDomain",
                Username = "Phogue",
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "Return Message"
                })
            });

            // Validate the return, this will assert if the command wasn't executed implying the 
            // assembly isn't loaded in the other app domain.
            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Success, result.Status);
            Assert.AreEqual("Return Message", result.Message);

            // Now make sure our current appdomain is clean of the test plugin
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies()) {
                Assert.IsFalse(assembly.FullName.Contains("TestPlugin"));
            }
        }

        /// <summary>
        /// Helper to test writing files to various directories and testing the output.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="expectedSuccessFlag"></param>
        /// <param name="resultType"></param>
        protected void TestPluginsIsolationWriteToDirectory(String path, bool expectedSuccessFlag, CommandResultType resultType) {
            PluginController plugins = new PluginController().Execute() as PluginController;

            CommandResultArgs result = plugins.Execute(new Command() {
                Name = "TestPluginsIsolationWriteToDirectory",
                Username = "Phogue",
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    path
                })
            });

            Assert.IsTrue(result.Success == expectedSuccessFlag);
            Assert.AreEqual(resultType, result.Status);
        }

        /// <summary>
        /// Tests that a plugin in the AppDomain cannot write to the root directory of Procon
        /// </summary>
        [TestMethod]
        public void TestPluginsIsolationProhibtedWriteAccessToRootDirectory() {
            this.TestPluginsIsolationWriteToDirectory(AppDomain.CurrentDomain.BaseDirectory, false, CommandResultType.Failed);
        }

        /// <summary>
        /// Tests that a plugin can write to the plugins directory.
        /// </summary>
        [TestMethod]
        public void TestPluginsIsolationAllowedWriteAccessToPluginsDirectory() {
            this.TestPluginsIsolationWriteToDirectory(Defines.PluginsDirectory, true, CommandResultType.Success);
        }

        /// <summary>
        /// Tests that a plugin can write to the plugins directory.
        /// </summary>
        [TestMethod]
        public void TestPluginsIsolationAllowedWriteAccessToLogsDirectory() {
            this.TestPluginsIsolationWriteToDirectory(Defines.LogsDirectory, true, CommandResultType.Success);
        }

        /// <summary>
        /// Tests that a plugin can write to the plugins directory.
        /// </summary>
        [TestMethod]
        public void TestPluginsIsolationProhibitedWriteAccessToLocalizationDirectory() {
            this.TestPluginsIsolationWriteToDirectory(Defines.LocalizationDirectory, false, CommandResultType.Failed);
        }
    }
}
