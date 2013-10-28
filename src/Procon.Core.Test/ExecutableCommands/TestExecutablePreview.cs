using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Procon.Core.Test.ExecutableCommands.Objects;

namespace Procon.Core.Test.ExecutableCommands {
    /// <summary>
    /// Summary description for ExecutableBase
    /// </summary>
    [TestClass]
    public class TestExecutablePreview {

        /// <summary>
        /// Tests we can match and set a variable in a executable class.
        /// </summary>
        [TestMethod]
        public void TestExecutablePreviewSetPassThrough() {
            ExecutablePreviewTester tester = new ExecutablePreviewTester() {
                TestNumber = 0
            };

            tester.Execute(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSet,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    50
                })
            });

            Assert.AreEqual<int>(tester.TestNumber, 50);
        }

        /// <summary>
        /// Tests we can match and set a variable in a executable class.
        /// </summary>
        [TestMethod]
        public void TestExecutablePreviewSetCancel() {
            ExecutablePreviewTester tester = new ExecutablePreviewTester() {
                TestNumber = 0
            };

            tester.Execute(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSet,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    10
                })
            });

            Assert.AreEqual<int>(0, tester.TestNumber);
        }
    }
}
