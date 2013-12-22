#region

using System.Linq;
using NUnit.Framework;
using Procon.Core.Connections.Plugins;

#endregion

namespace Procon.Core.Test.Plugins {
    [TestFixture]
    public class TestPlugins {
        [Test]
        public void TestPluginsDisposed() {
            var plugins = new PluginController().Execute() as PluginController;

            // Dispose of the controller
            plugins.Dispose();

            Assert.IsNull(plugins.Plugins);
            Assert.IsNull(plugins.AppDomainSandbox);
            Assert.IsNull(plugins.PluginFactory);
        }

        /// <summary>
        ///     Makes sure the plugin is not loaded into the current appdomain.
        /// </summary>
        [Test]
        public void TestPluginsSinglePluginDisposed() {
            var plugins = new PluginController().Execute() as PluginController;

            HostPlugin plugin = plugins.Plugins.First();

            // Dispose of the plugin
            plugin.Dispose();

            Assert.IsNull(plugin.PluginFactory);
        }
    }
}