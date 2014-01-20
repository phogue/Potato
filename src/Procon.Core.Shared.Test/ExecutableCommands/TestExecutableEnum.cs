#region

using System;
using System.Collections.Generic;
using NUnit.Framework;
using Procon.Core.Shared.Test.ExecutableCommands.Objects;

#endregion

namespace Procon.Core.Shared.Test.ExecutableCommands {
    [TestFixture]
    public class TestExecutableEnum {
        /// <summary>
        ///     Tests that a enumerator will be passed through with the same type.
        /// </summary>
        [Test]
        public void TestExecutableEnumParser() {
            var tester = new ExecutableEnumTester();

            tester.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSet,
                Parameters = new List<ICommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            Content = new List<string>() {
                                ExecutableEnum.Seven.ToString()
                            }
                        }
                    }
                }
            });

            Assert.AreEqual(ExecutableEnum.Seven, tester.TestExecutableEnum);
        }

        /// <summary>
        ///     Tests that a flag enumerator will be passed through with the same type.
        /// </summary>
        [Test]
        public void TestExecutableEnumParserFlags() {
            var tester = new ExecutableEnumTester();

            tester.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSet,
                Parameters = new List<ICommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            Content = new List<string>() {
                                ExecutableFlagsEnum.Three.ToString()
                            }
                        }
                    }
                }
            });

            Assert.AreEqual(ExecutableFlagsEnum.Three, tester.TestExecutableFlagsEnum);
        }

        /// <summary>
        ///     Tests that a string value will be converted to a enumerator.
        /// </summary>
        [Test]
        public void TestExecutableEnumParserTypeConversion() {
            var tester = new ExecutableEnumTester();

            tester.Tunnel(new Command() {
                Origin = CommandOrigin.Local,
                CommandType = CommandType.VariablesSet,
                Parameters = new List<ICommandParameter>() {
                    new CommandParameter() {
                        Data = {
                            Content = new List<string>() {
                                "Eight"
                            }
                        }
                    }
                }
            });

            Assert.AreEqual(ExecutableEnum.Eight, tester.TestExecutableEnum);
        }
    }
}