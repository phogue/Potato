using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Procon.Core.Test.ExecutableCommands.Objects;

namespace Procon.Core.Test.ExecutableCommands {
    [TestClass]
    public class TestExecutableEnum {

        /// <summary>
        /// Tests that a flag enumerator will be passed through with the same type.
        /// </summary>
        [TestMethod]
        public void TestExecutableEnumParserFlags() {
            ExecutableEnumTester tester = new ExecutableEnumTester();

            tester.Execute(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSet,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    ExecutableFlagsEnum.Three
                })
            });

            Assert.AreEqual<ExecutableFlagsEnum>(ExecutableFlagsEnum.Three, tester.TestExecutableFlagsEnum);
        }

        /// <summary>
        /// Tests that a enumerator will be passed through with the same type.
        /// </summary>
        [TestMethod]
        public void TestExecutableEnumParser() {
            ExecutableEnumTester tester = new ExecutableEnumTester();

            tester.Execute(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSet,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    ExecutableEnum.Seven
                })
            });

            Assert.AreEqual<ExecutableEnum>(ExecutableEnum.Seven, tester.TestExecutableEnum);
        }

        /// <summary>
        /// Tests that a string value will be converted to a enumerator.
        /// </summary>
        [TestMethod]
        public void TestExecutableEnumParserTypeConversion() {
            ExecutableEnumTester tester = new ExecutableEnumTester();

            tester.Execute(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSet,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    "Eight"
                })
            });

            Assert.AreEqual<ExecutableEnum>(ExecutableEnum.Eight, tester.TestExecutableEnum);
        }
    }
}
