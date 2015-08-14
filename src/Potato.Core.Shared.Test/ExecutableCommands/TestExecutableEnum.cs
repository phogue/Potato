#region Copyright
// Copyright 2015 Geoff Green.
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion
#region

using System;
using System.Collections.Generic;
using NUnit.Framework;
using Potato.Core.Shared.Test.ExecutableCommands.Objects;

#endregion

namespace Potato.Core.Shared.Test.ExecutableCommands {
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