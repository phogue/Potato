#region

using System.Linq;
using NUnit.Framework;
using Procon.Core.Connections.Plugins;

#endregion

namespace Procon.Core.Test.Plugins {
    [TestFixture]
    public class TestPlugins {
        [SetUp]
        public void Initialize() {
            SharedReferences.Setup();
        }

        [Test]
        public void TestPluginsDisposed() {
            var plugins = new CorePluginController().Execute() as CorePluginController;

            // Dispose of the controller
            plugins.Dispose();

            Assert.IsNull(plugins.LoadedPlugins);
            Assert.IsNull(plugins.AppDomainSandbox);
            Assert.IsNull(plugins.PluginFactory);
        }
    }
}