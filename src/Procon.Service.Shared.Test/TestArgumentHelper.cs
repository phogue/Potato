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
    }
}
