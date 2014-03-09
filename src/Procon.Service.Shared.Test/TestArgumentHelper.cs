using System;
using System.Collections.Generic;
using NUnit.Framework;

namespace Procon.Service.Shared.Test {
    [TestFixture]
    public class TestArgumentHelper {
        /// <summary>
        /// Tests the four falsey strings will equate to false.
        /// </summary>
        [Test]
        public void TestIsFalsey() {
            Assert.IsTrue(ArgumentHelper.IsFalsey("false"));
            Assert.IsTrue(ArgumentHelper.IsFalsey("False"));
            Assert.IsTrue(ArgumentHelper.IsFalsey("0"));
            Assert.IsFalse(ArgumentHelper.IsFalsey("1"));
            Assert.IsFalse(ArgumentHelper.IsFalsey("True"));
            Assert.IsFalse(ArgumentHelper.IsFalsey("Literally anything else"));
        }

        /// <summary>
        /// Tests that a value is numeric and is returned.
        /// </summary>
        [Test]
        public void TestNumericSuccess() {
            Assert.AreEqual(50.01F, ArgumentHelper.ParseNumeric("50.01"));
        }

        /// <summary>
        /// Tests that if the input string isn't numeric then the default value will be returned.
        /// </summary>
        [Test]
        public void TestNumericFailureReturnDefault() {
            Assert.AreEqual(49.99F, ArgumentHelper.ParseNumeric("gg", 49.99F));
        }

        /// <summary>
        /// Tests an empty list will produce an empty arguments dictionary
        /// </summary>
        [Test]
        public void TestToArgumentsEmptyList() {
            var arguments = ArgumentHelper.ToArguments(new List<String>());

            Assert.IsEmpty(arguments);
        }

        /// <summary>
        /// Tests passing through a -key without a value will give it a value of "1"
        /// for flags implied as being "on"
        /// </summary>
        [Test]
        public void TestToArgumentsImpliedFlagEnabled() {
            var arguments = ArgumentHelper.ToArguments(new List<String>() {
                "-key"
            });

            Assert.IsNotEmpty(arguments);
            Assert.AreEqual("1", arguments["key"]);
        }

        /// <summary>
        /// Tests passing through a -key without a value will give it a value of "1"
        /// for flags implied as being "on"
        /// </summary>
        [Test]
        public void TestToArgumentsImpliedFlagEnabledUpdate() {
            var arguments = ArgumentHelper.ToArguments(new List<String>() {
                "-key",
                "value",
                "-key"
            });

            Assert.IsNotEmpty(arguments);
            Assert.AreEqual("1", arguments["key"]);
        }

        /// <summary>
        /// Tests passing through a -key with a value will set that key to value.
        /// </summary>
        [Test]
        public void TestToArgumentsExplicitValue() {
            var arguments = ArgumentHelper.ToArguments(new List<String>() {
                "-key",
                "value"
            });

            Assert.IsNotEmpty(arguments);
            Assert.AreEqual("value", arguments["key"]);
        }

        /// <summary>
        /// Tests passing through a -key with a value will set that key to value, updating
        /// existing values if they already exist in the arguments
        /// </summary>
        [Test]
        public void TestToArgumentsExplicitValueUpdate() {
            var arguments = ArgumentHelper.ToArguments(new List<String>() {
                "-key",
                "value1",
                "-key",
                "value2"
            });

            Assert.IsNotEmpty(arguments);
            Assert.AreEqual("value2", arguments["key"]);
        }

        /// <summary>
        /// Tests passing through a -key with a value will set that key to value, updating
        /// existing values if they already exist in the arguments even if both keys
        /// were passed through with different case
        /// </summary>
        [Test]
        public void TestToArgumentsExplicitValueUpdateCaseInsensitive() {
            var arguments = ArgumentHelper.ToArguments(new List<String>() {
                "-key",
                "value1",
                "-Key",
                "value2"
            });

            Assert.IsNotEmpty(arguments);
            Assert.AreEqual("value2", arguments["key"]);
        }

        /// <summary>
        /// This test ensures that ordered input will result in ordered output with dictionary values.
        /// </summary>
        [Test]
        public void TestToArgumentsAlphaKeysValuesOrderingMaintained() {
            var arguments = ArgumentHelper.ToArguments(new List<String>() {
                "-a",
                "A",
                "-b",
                "B",
                "-c",
                "C"
            });

            List<String> values = new List<String>(arguments.Values);

            Assert.AreEqual("A", values[0]);
            Assert.AreEqual("B", values[1]);
            Assert.AreEqual("C", values[2]);
        }

        /// <summary>
        /// Test ensures that unordered input will result in ordered output with dictionary values.
        /// </summary>
        [Test]
        public void TestToArgumentsAlphaKeysValuesOrderingSorted() {
            var arguments = ArgumentHelper.ToArguments(new List<String>() {
                "-a",
                "A",
                "-c",
                "C",
                "-b",
                "B"
            });

            List<String> values = new List<String>(arguments.Values);

            Assert.AreEqual("A", values[0]);
            Assert.AreEqual("B", values[1]);
            Assert.AreEqual("C", values[2]);
        }

        /// <summary>
        /// This test ensures that ordered input will result in ordered output with dictionary values.
        /// </summary>
        [Test]
        public void TestToArgumentsNumericKeysValuesOrderingMaintained() {
            var arguments = ArgumentHelper.ToArguments(new List<String>() {
                "-1",
                "A",
                "-2",
                "B",
                "-3",
                "C"
            });

            List<String> values = new List<String>(arguments.Values);

            Assert.AreEqual("A", values[0]);
            Assert.AreEqual("B", values[1]);
            Assert.AreEqual("C", values[2]);
        }

        /// <summary>
        /// Test ensures that unordered input will result in ordered output with dictionary values.
        /// </summary>
        [Test]
        public void TestToArgumentsNumericKeysValuesOrderingSorted() {
            var arguments = ArgumentHelper.ToArguments(new List<String>() {
                "-1",
                "A",
                "-3",
                "C",
                "-2",
                "B"
            });

            List<String> values = new List<String>(arguments.Values);

            Assert.AreEqual("A", values[0]);
            Assert.AreEqual("B", values[1]);
            Assert.AreEqual("C", values[2]);
        }
    }
}
