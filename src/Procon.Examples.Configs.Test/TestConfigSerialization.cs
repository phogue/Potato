using System;
using System.IO;
using System.Linq;
using NUnit.Framework;
using Procon.Core.Connections;
using Procon.Core.Connections.Plugins;
using Procon.Net.Utils;

namespace Procon.Examples.Configs.Test {
    /// <summary>
    /// We don't actually assert on any of these tests, since config saving/loading is tested
    /// elsewhere 
    /// </summary>
    [TestFixture]
    public class TestConfigSerialization {
        /// <summary>
        /// Test for you to debug.
        /// Set a breakpoint within Procon.Examples.Configs.Program.WriteConfig
        /// </summary>
        [Test]
        public void TestConfigSaving() {
            // A guid for this connection based on an md5 hash. If none is supplied
            // then the configs directory will have an empty guid directory name in it.
            // I added this here just so you know what's used to name the directory.
            Guid connectionGuid = MD5.Guid("not a zero guid");

            PluginController pluginsSaving = new PluginController() {
                Connection = new Connection() {
                    ConnectionModel = {
                        ConnectionGuid = connectionGuid
                    }
                }
            }.Execute() as PluginController;

            Guid pluginGuid = pluginsSaving.Plugins.First().PluginModel.PluginGuid;

            // Now shut it down..
            pluginsSaving.Dispose();

            String configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configs", connectionGuid.ToString(), pluginGuid.ToString(), "Procon.Examples.Configs.xml");

            // See [execution path]/Configs/477b1278-7b48-f5ae-4f91-f9ba12e204e7/bf3b9c62-050a-4d25-bb2e-0bb48a394eb5/Procon.Examples.Configs.xml
            Assert.IsTrue(File.Exists(configPath));

            File.Delete(configPath);
        }

        /// <summary>
        /// Test for you to debug.
        /// Set a breakpoint within Procon.Examples.Configs.Program.ThisIsJustACommand
        /// </summary>
        [Test]
        public void TestConfigLoading() {
            // A guid for this connection based on an md5 hash. If none is supplied
            // then the configs directory will have an empty guid directory name in it.
            // I added this here just so you know what's used to name the directory.
            Guid connectionGuid = MD5.Guid("not a zero guid");

            PluginController pluginsSaving = new PluginController() {
                Connection = new Connection() {
                    ConnectionModel = {
                        ConnectionGuid = connectionGuid
                    }
                }
            }.Execute() as PluginController;

            Guid pluginGuid = pluginsSaving.Plugins.First().PluginModel.PluginGuid;

            // Now shut it down..
            pluginsSaving.Dispose();

            String configPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Configs", connectionGuid.ToString(), pluginGuid.ToString(), "Procon.Examples.Configs.xml");

            // See [execution path]/Configs/477b1278-7b48-f5ae-4f91-f9ba12e204e7/bf3b9c62-050a-4d25-bb2e-0bb48a394eb5/Procon.Examples.Configs.xml
            Assert.IsTrue(File.Exists(configPath));

            // Now the config is saved we can load up the config.
            // ---------- 
            new PluginController() {
                Connection = new Connection() {
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
