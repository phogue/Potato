using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Procon.Core.Test.ExecutableCommands.Objects;

namespace Procon.Core.Test.ExecutableCommands {
    /// <summary>
    /// Summary description for ExecutableBase
    /// </summary>
    [TestClass]
    public class TestExecutableOverride {

        /// <summary>
        /// Tests that an overridden method can still be called with a primitive signature.
        /// </summary>
        [TestMethod]
        public void TestExecutableOverrideSetPrimitive() {
            ExecutableOverrideTester tester = new ExecutableOverrideTester() {
                TestNumber = 0
            };

            tester.Execute(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSet,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    50
                })
            });

            Assert.AreEqual<int>(50, tester.TestNumber);
        }

        /// <summary>
        /// Tests that an overridden method can still be called with an object parameter in the signature.
        /// </summary>
        [TestMethod]
        public void TestExecutableOverrideSetObject() {
            ExecutableOverrideTester tester = new ExecutableOverrideTester() {
                TestNumber = 0
            };
            tester.Execute(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSet,
                Parameters = new List<CommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            Content = new List<String>() {
                                "166.7"
                            }
                        }
                    }
                }
            });

            Assert.AreEqual<int>(166, tester.TestNumber);
        }

    }
}
