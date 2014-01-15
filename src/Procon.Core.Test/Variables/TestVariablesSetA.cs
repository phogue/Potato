#region

using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Procon.Core.Shared;
using Procon.Core.Shared.Models;
using Procon.Core.Variables;

#endregion

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
        public void TestVariablesSetValueA() {
            var variables = new VariableController();

            // Set an archive variable
            CommandResultArgs result = variables.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSetA,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "key",
                    "value"
                })
            });

            // Validate that the command was successful and the key was set to the passed value.
            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Success, result.Status);
            Assert.AreEqual("value", variables.Get("key", String.Empty));
            Assert.AreEqual("value", variables.ArchiveVariables.First(v => v.Name == "key").ToType<String>());
        }

        /// <summary>
        ///     Sets a value for the archive using the common name enum.
        /// </summary>
        [Test]
        public void TestVariablesSetValueACommonName() {
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
        public void TestVariablesSetValueAOverrideExisting() {
            var variables = new VariableController();

            // Set an archive variable
            CommandResultArgs result = variables.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSetA,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "key",
                    "value"
                })
            });

            // Validate that initially setting the VariableModel is successful.
            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Success, result.Status);
            Assert.AreEqual("value", variables.Get("key", String.Empty));
            Assert.AreEqual("value", variables.ArchiveVariables.First(v => v.Name == "key").ToType<String>());

            result = variables.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSetA,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "key",
                    "changed value"
                })
            });

            // Validate that we changed changed an existing VariableModel value.
            Assert.IsTrue(result.Success);
            Assert.AreEqual(CommandResultType.Success, result.Status);
            Assert.AreEqual("changed value", variables.Get("key", String.Empty));
            Assert.AreEqual("changed value", variables.ArchiveVariables.First(v => v.Name == "key").ToType<String>());
        }
    }
}