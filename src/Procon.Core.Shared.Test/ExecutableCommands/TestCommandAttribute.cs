#region

using System;
using NUnit.Framework;

#endregion

namespace Procon.Core.Shared.Test.ExecutableCommands {
    [TestFixture]
    public class TestCommandAttribute {
        /// <summary>
        ///     Tests the command attribute can be disposed properly, making the attribute inert.
        /// </summary>
        [Test]
        public void TestCommandAttributeDisposal() {
            var command = new CommandDispatch() {
                Name = "dispose"
            };

            command.Dispose();

            Assert.IsNull(command.Name);
            Assert.AreEqual(CommandType.None, command.CommandType);
        }

        /// <summary>
        ///     Tests that a command type will pass on the value as a string to the Name attribute.
        /// </summary>
        [Test]
        public void TestCommandAttributeNameAliasFromCommandType() {
            var command = new CommandDispatch() {
                CommandType = CommandType.InstanceAddConnection
            };

            Assert.AreEqual("InstanceAddConnection", command.Name);
            Assert.AreEqual(CommandType.InstanceAddConnection, command.CommandType);
        }

        /// <summary>
        ///     Tests that if a command type is not valid during parsing the Name will at least be populated (good thing)
        /// </summary>
        [Test]
        public void TestCommandAttributeParseInvalidCommandType() {
            CommandDispatch command = new CommandDispatch().ParseCommandType("CustomCommand");

            Assert.AreEqual("CustomCommand", command.Name);
            Assert.AreEqual(CommandType.None, command.CommandType);
        }

        /// <summary>
        ///     Tests the command type can be found from the string.
        /// </summary>
        [Test]
        public void TestCommandAttributeParseValidCommandType() {
            CommandDispatch command = new CommandDispatch().ParseCommandType("InstanceAddConnection");

            Assert.AreEqual("InstanceAddConnection", command.Name);
            Assert.AreEqual(CommandType.InstanceAddConnection, command.CommandType);
        }
    }
}