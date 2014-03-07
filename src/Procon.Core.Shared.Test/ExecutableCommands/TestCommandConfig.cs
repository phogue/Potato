using System;
using NUnit.Framework;

namespace Procon.Core.Shared.Test.ExecutableCommands {
    [TestFixture]
    public class TestCommandConfig {
        /// <summary>
        /// Tests that the scope model is nulled out if the scope model in the source
        /// is also nulled out.
        /// </summary>
        [Test]
        public void TestScopeNulledWhenNulled() {
            ICommand command = new Command() {
                ScopeModel = null
            }.ToConfigCommand();

            Assert.IsNull(command.ScopeModel);
        }

        /// <summary>
        /// Tests that the scope model is nulled out if the scope contains empty guid's
        /// </summary>
        [Test]
        public void TestScopeNulledWhenBothEmpty() {
            ICommand command = new Command() {
                ScopeModel = {
                    ConnectionGuid = Guid.Empty,
                    PluginGuid = Guid.Empty
                }
            }.ToConfigCommand();

            Assert.IsNull(command.ScopeModel);
        }

        /// <summary>
        /// Tests that the scope model is nulled out if the scope contains empty guid's
        /// </summary>
        [Test]
        public void TestScopeMaintainedWhenConnectionGuidNotEmpty() {
            Guid guid = Guid.NewGuid();

            ICommand command = new Command() {
                ScopeModel = {
                    ConnectionGuid = guid,
                    PluginGuid = Guid.Empty
                }
            }.ToConfigCommand();

            Assert.IsNotNull(command.ScopeModel);
            Assert.AreEqual(command.ScopeModel.ConnectionGuid, guid);
        }

        /// <summary>
        /// Tests that the scope model is nulled out if the scope contains empty guid's
        /// </summary>
        [Test]
        public void TestScopeMaintainedWhenPluginGuidNotEmpty() {
            Guid guid = Guid.NewGuid();

            ICommand command = new Command() {
                ScopeModel = {
                    ConnectionGuid = Guid.Empty,
                    PluginGuid = guid
                }
            }.ToConfigCommand();

            Assert.IsNotNull(command.ScopeModel);
            Assert.AreEqual(command.ScopeModel.PluginGuid, guid);
        }
    }
}
