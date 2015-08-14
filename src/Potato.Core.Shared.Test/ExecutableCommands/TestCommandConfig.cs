#region Copyright
// Copyright 2014 Myrcon Pty. Ltd.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
using System;
using NUnit.Framework;
using Potato.Core.Shared.Models;

namespace Potato.Core.Shared.Test.ExecutableCommands {
    [TestFixture]
    public class TestCommandConfig {
        /// <summary>
        /// Tests that the scope model is nulled out if the scope model in the source
        /// is also nulled out.
        /// </summary>
        [Test]
        public void TestScopeNulledWhenNulled() {
            var command = new Command() {
                Scope = null
            }.ToConfigCommand();

            Assert.IsNull(command.Command.Scope);
        }

        /// <summary>
        /// Tests that the scope model is nulled out if the scope contains empty guid's
        /// </summary>
        [Test]
        public void TestScopeNulledWhenBothEmpty() {
            var command = new Command() {
                Scope = {
                    ConnectionGuid = Guid.Empty,
                    PluginGuid = Guid.Empty
                }
            }.ToConfigCommand();

            Assert.IsNull(command.Command.Scope);
        }

        /// <summary>
        /// Tests that the scope model is nulled out if the scope contains empty guid's
        /// </summary>
        [Test]
        public void TestScopeMaintainedWhenConnectionGuidNotEmpty() {
            var guid = Guid.NewGuid();

            var command = new Command() {
                Scope = {
                    ConnectionGuid = guid,
                    PluginGuid = Guid.Empty
                }
            }.ToConfigCommand();

            Assert.IsNotNull(command.Command.Scope);
            Assert.AreEqual(command.Command.Scope.ConnectionGuid, guid);
        }

        /// <summary>
        /// Tests that the scope model is nulled out if the scope contains empty guid's
        /// </summary>
        [Test]
        public void TestScopeMaintainedWhenPluginGuidNotEmpty() {
            var guid = Guid.NewGuid();

            var command = new Command() {
                Scope = {
                    ConnectionGuid = Guid.Empty,
                    PluginGuid = guid
                }
            }.ToConfigCommand();

            Assert.IsNotNull(command.Command.Scope);
            Assert.AreEqual(command.Command.Scope.PluginGuid, guid);
        }

        /// <summary>
        /// Tests the authentication will be nulled out when supplied with details
        /// </summary>
        [Test]
        public void TestAuthenticationNulled() {
            var command = new Command() {
                Authentication = new CommandAuthenticationModel() {
                    Uid = "Never seen"
                }
            }.ToConfigCommand();

            Assert.IsNull(command.Command.Authentication);
        }

        /// <summary>
        /// Tests the authentication will be nulled of the resulting config command when nulled to begin with
        /// </summary>
        [Test]
        public void TestAuthenticationNulledWhenNulled() {
            var command = new Command() {
                Authentication = null
            }.ToConfigCommand();

            Assert.IsNull(command.Command.Authentication);
        }
    }
}
