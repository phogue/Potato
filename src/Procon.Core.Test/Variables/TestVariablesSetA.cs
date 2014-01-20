using System;
using System.Linq;
using NUnit.Framework;
using Procon.Core.Shared;
using Procon.Core.Shared.Models;
using Procon.Core.Variables;

namespace Procon.Core.Test.Variables {
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
            ICommandResult result = variables.Tunnel(CommandBuilder.VariablesSetA("key", "value").SetOrigin(CommandOrigin.Local));

            // Validate that the command was successful and the key was set to the passed value.
            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Success, result.Status);
            Assert.AreEqual("value", variables.Get("key", String.Empty));
            Assert.AreEqual("value", variables.ArchiveVariables.First(v => v.Name == "key").ToType<String>());
        }


        [Test]
        public void TestEmptyKeyValue() {
            var variables = new VariableController();

            // Set an archive variable
            ICommandResult result = variables.Tunnel(CommandBuilder.VariablesSetA(String.Empty, "value").SetOrigin(CommandOrigin.Local));

            // Validate that the command failed
            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.InvalidParameter, result.Status);
        }

        /// <summary>
        ///     Checks that a user must have permission to set a archive VariableModel.
        /// </summary>
        [Test]
        public void TestInsufficientPermission() {
            var variables = new VariableController();

            ICommandResult result = variables.Tunnel(CommandBuilder.VariablesSetA("key", "value").SetUsername("Phogue").SetOrigin(CommandOrigin.Remote));

            // Validate the command failed because we don't have permissions to execute it.
            Assert.IsFalse(result.Success);
            Assert.AreEqual(CommandResultType.InsufficientPermissions, result.Status);
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
            Assert.AreEqual("value", variables.Get(CommonVariableNames.MaximumProtocolConnections, String.Empty));
            Assert.AreEqual("value", variables.ArchiveVariables.First(v => v.Name == CommonVariableNames.MaximumProtocolConnections.ToString()).ToType<String>());
        }

        /// <summary>
        ///     Checks that we can override the value of an existing key in the VariableModel archive.
        /// </summary>
        [Test]
        public void TestOverrideExisting() {
            var variables = new VariableController();

            // Set an archive variable
            ICommandResult result = variables.Tunnel(CommandBuilder.VariablesSetA("key", "value").SetOrigin(CommandOrigin.Local));

            // Validate that initially setting the VariableModel is successful.
            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Success, result.Status);
            Assert.AreEqual("value", variables.Get("key", String.Empty));
            Assert.AreEqual("value", variables.ArchiveVariables.First(v => v.Name == "key").ToType<String>());

            result = variables.Tunnel(CommandBuilder.VariablesSetA("key", "changed value").SetOrigin(CommandOrigin.Local));

            // Validate that we changed changed an existing VariableModel value.
            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Success, result.Status);
            Assert.AreEqual("changed value", variables.Get("key", String.Empty));
            Assert.AreEqual("changed value", variables.ArchiveVariables.First(v => v.Name == "key").ToType<String>());
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