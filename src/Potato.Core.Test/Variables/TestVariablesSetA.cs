#region Copyright
// Copyright 2015 Geoff Green.
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
using System.Linq;
using NUnit.Framework;
using Potato.Core.Shared;
using Potato.Core.Shared.Models;
using Potato.Core.Variables;

namespace Potato.Core.Test.Variables {
    [TestFixture]
    public class TestVariablesSetA {
        [SetUp]
        public void Initialize() {
            SharedReferences.Setup();
        }

        /// <summary>
        ///     Sets a value for the archive, follows same code path as Set, but sets
        ///     the same VariableModel in VariablesArchive.
        /// </summary>
        [Test]
        public void TestValue() {
            var variables = new VariableController();

            // Set an archive variable
            var result = variables.Tunnel(CommandBuilder.VariablesSetA("key", "value").SetOrigin(CommandOrigin.Local));

            // Validate that the command was successful and the key was set to the passed value.
            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Success, result.CommandResultType);
            Assert.AreEqual("value", variables.Get("key", string.Empty));
            Assert.AreEqual("value", variables.ArchiveVariables.Values.First(v => v.Name == "key").ToType<string>());
        }


        [Test]
        public void TestEmptyKeyValue() {
            var variables = new VariableController();

            // Set an archive variable
            var result = variables.Tunnel(CommandBuilder.VariablesSetA(string.Empty, "value").SetOrigin(CommandOrigin.Local));

            // Validate that the command failed
            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.InvalidParameter, result.CommandResultType);
        }

        /// <summary>
        ///     Checks that a user must have permission to set a archive VariableModel.
        /// </summary>
        [Test]
        public void TestInsufficientPermission() {
            var variables = new VariableController();

            var result = variables.Tunnel(CommandBuilder.VariablesSetA("key", "value").SetAuthentication(new CommandAuthenticationModel() {
                Username = "Phogue"
            }).SetOrigin(CommandOrigin.Remote));

            // Validate the command failed because we don't have permissions to execute it.
            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.InsufficientPermissions, result.CommandResultType);
        }

        /// <summary>
        ///     Sets a value for the archive using the common name enum.
        /// </summary>
        [Test]
        public void TestCommonNameValue() {
            var variables = new VariableController();

            // Set an archive variable
            variables.Tunnel(CommandBuilder.VariablesSetA(CommonVariableNames.MaximumProtocolConnections, "value").SetOrigin(CommandOrigin.Local));

            // Validate that the command was successful and the key was set to the passed value.
            Assert.AreEqual("value", variables.Get(CommonVariableNames.MaximumProtocolConnections, string.Empty));
            Assert.AreEqual("value", variables.ArchiveVariables.Values.First(v => v.Name == CommonVariableNames.MaximumProtocolConnections.ToString()).ToType<string>());
        }

        /// <summary>
        ///     Checks that we can override the value of an existing key in the VariableModel archive.
        /// </summary>
        [Test]
        public void TestOverrideExisting() {
            var variables = new VariableController();

            // Set an archive variable
            var result = variables.Tunnel(CommandBuilder.VariablesSetA("key", "value").SetOrigin(CommandOrigin.Local));

            // Validate that initially setting the VariableModel is successful.
            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Success, result.CommandResultType);
            Assert.AreEqual("value", variables.Get("key", string.Empty));
            Assert.AreEqual("value", variables.ArchiveVariables.Values.First(v => v.Name == "key").ToType<string>());

            result = variables.Tunnel(CommandBuilder.VariablesSetA("key", "changed value").SetOrigin(CommandOrigin.Local));

            // Validate that we changed changed an existing VariableModel value.
            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Success, result.CommandResultType);
            Assert.AreEqual("changed value", variables.Get("key", string.Empty));
            Assert.AreEqual("changed value", variables.ArchiveVariables.Values.First(v => v.Name == "key").ToType<string>());
        }

        /// <summary>
        /// Tests that setting an archive value will remove it from the flash variables list.
        /// </summary>
        [Test]
        public void TestRemovesFlashVariable() {
            var variables = new VariableController();

            // Set a flash variable.
            variables.Tunnel(CommandBuilder.VariablesSetF("key", "value").SetOrigin(CommandOrigin.Local));

            // Set a archive value
            variables.Tunnel(CommandBuilder.VariablesSetA("key", "value").SetOrigin(CommandOrigin.Local));

            // Validate archive value exists and flash value does not.
            Assert.IsEmpty(variables.FlashVariables);
            Assert.IsNotEmpty(variables.ArchiveVariables);
        }
    }
}