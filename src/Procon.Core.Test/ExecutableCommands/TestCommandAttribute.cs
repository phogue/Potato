#region

using System;
using NUnit.Framework;
using Procon.Core.Shared;

#endregion

namespace Procon.Core.Test.ExecutableCommands {
    [TestFixture]
    public class TestCommandAttribute {
        /// <summary>
        ///     Tests the command attribute can be disposed properly, making the attribute inert.
        /// </summary>
        [Test]
        public void TestCommandAttributeDisposal() {
            var command = new CommandAttribute() {
                Name = "dispose"
            };

            command.Dispose();

            Assert.IsNull(command.Name);
            Assert.AreEqual(CommandType.None, command.CommandType);
        }

        /// <summary>
        ///     Tests equality with another object with the same value.
        /// </summary>
        [Test]
        public void TestCommandAttributeEquality() {
            var commandA = new CommandAttribute() {
                Name = "equal"
            };

            var commandB = new CommandAttribute() {
                Name = "equal"
            };

            Assert.IsTrue(commandA.Equals(commandB));
        }

        /// <summary>
        ///     Tests that a comparison with a different type will come back as false.
        /// </summary>
        [Test]
        public void TestCommandAttributeEqualityAgainstDifferenceType() {
            var commandA = new CommandAttribute() {
                Name = "equal"
            };

            String commandB = "equal";

            Assert.IsFalse(commandA.Equals(commandB));
        }

        /// <summary>
        ///     Tests a comparison with null will not be true.
        /// </summary>
        [Test]
        public void TestCommandAttributeEqualityAgainstNull() {
            var command = new CommandAttribute() {
                Name = "equal"
            };

            Assert.IsFalse(command.Equals(null));
        }

        /// <summary>
        ///     Tests that a command attribute will be equal against a reference to itself.
        /// </summary>
        [Test]
        public void TestCommandAttributeEqualityAgainstReferenceToSameObject() {
            var commandA = new CommandAttribute() {
                Name = "equal"
            };

            CommandAttribute commandB = commandA;

            Assert.IsTrue(commandA.Equals(commandB));
        }

        /// <summary>
        ///     Tests the same value in two different objects with have the same hash codes.
        /// </summary>
        [Test]
        public void TestCommandAttributeEqualityHashCode() {
            var commandA = new CommandAttribute() {
                Name = "equal"
            };

            var commandB = new CommandAttribute() {
                Name = "equal"
            };

            Assert.AreEqual(commandA.GetHashCode(), commandB.GetHashCode());
        }

        /// <summary>
        ///     Tests the equality operator
        /// </summary>
        [Test]
        public void TestCommandAttributeEqualityOperator() {
            var commandA = new CommandAttribute() {
                Name = "equal"
            };

            var commandB = new CommandAttribute() {
                Name = "equal"
            };

            Assert.IsTrue(commandA == commandB);
        }

        /// <summary>
        ///     Tests a reference comparison for equality.
        /// </summary>
        [Test]
        public void TestCommandAttributeEqualityOperatorNull() {
            var commandA = new CommandAttribute() {
                Name = "equal"
            };

            Assert.IsFalse(commandA == null);
        }

        [Test]
        public void TestCommandAttributeInequalityOperator() {
            var commandA = new CommandAttribute() {
                Name = "equalA"
            };

            var commandB = new CommandAttribute() {
                Name = "equalB"
            };

            Assert.IsTrue(commandA != commandB);
        }

        /// <summary>
        ///     Tests that a command type will pass on the value as a string to the Name attribute.
        /// </summary>
        [Test]
        public void TestCommandAttributeNameAliasFromCommandType() {
            var command = new CommandAttribute() {
                CommandType = CommandType.InstanceAddConnection
            };

            Assert.AreEqual("InstanceAddConnection", command.Name);
            Assert.AreEqual(CommandType.InstanceAddConnection, command.CommandType);
        }

        /// <summary>
        ///     Tests that different values will yield a different hash code.
        /// </summary>
        [Test]
        public void TestCommandAttributeNotEqualHashCode() {
            var commandA = new CommandAttribute() {
                Name = "equalA"
            };

            var commandB = new CommandAttribute() {
                Name = "equalB"
            };

            Assert.AreNotEqual(commandA.GetHashCode(), commandB.GetHashCode());
        }

        /// <summary>
        ///     Tests that if a command type is not valid during parsing the Name will at least be populated (good thing)
        /// </summary>
        [Test]
        public void TestCommandAttributeParseInvalidCommandType() {
            CommandAttribute command = new CommandAttribute().ParseCommandType("CustomCommand");

            Assert.AreEqual("CustomCommand", command.Name);
            Assert.AreEqual(CommandType.None, command.CommandType);
        }

        /// <summary>
        ///     Tests the command type can be found from the string.
        /// </summary>
        [Test]
        public void TestCommandAttributeParseValidCommandType() {
            CommandAttribute command = new CommandAttribute().ParseCommandType("InstanceAddConnection");

            Assert.AreEqual("InstanceAddConnection", command.Name);
            Assert.AreEqual(CommandType.InstanceAddConnection, command.CommandType);
        }

        /// <summary>
        ///     Tests a reference comparison for equality.
        /// </summary>
        [Test]
        public void TestCommandAttributeReferenceEqualityOperator() {
            var commandA = new CommandAttribute() {
                Name = "equal"
            };

            CommandAttribute commandB = commandA;

            Assert.IsTrue(commandA == commandB);
        }
    }
}