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
    public class TestExecutablePreview {
        /// <summary>
        ///     Tests we can match and set a VariableModel in a executable class.
        /// </summary>
        [Test]
        public void TestExecutablePreviewSetCancel() {
            var tester = new ExecutablePreviewTester() {
                TestNumber = 0
            };

            tester.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSet,
                Parameters = new List<ICommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            Content = new List<string>() {
                                10.ToString(CultureInfo.InvariantCulture)
                            }
                        }
                    }
                }
            });

            Assert.AreEqual(0, tester.TestNumber);
        }

        /// <summary>
        ///     Tests we can match and set a VariableModel in a executable class.
        /// </summary>
        [Test]
        public void TestExecutablePreviewSetPassThrough() {
            var tester = new ExecutablePreviewTester() {
                TestNumber = 0
            };

            tester.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSet,
                Parameters = new List<ICommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            Content = new List<string>() {
                                50.ToString(CultureInfo.InvariantCulture)
                            }
                        }
                    }
                }
            });

            Assert.AreEqual(tester.TestNumber, 50);
        }
    }
}