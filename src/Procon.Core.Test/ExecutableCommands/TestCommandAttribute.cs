using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Procon.Core.Test.ExecutableCommands {
    [TestClass]
    public class TestCommandAttribute {

        /// <summary>
        /// Tests equality with another object with the same value.
        /// </summary>
        [TestMethod]
        public void TestCommandAttributeEquality() {
            CommandAttribute commandA = new CommandAttribute() {
                Name = "equal"
            };

            CommandAttribute commandB = new CommandAttribute() {
                Name = "equal"
            };

            Assert.IsTrue(commandA.Equals(commandB));
        }

        /// <summary>
        /// Tests a comparison with null will not be true.
        /// </summary>
        [TestMethod]
        public void TestCommandAttributeEqualityAgainstNull() {
            CommandAttribute command = new CommandAttribute() {
                Name = "equal"
            };

            Assert.IsFalse(command.Equals(null));
        }

        /// <summary>
        /// Tests that a command attribute will be equal against a reference to itself.
        /// </summary>
        [TestMethod]
        public void TestCommandAttributeEqualityAgainstReferenceToSameObject() {
            CommandAttribute commandA = new CommandAttribute() {
                Name = "equal"
            };

            CommandAttribute commandB = commandA;

            Assert.IsTrue(commandA.Equals(commandB));
        }

        /// <summary>
        /// Tests that a comparison with a different type will come back as false.
        /// </summary>
        [TestMethod]
        public void TestCommandAttributeEqualityAgainstDifferenceType() {
            CommandAttribute commandA = new CommandAttribute() {
                Name = "equal"
            };

            String commandB = "equal";

            Assert.IsFalse(commandA.Equals(commandB));
        }

        /// <summary>
        /// Tests the same value in two different objects with have the same hash codes.
        /// </summary>
        [TestMethod]
        public void TestCommandAttributeEqualityHashCode() {
            CommandAttribute commandA = new CommandAttribute() {
                Name = "equal"
            };

            CommandAttribute commandB = new CommandAttribute() {
                Name = "equal"
            };

            Assert.AreEqual(commandA.GetHashCode(), commandB.GetHashCode());
        }

        /// <summary>
        /// Tests that different values will yield a different hash code.
        /// </summary>
        [TestMethod]
        public void TestCommandAttributeNotEqualHashCode() {
            CommandAttribute commandA = new CommandAttribute() {
                Name = "equalA"
            };

            CommandAttribute commandB = new CommandAttribute() {
                Name = "equalB"
            };

            Assert.AreNotEqual(commandA.GetHashCode(), commandB.GetHashCode());
        }

        /// <summary>
        /// Tests the equality operator
        /// </summary>
        [TestMethod]
        public void TestCommandAttributeEqualityOperator() {
            CommandAttribute commandA = new CommandAttribute() {
                Name = "equal"
            };

            CommandAttribute commandB = new CommandAttribute() {
                Name = "equal"
            };

            Assert.IsTrue(commandA == commandB);
        }

        [TestMethod]
        public void TestCommandAttributeInequalityOperator() {
            CommandAttribute commandA = new CommandAttribute() {
                Name = "equalA"
            };

            CommandAttribute commandB = new CommandAttribute() {
                Name = "equalB"
            };

            Assert.IsTrue(commandA != commandB);
        }

        /// <summary>
        /// Tests a reference comparison for equality.
        /// </summary>
        [TestMethod]
        public void TestCommandAttributeReferenceEqualityOperator() {
            CommandAttribute commandA = new CommandAttribute() {
                Name = "equal"
            };

            CommandAttribute commandB = commandA;

            Assert.IsTrue(commandA == commandB);
        }

        /// <summary>
        /// Tests a reference comparison for equality.
        /// </summary>
        [TestMethod]
        public void TestCommandAttributeEqualityOperatorNull() {
            CommandAttribute commandA = new CommandAttribute() {
                Name = "equal"
            };

            Assert.IsFalse(commandA == null);
        }

        /// <summary>
        /// Tests that a command type will pass on the value as a string to the Name attribute.
        /// </summary>
        [TestMethod]
        public void TestCommandAttributeNameAliasFromCommandType() {
            CommandAttribute command = new CommandAttribute() {
                CommandType = CommandType.InstanceAddConnection
            };

            Assert.AreEqual("InstanceAddConnection", command.Name);
            Assert.AreEqual(CommandType.InstanceAddConnection, command.CommandType);
        }

        /// <summary>
        /// Tests the command type can be found from the string.
        /// </summary>
        [TestMethod]
        public void TestCommandAttributeParseValidCommandType() {
            CommandAttribute command = new CommandAttribute().ParseCommandType("InstanceAddConnection");

            Assert.AreEqual("InstanceAddConnection", command.Name);
            Assert.AreEqual(CommandType.InstanceAddConnection, command.CommandType);
        }

        /// <summary>
        /// Tests that if a command type is not valid during parsing the Name will at least be populated (good thing)
        /// </summary>
        [TestMethod]
        public void TestCommandAttributeParseInvalidCommandType() {
            CommandAttribute command = new CommandAttribute().ParseCommandType("CustomCommand");

            Assert.AreEqual("CustomCommand", command.Name);
            Assert.AreEqual(CommandType.None, command.CommandType);
        }

        /// <summary>
        /// Tests the command attribute can be disposed properly, making the attribute inert.
        /// </summary>
        [TestMethod]
        public void TestCommandAttributeDisposal() {
            CommandAttribute command = new CommandAttribute() {
                Name = "dispose"
            };

            command.Dispose();

            Assert.IsNull(command.Name);
            Assert.AreEqual(CommandType.None, command.CommandType);
        }
    }
}
