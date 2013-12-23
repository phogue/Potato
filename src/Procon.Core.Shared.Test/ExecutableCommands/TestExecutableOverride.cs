#region

using System;
using System.Collections.Generic;
using System.Globalization;
using NUnit.Framework;
using Procon.Core.Shared.Test.ExecutableCommands.Objects;

#endregion

namespace Procon.Core.Shared.Test.ExecutableCommands {
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
                Parameters = new List<CommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            Content = new List<string>() {
                                50.ToString(CultureInfo.InvariantCulture)
                            }
                        }
                    }
                }
            });

            Assert.AreEqual(50, tester.TestNumber);
        }
    }
}