using System;
using System.Reflection;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Procon.Core.Connections.Plugins;

namespace Procon.Core.Test.Plugins {

    [TestFixture]
    public class TestPlugins {

        /// <summary>
        /// Makes sure the plugin is not loaded into the current appdomain.
        /// </summary>
        [Test]
        public void TestPluginsSinglePluginDisposed() {
            PluginController plugins = new PluginController().Execute() as PluginController;

            HostPlugin plugin = plugins.Plugins.First();

            // Dispose of the plugin
            plugin.Dispose();

            Assert.IsNull(plugin.Connection);
            Assert.IsNull(plugin.PluginFactory);
        }

        [Test]
        public void TestPluginsDisposed() {
            PluginController plugins = new PluginController().Execute() as PluginController;

            // Dispose of the controller
            plugins.Dispose();

            Assert.IsNull(plugins.Plugins);
            Assert.IsNull(plugins.AppDomainSandbox);
            Assert.IsNull(plugins.PluginFactory);
        }
    }
}
