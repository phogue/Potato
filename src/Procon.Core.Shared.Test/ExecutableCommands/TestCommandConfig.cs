using System;
using NUnit.Framework;
using Procon.Core.Shared.Models;

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

        /// <summary>
        /// Tests the authentication will be nulled out when supplied with details
        /// </summary>
        [Test]
        public void TestAuthenticationNulled() {
            ICommand command = new Command() {
                Authentication = new CommandAuthenticationModel() {
                    Uid = "Never seen"
                }
            }.ToConfigCommand();

            Assert.IsNull(command.Authentication);
        }

        /// <summary>
        /// Tests the authentication will be nulled of the resulting config command when nulled to begin with
        /// </summary>
        [Test]
        public void TestAuthenticationNulledWhenNulled() {
            ICommand command = new Command() {
                Authentication = null
            }.ToConfigCommand();

            Assert.IsNull(command.Authentication);
        }
    }
}
