#region

using System;
using System.Collections.Generic;
using NUnit.Framework;
using Procon.Core.Shared;
using Procon.Core.Test.ExecutableCommands.Objects;

#endregion

namespace Procon.Core.Test.ExecutableCommands {
    /// <summary>
    ///     Summary description for ExecutableBase
    /// </summary>
    [TestFixture]
    public class TestExecutableOverride {
        /// <summary>
        ///     Tests that an overridden method can still be called with an object parameter in the signature.
        /// </summary>
        [Test, Ignore]
        
        public void TestExecutableOverrideSetObject() {
            var tester = new ExecutableOverrideTester() {
                TestNumber = 0
            };

            tester.Tunnel(new Command() {
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

            Assert.AreEqual(166, tester.TestNumber);
        }

        /// <summary>
        ///     Tests that an overridden method can still be called with a primitive signature.
        /// </summary>
        [Test]
        public void TestExecutableOverrideSetPrimitive() {
            var tester = new ExecutableOverrideTester() {
                TestNumber = 0
            };

            tester.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSet,
                Parameters = TestHelpers.ObjectListToContentList(new List<Object>() {
                    50
                })
            });

            Assert.AreEqual(50, tester.TestNumber);
        }
    }
}