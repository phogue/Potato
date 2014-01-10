#region

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using Procon.Core.Connections.Plugins;
using Procon.Core.Shared;
using Procon.Service.Shared;

#endregion

namespace Procon.Core.Test.Plugins {
    [TestFixture]
    public class TestPluginsIsolation {
        [SetUp]
        public void Initialize() {
            SharedReferences.Setup();
        }

        /// <summary>
        ///     Helper to test writing files to various directories and testing the output.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="expectedSuccessFlag"></param>
        /// <param name="resultType"></param>
        protected void TestPluginsIsolationWriteToDirectory(String path, bool expectedSuccessFlag, CommandResultType resultType) {
            var plugins = new CorePluginController().Execute() as CorePluginController;

            plugins.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.PluginsEnable,
                Scope = {
                    PluginGuid = plugins.LoadedPlugins.First().PluginGuid
                }
            });

            CommandResultArgs result = plugins.Tunnel(new Command() {
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
        ///     Tests that a plugin can write to the plugins directory.
        /// </summary>
        [Test]
        public void TestPluginsIsolationAllowedWriteAccessToLogsDirectory() {
            TestPluginsIsolationWriteToDirectory(Defines.LogsDirectory, true, CommandResultType.Success);
        }
        /*
        /// <summary>
        ///     Tests that a plugin can write to the plugins directory.
        /// </summary>
        [Test]
        public void TestPluginsIsolationAllowedWriteAccessToPluginsDirectory() {
            TestPluginsIsolationWriteToDirectory(Defines.PluginsDirectory, true, CommandResultType.Success);
        }
        */
        /// <summary>
        ///     Makes sure the plugin is not loaded into the current appdomain.
        /// </summary>
        [Test]
        public void TestPluginsIsolationCleanCurrentAppDomain() {
            var plugins = new CorePluginController().Execute() as CorePluginController;

            plugins.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.PluginsEnable,
                Scope = {
                    PluginGuid = plugins.LoadedPlugins.First().PluginGuid
                }
            });

            // Send a command to ensure the appdomain actually has a functional copy of the TestPlugin
            // assembly loaded.
            CommandResultArgs result = plugins.Tunnel(new Command() {
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
        /*
        /// <summary>
        ///     Tests that a plugin can write to the plugins directory.
        /// </summary>
        [Test]
        public void TestPluginsIsolationProhibitedWriteAccessToLocalizationDirectory() {
            TestPluginsIsolationWriteToDirectory(Defines.LocalizationDirectory, false, CommandResultType.Failed);
        }
        */
        /// <summary>
        ///     Tests that a plugin in the AppDomain cannot write to the root directory of Procon
        /// </summary>
        [Test]
        public void TestPluginsIsolationProhibtedWriteAccessToRootDirectory() {
            TestPluginsIsolationWriteToDirectory(AppDomain.CurrentDomain.BaseDirectory, false, CommandResultType.Failed);
        }
    }
}